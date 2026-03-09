import React, { useState, useRef, useEffect, useLayoutEffect } from 'react';
import { Button } from 'antd';
import { DeleteOutlined, SendOutlined } from '@ant-design/icons';
import { useApiService } from '../../api/useApiService';
import { IPresignedUrlResponse } from '../../types/files';
import axios from 'axios';
import { toast } from 'react-toastify';

export interface VoiceRecordResult {
  fileId: string;
  durationMs: number;
  mimeType: string;
  peaks: number[];
}

interface VoiceRecorderProps {
  onSend: (result: VoiceRecordResult) => Promise<void>;
  onCancel: () => void;
}

type State = 'recording' | 'stopping' | 'preview';

const MAX_DURATION_MS = 3 * 60 * 1000; // 3 minutes

const PREFERRED_MIME_TYPES = [
  'audio/webm;codecs=opus',
  'audio/webm',
  'audio/ogg;codecs=opus',
  'audio/mp4',
];

function detectMimeType(): string {
  if (typeof MediaRecorder === 'undefined') return '';
  for (const type of PREFERRED_MIME_TYPES) {
    if (MediaRecorder.isTypeSupported(type)) return type;
  }
  return '';
}

function mimeToExt(mimeType: string): string {
  if (mimeType.includes('webm')) return 'webm';
  if (mimeType.includes('ogg')) return 'ogg';
  if (mimeType.includes('mp4')) return 'm4a';
  return 'audio';
}

function formatMs(ms: number): string {
  const s = Math.floor(ms / 1000);
  return `${Math.floor(s / 60)}:${String(s % 60).padStart(2, '0')}`;
}

function drawBars(
  canvas: HTMLCanvasElement,
  peaks: number[],
  progress: number,
  live: boolean
) {
  const ctx = canvas.getContext('2d');
  if (!ctx) return;
  const { width, height } = canvas;
  ctx.clearRect(0, 0, width, height);
  const barW = 3;
  const gap = 2;
  const total = Math.floor(width / (barW + gap));
  const midY = height / 2;

  const slice = live ? peaks.slice(-total) : peaks;
  const step = live ? 1 : slice.length / total;

  for (let i = 0; i < total; i++) {
    const idx = live ? i : Math.floor(i * step);
    const peak = slice[idx] ?? 0;
    const barH = Math.max(3, peak * (height - 4));
    const x = i * (barW + gap);
    const played = !live && i / total <= progress;
    ctx.fillStyle = live ? '#ef4444' : played ? '#3b82f6' : '#d1d5db';
    ctx.beginPath();
    ctx.roundRect(x, midY - barH / 2, barW, barH, 1.5);
    ctx.fill();
  }
}

