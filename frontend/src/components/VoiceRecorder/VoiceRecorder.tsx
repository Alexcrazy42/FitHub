import React, { useState, useRef, useEffect } from 'react';
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

type State = 'recording' | 'preview';

const MIME_TYPE = 'audio/wav';
const BUFFER_SIZE = 4096;

function formatMs(ms: number): string {
  const s = Math.floor(ms / 1000);
  return `${Math.floor(s / 60)}:${String(s % 60).padStart(2, '0')}`;
}

function createWavBlob(chunks: Int16Array[], sampleRate: number): Blob {
  const totalSamples = chunks.reduce((s, c) => s + c.length, 0);
  const buffer = new ArrayBuffer(44 + totalSamples * 2);
  const view = new DataView(buffer);

  const write = (off: number, str: string) => {
    for (let i = 0; i < str.length; i++) view.setUint8(off + i, str.charCodeAt(i));
  };

  write(0, 'RIFF');
  view.setUint32(4, 36 + totalSamples * 2, true);
  write(8, 'WAVE');
  write(12, 'fmt ');
  view.setUint32(16, 16, true);
  view.setUint16(20, 1, true);   // PCM
  view.setUint16(22, 1, true);   // mono
  view.setUint32(24, sampleRate, true);
  view.setUint32(28, sampleRate * 2, true);
  view.setUint16(32, 2, true);
  view.setUint16(34, 16, true);
  write(36, 'data');
  view.setUint32(40, totalSamples * 2, true);

  let offset = 44;
  for (const chunk of chunks) {
    for (let i = 0; i < chunk.length; i++) {
      view.setInt16(offset, chunk[i], true);
      offset += 2;
    }
  }

  return new Blob([buffer], { type: MIME_TYPE });
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
  const [playProgress, setPlayProgress] = useState(0);
  const [isPlaying, setIsPlaying] = useState(false);
  const [isSending, setIsSending] = useState(false);

  const canvasRef = useRef<HTMLCanvasElement>(null);
  const audioCtxRef = useRef<AudioContext | null>(null);
  const processorRef = useRef<ScriptProcessorNode | null>(null);
  const streamRef = useRef<MediaStream | null>(null);
  const pcmChunksRef = useRef<Int16Array[]>([]);
  const peaksRef = useRef<number[]>([]);
  const blobRef = useRef<Blob | null>(null);
  const audioRef = useRef<HTMLAudioElement | null>(null);
  const blobUrlRef = useRef<string | null>(null);
  const startTimeRef = useRef<number>(0);
  const durationMsRef = useRef<number>(0);
  const timerRef = useRef<ReturnType<typeof setInterval>>();

  useEffect(() => {
    startRecording();
    return cleanup;
  }, []);

  useEffect(() => {
    if (state === 'preview' && canvasRef.current) {
      drawBars(canvasRef.current, peaksRef.current, playProgress, false);
    }
  }, [playProgress, state]);

  const cleanup = () => {
    clearInterval(timerRef.current);
    processorRef.current?.disconnect();
    streamRef.current?.getTracks().forEach((t) => t.stop());
    audioCtxRef.current?.close();
    audioRef.current?.pause();
    if (blobUrlRef.current) URL.revokeObjectURL(blobUrlRef.current);
  };

  const startRecording = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      streamRef.current = stream;

      const ctx = new AudioContext();
      audioCtxRef.current = ctx;
      await ctx.resume();

      pcmChunksRef.current = [];
      peaksRef.current = [];

      const source = ctx.createMediaStreamSource(stream);
      const processor = ctx.createScriptProcessor(BUFFER_SIZE, 1, 1);
      processorRef.current = processor;

      const gain = ctx.createGain();
      gain.gain.value = 0;

      processor.onaudioprocess = (e) => {
        const pcmFloat = e.inputBuffer.getChannelData(0);

        let max = 0;
        for (let i = 0; i < pcmFloat.length; i++) {
          const v = Math.abs(pcmFloat[i]);
          if (v > max) max = v;
        }
        peaksRef.current.push(max);
        if (canvasRef.current) drawBars(canvasRef.current, peaksRef.current, 0, true);

        const pcmInt16 = new Int16Array(pcmFloat.length);
        for (let i = 0; i < pcmFloat.length; i++) {
          pcmInt16[i] = Math.max(-32768, Math.min(32767, pcmFloat[i] * 32768));
        }
        pcmChunksRef.current.push(pcmInt16);
      };

      source.connect(processor);
      processor.connect(gain);
      gain.connect(ctx.destination);

      startTimeRef.current = Date.now();
      timerRef.current = setInterval(() => setElapsedMs(Date.now() - startTimeRef.current), 100);
    } catch (err) {
      console.error('startRecording error:', err);
      toast.error('Не удалось получить доступ к микрофону');
      onCancel();
    }
  };

  const stopRecording = () => {
    clearInterval(timerRef.current);
    processorRef.current?.disconnect();
    streamRef.current?.getTracks().forEach((t) => t.stop());

    durationMsRef.current = Date.now() - startTimeRef.current;

    const sampleRate = audioCtxRef.current?.sampleRate ?? 44100;
    const blob = createWavBlob(pcmChunksRef.current, sampleRate);
    blobRef.current = blob;

    // Downsample peaks to ~80 for storage
    const raw = peaksRef.current;
    const target = 80;
    const step = Math.max(1, Math.floor(raw.length / target));
    const sampled: number[] = [];
    for (let i = 0; i < raw.length; i += step) sampled.push(raw[i]);
    peaksRef.current = sampled;

    const url = URL.createObjectURL(blob);
    blobUrlRef.current = url;
    const audio = new Audio(url);
    audioRef.current = audio;
    audio.ontimeupdate = () => {
      if (audio.duration) setPlayProgress(audio.currentTime / audio.duration);
    };
    audio.onended = () => {
      setIsPlaying(false);
      setPlayProgress(0);
    };

    audioCtxRef.current?.close();
    audioCtxRef.current = null;
    setState('preview');
  };

  const togglePlay = () => {
    const audio = audioRef.current;
    if (!audio) return;
    if (isPlaying) {
      audio.pause();
      setIsPlaying(false);
    } else {
      audio.play();
      setIsPlaying(true);
    }
  };

  const handleSend = async () => {
    if (!blobRef.current) return;
    setIsSending(true);
    try {
      const file = new File([blobRef.current], `voice_${Date.now()}.wav`, { type: MIME_TYPE });
      const formData = new FormData();
      formData.append('File', file);

      const presigned = await apiService.postFormData<IPresignedUrlResponse>(
        '/v1/files/get-presigned-url',
        formData
      );
      if (!presigned.success || !presigned.data) throw new Error('Presigned URL failed');

      const { url, fileId } = presigned.data;
      await axios.put(url, blobRef.current, { headers: { 'Content-Type': MIME_TYPE } });
      await apiService.post(`/v1/files/${fileId}/confirm-upload`, [fileId]);

      await onSend({
        fileId,
        durationMs: durationMsRef.current,
        mimeType: MIME_TYPE,
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
    if (state === 'recording') stopRecording();
    cleanup();
    onCancel();
  };

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

      <canvas
        ref={canvasRef}
        width={200}
        height={36}
        className="flex-1 cursor-pointer"
        onClick={(e) => {
          if (state !== 'preview' || !audioRef.current?.duration) return;
          const rect = (e.target as HTMLCanvasElement).getBoundingClientRect();
          audioRef.current.currentTime =
            ((e.clientX - rect.left) / rect.width) * audioRef.current.duration;
        }}
      />

      <span className="text-sm text-gray-600 tabular-nums min-w-[36px]">
        {formatMs(state === 'recording' ? elapsedMs : durationMsRef.current)}
      </span>

      {state === 'recording' && (
        <Button type="primary" danger size="small" onClick={stopRecording}>
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