const VoiceRecorder: React.FC<VoiceRecorderProps> = ({ onSend, onCancel }) => {
  const apiService = useApiService();

  const [state, setState] = useState<State>('recording');
  const [elapsedMs, setElapsedMs] = useState(0);
  const [playTimeLabel, setPlayTimeLabel] = useState('');
  const [isPlaying, setIsPlaying] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const canvasRef = useRef<HTMLCanvasElement>(null);
  const mediaRecorderRef = useRef<MediaRecorder | null>(null);
  const audioCtxRef = useRef<AudioContext | null>(null);
  const micSourceRef = useRef<MediaStreamAudioSourceNode | null>(null);
  const streamRef = useRef<MediaStream | null>(null);
  const chunksRef = useRef<Blob[]>([]);
  const peaksRef = useRef<number[]>([]);
  const blobRef = useRef<Blob | null>(null);
  const mimeTypeRef = useRef<string>('');
  const audioRef = useRef<HTMLAudioElement | null>(null);
  const blobUrlRef = useRef<string | null>(null);
  const startTimeRef = useRef<number>(0);
  const durationMsRef = useRef<number>(0);
  const timerRef = useRef<ReturnType<typeof setInterval>>();
  const animFrameRef = useRef<number>();
  const playRafRef = useRef<number>();
  const isMountedRef = useRef(true);

  // Sync canvas internal resolution to its CSS display width to avoid blurry scaling
  useLayoutEffect(() => {
    const canvas = canvasRef.current;
    if (!canvas) return;

    const sync = () => {
      const w = Math.round(canvas.getBoundingClientRect().width);
      if (w > 0 && canvas.width !== w) {
        canvas.width = w;
        // Redraw immediately after resize
        if (state === 'preview') {
          const prog = audioRef.current && audioRef.current.duration
            ? audioRef.current.currentTime / audioRef.current.duration
            : 0;
          drawBars(canvas, peaksRef.current, prog, false);
        }
      }
    };

    const observer = new ResizeObserver(sync);
    observer.observe(canvas);
    sync();
    return () => observer.disconnect();
  }, [state]);

  useEffect(() => {
    startRecording();
    return () => {
      isMountedRef.current = false;
      cleanup();
    };
  }, []);

  const cleanup = () => {
    clearInterval(timerRef.current);
    if (animFrameRef.current) cancelAnimationFrame(animFrameRef.current);
    if (playRafRef.current) cancelAnimationFrame(playRafRef.current);
    const mr = mediaRecorderRef.current;
    if (mr && mr.state !== 'inactive') {
      mr.onstop = null;
      mr.stop();
    }
    micSourceRef.current?.disconnect();
    micSourceRef.current = null;
    streamRef.current?.getTracks().forEach((t) => t.stop());
    audioCtxRef.current?.close();
    audioRef.current?.pause();
    if (blobUrlRef.current) URL.revokeObjectURL(blobUrlRef.current);
  };

  const startRecording = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      // if (!isMountedRef.current) {
      //   stream.getTracks().forEach((t) => t.stop());
      //   return;
      // }
      streamRef.current = stream;

      const ctx = new AudioContext();
      audioCtxRef.current = ctx;
      const source = ctx.createMediaStreamSource(stream);
      micSourceRef.current = source;
      const analyser = ctx.createAnalyser();
      analyser.fftSize = 1024;
      source.connect(analyser);

      const mimeType = detectMimeType();
      mimeTypeRef.current = mimeType;
      const options: MediaRecorderOptions = mimeType ? { mimeType } : {};
      const mediaRecorder = new MediaRecorder(stream, options);
      mediaRecorderRef.current = mediaRecorder;
      chunksRef.current = [];

      mediaRecorder.ondataavailable = (e) => {
        if (e.data.size > 0) chunksRef.current.push(e.data);
      };

      mediaRecorder.start(100);

      const dataArray = new Float32Array(analyser.fftSize);
      const animate = () => {
        analyser.getFloatTimeDomainData(dataArray);
        let max = 0;
        for (let i = 0; i < dataArray.length; i++) {
          const v = Math.abs(dataArray[i]);
          if (v > max) max = v;
        }
        peaksRef.current.push(max);
        if (canvasRef.current) drawBars(canvasRef.current, peaksRef.current, 0, true);
        animFrameRef.current = requestAnimationFrame(animate);
      };
      animFrameRef.current = requestAnimationFrame(animate);

      startTimeRef.current = Date.now();
      timerRef.current = setInterval(() => {
        const elapsed = Date.now() - startTimeRef.current;
        setElapsedMs(elapsed);
        if (elapsed >= MAX_DURATION_MS) stopRecording();
      }, 100);
    } catch (err) {
      console.error('startRecording error:', err);
      toast.error('Не удалось получить доступ к микрофону');
      onCancel();
    }
  };

  const stopRecording = () => {
    clearInterval(timerRef.current);
    if (animFrameRef.current) cancelAnimationFrame(animFrameRef.current);

    // Disconnect the MediaStreamSourceNode synchronously — this is the only reliable way
    // to release the AudioContext's hold on the stream before stopping tracks.
    // audioCtx.close() is async (Promise), so calling it without await still holds
    // the stream reference when track.stop() runs, keeping the mic indicator active.
    micSourceRef.current?.disconnect();
    micSourceRef.current = null;
    audioCtxRef.current?.close();
    audioCtxRef.current = null;

    durationMsRef.current = Date.now() - startTimeRef.current;
    streamRef.current?.getTracks().forEach((t) => t.stop());

    const mediaRecorder = mediaRecorderRef.current;
    if (!mediaRecorder || mediaRecorder.state === 'inactive') return;

    setState('stopping');

    mediaRecorder.onstop = () => {
      const actualMimeType = mimeTypeRef.current || mediaRecorder.mimeType;
      mimeTypeRef.current = actualMimeType;

      const blob = new Blob(chunksRef.current, { type: actualMimeType });
      blobRef.current = blob;

      // Downsample to ~80 peaks and normalize so quiet recordings still fill the waveform
      const raw = peaksRef.current;
      const target = 80;
      const step = Math.max(1, Math.floor(raw.length / target));
      const sampled: number[] = [];
      for (let i = 0; i < raw.length; i += step) sampled.push(raw[i]);

      const maxPeak = Math.max(...sampled, 0.01);
      peaksRef.current = sampled.map((p) => p / maxPeak);

      const url = URL.createObjectURL(blob);
      blobUrlRef.current = url;
      const audio = new Audio(url);
      audioRef.current = audio;
      audio.onended = () => {
        if (playRafRef.current) cancelAnimationFrame(playRafRef.current);
        setIsPlaying(false);
        setPlayTimeLabel('');
        if (canvasRef.current) drawBars(canvasRef.current, peaksRef.current, 0, false);
      };

      setState('preview');
    };

    mediaRecorder.stop();
  };

  // RAF loop drives canvas at 60 fps via direct DOM — zero React re-renders per frame.
  // The time label (React state) only updates when the MM:SS string actually changes,
  // which is at most once per second, keeping re-render pressure negligible.
  const startPlayRaf = (audio: HTMLAudioElement) => {
    let lastLabel = '';
    const tick = () => {
      const progress = audio.duration ? audio.currentTime / audio.duration : 0;

      // Direct canvas write — no state, no reconciler
      if (canvasRef.current) drawBars(canvasRef.current, peaksRef.current, progress, false);

      // State update only when the visible string changes (≤ 1/sec)
      const label = formatMs(audio.currentTime * 1000);
      if (label !== lastLabel) {
        lastLabel = label;
        setPlayTimeLabel(label);
      }

      playRafRef.current = requestAnimationFrame(tick);
    };
    playRafRef.current = requestAnimationFrame(tick);
  };

  const togglePlay = async () => {
    const audio = audioRef.current;
    if (!audio) return;
    if (isPlaying) {
      audio.pause();
      setIsPlaying(false);
      if (playRafRef.current) cancelAnimationFrame(playRafRef.current);
    } else {
      try {
        await audio.play();
        setIsPlaying(true);
        startPlayRaf(audio);
      } catch (err) {
        if ((err as Error).name !== 'AbortError') console.error(err);
      }
    }
  };

  const handleSend = async () => {
    if (!blobRef.current) return;
    setIsSending(true);
    try {
      const mimeType = mimeTypeRef.current || blobRef.current.type;
      const ext = mimeToExt(mimeType);
      const file = new File([blobRef.current], `voice_${Date.now()}.${ext}`, { type: mimeType });
      const formData = new FormData();
      formData.append('File', file);

      const presigned = await apiService.postFormData<IPresignedUrlResponse>(
        '/v1/files/get-presigned-url',
        formData
      );
      if (!presigned.success || !presigned.data) throw new Error('Presigned URL failed');

      const { url, fileId } = presigned.data;
      await axios.put(url, blobRef.current, { headers: { 'Content-Type': mimeType } });
      await apiService.post(`/v1/files/${fileId}/confirm-upload`, [fileId]);

      await onSend({
        fileId,
        durationMs: durationMsRef.current,
        mimeType,
        peaks: peaksRef.current,
      });
    } catch (err) {
      console.error(err);
      toast.error('Не удалось отправить голосовое сообщение');
    } finally {
      setIsSending(false);
    }
  };

  const handleCancel = () => {
    cleanup();
    onCancel();
  };

  const timeLabel = (() => {
    if (state === 'recording') return formatMs(elapsedMs);
    if (state === 'stopping') return formatMs(durationMsRef.current);
    // During playback: show RAF-driven label; when paused/stopped: show total duration
    return (isPlaying && playTimeLabel) ? playTimeLabel : formatMs(durationMsRef.current);
  })();

  return (
    <div className="flex items-center gap-2 px-3 py-2 bg-gray-50 border-t border-gray-200">
      <Button type="text" danger icon={<DeleteOutlined />} onClick={handleCancel} />

      {state === 'recording' && (
        <span className="w-2 h-2 rounded-full bg-red-500 animate-pulse flex-shrink-0" />
      )}
      {state === 'preview' && (
        <Button type="text" size="small" onClick={togglePlay}>
          {isPlaying ? '⏸' : '▶'}
        </Button>
      )}
      {state === 'stopping' && (
        <span className="w-2 h-2 rounded-full bg-gray-400 flex-shrink-0" />
      )}

      <canvas
        ref={canvasRef}
        height={36}
        className="flex-1 cursor-pointer"
        onClick={(e) => {
          if (state !== 'preview' || !audioRef.current?.duration) return;
          const rect = (e.target as HTMLCanvasElement).getBoundingClientRect();
          const ratio = (e.clientX - rect.left) / rect.width;
          audioRef.current.currentTime = ratio * audioRef.current.duration;
          if (!isPlaying && canvasRef.current) {
            drawBars(canvasRef.current, peaksRef.current, ratio, false);
          }
        }}
      />

      <span className="text-sm text-gray-600 tabular-nums min-w-[36px]">{timeLabel}</span>

      {state === 'recording' && (
        <Button type="primary" danger size="small" onClick={stopRecording}>
          ■ Стоп
        </Button>
      )}
      {state === 'stopping' && (
        <Button type="primary" size="small" loading disabled>
          ■ Стоп
        </Button>
      )}
      {state === 'preview' && (
        <Button type="primary" icon={<SendOutlined />} loading={isSending} onClick={handleSend} />
      )}
    </div>
  );
};

export default VoiceRecorder;
