import React, { useRef, useState, useEffect, useCallback } from 'react';
import { Button } from 'antd';
import { IMessageAttachmentResponse } from '../../../../../types/messaging';
import { useApiService } from '../../../../../api/useApiService';

interface VoiceData {
  fileId: { value: string };
  durationMs: number;
  mimeType: string;
  peaks: number[];
}

interface Props {
  attachment: IMessageAttachmentResponse;
  isMyMessage?: boolean;
}

function formatDuration(ms: number): string {
  const totalSec = Math.floor(ms / 1000);
  const m = Math.floor(totalSec / 60);
  const s = totalSec % 60;
  return `${m}:${s.toString().padStart(2, '0')}`;
}

function drawWaveform(
  canvas: HTMLCanvasElement,
  peaks: number[],
  progress: number,
  isMyMessage: boolean
) {
  const ctx = canvas.getContext('2d');
  if (!ctx) return;
  const { width, height } = canvas;
  ctx.clearRect(0, 0, width, height);

  const barWidth = 3;
  const gap = 2;
  const totalBars = Math.floor(width / (barWidth + gap));
  const step = peaks.length / totalBars;
  const midY = height / 2;

  for (let i = 0; i < totalBars; i++) {
    const peakIdx = Math.floor(i * step);
    const peak = peaks[peakIdx] ?? 0;
    const barH = Math.max(3, peak * (height - 4));
    const x = i * (barWidth + gap);
    const played = i / totalBars <= progress;

    ctx.fillStyle = played
      ? (isMyMessage ? '#fff' : '#3b82f6')
      : (isMyMessage ? 'rgba(255,255,255,0.4)' : '#d1d5db');
    ctx.beginPath();
    ctx.roundRect(x, midY - barH / 2, barWidth, barH, 1.5);
    ctx.fill();
  }
}

export const VoiceMessageAttachment: React.FC<Props> = ({ attachment, isMyMessage }) => {
  const apiService = useApiService();
  const data: VoiceData = JSON.parse(attachment.data);

  const canvasRef = useRef<HTMLCanvasElement>(null);
  const animRef = useRef<number>();

  // AudioContext-based playback — fully decodes blob before play so there are
  // no streaming/buffering pauses at MediaRecorder chunk boundaries.
  const audioCtxRef = useRef<AudioContext | null>(null);
  const audioBufferRef = useRef<AudioBuffer | null>(null);
  const sourceNodeRef = useRef<AudioBufferSourceNode | null>(null);
  // Tracks where in the buffer we are (seconds) across pause/resume/seek.
  const playOffsetRef = useRef<number>(0);
  // AudioContext.currentTime when the last play() started.
  const playStartCtxTimeRef = useRef<number>(0);

  const [isPlaying, setIsPlaying] = useState(false);
  const [currentMs, setCurrentMs] = useState(data.durationMs);
  const [loading, setLoading] = useState(false);

  const peaks = data.peaks?.length ? data.peaks : Array(40).fill(0.3);
  const totalDurationSec = data.durationMs / 1000;

  // Draw initial static waveform once on mount (progress = 0)
  useEffect(() => {
    if (canvasRef.current) drawWaveform(canvasRef.current, peaks, 0, !!isMyMessage);
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const stopRaf = useCallback(() => {
    if (animRef.current) {
      cancelAnimationFrame(animRef.current);
      animRef.current = undefined;
    }
  }, []);

  // RAF loop reads AudioContext.currentTime directly — no streaming, no browser guessing.
  const startRaf = useCallback(() => {
    let lastLabel = '';
    const tick = () => {
      const ctx = audioCtxRef.current;
      if (!ctx) return;
      const currentSec = Math.min(
        (ctx.currentTime - playStartCtxTimeRef.current) + playOffsetRef.current,
        totalDurationSec
      );
      const progress = totalDurationSec > 0 ? currentSec / totalDurationSec : 0;
      if (canvasRef.current) drawWaveform(canvasRef.current, peaks, progress, !!isMyMessage);

      const remainingMs = Math.max(0, (totalDurationSec - currentSec) * 1000);
      const label = formatDuration(remainingMs);
      if (label !== lastLabel) {
        lastLabel = label;
        setCurrentMs(remainingMs);
      }

      animRef.current = requestAnimationFrame(tick);
    };
    animRef.current = requestAnimationFrame(tick);
  }, [peaks, isMyMessage, totalDurationSec]);

  const loadBuffer = async (): Promise<AudioBuffer | null> => {
    if (audioBufferRef.current) return audioBufferRef.current;
    setLoading(true);
    const blob = await apiService.getBlob(`/v1/files/${data.fileId.value}`);
    setLoading(false);
    if (!blob) return null;

    const ctx = new AudioContext();
    audioCtxRef.current = ctx;

    // decodeAudioData fully decodes the webm/opus blob into a PCM AudioBuffer —
    // eliminates any chunk-boundary pauses that the <audio> element can have.
    const arrayBuffer = await blob.arrayBuffer();
    const audioBuffer = await ctx.decodeAudioData(arrayBuffer);
    audioBufferRef.current = audioBuffer;
    return audioBuffer;
  };

  const stopSource = () => {
    if (sourceNodeRef.current) {
      sourceNodeRef.current.onended = null;
      try { sourceNodeRef.current.stop(); } catch { /* already stopped */ }
      sourceNodeRef.current = null;
    }
  };

  const playFrom = (offsetSec: number) => {
    const ctx = audioCtxRef.current;
    const buffer = audioBufferRef.current;
    if (!ctx || !buffer) return;

    stopSource();
    const source = ctx.createBufferSource();
    source.buffer = buffer;
    source.connect(ctx.destination);
    source.onended = () => {
      // onended fires both on natural end AND on stop() — guard with isPlaying state
      // by checking if we reached the end naturally.
      const elapsed = ctx.currentTime - playStartCtxTimeRef.current;
      const position = offsetSec + elapsed;
      if (position >= buffer.duration - 0.05) {
        playOffsetRef.current = 0;
        stopRaf();
        setIsPlaying(false);
        setCurrentMs(data.durationMs);
        if (canvasRef.current) drawWaveform(canvasRef.current, peaks, 0, !!isMyMessage);
      }
    };
    playOffsetRef.current = offsetSec;
    playStartCtxTimeRef.current = ctx.currentTime;
    source.start(0, offsetSec);
    sourceNodeRef.current = source;
  };

  const togglePlay = async () => {
    if (isPlaying) {
      // Capture current position before stopping
      const ctx = audioCtxRef.current;
      if (ctx) {
        playOffsetRef.current = Math.min(
          (ctx.currentTime - playStartCtxTimeRef.current) + playOffsetRef.current,
          totalDurationSec
        );
      }
      stopSource();
      stopRaf();
      setIsPlaying(false);
      return;
    }

    let buffer = audioBufferRef.current;
    if (!buffer) {
      buffer = await loadBuffer();
      if (!buffer) return;
    }

    // Resume AudioContext if suspended (browser autoplay policy)
    await audioCtxRef.current?.resume();

    playFrom(playOffsetRef.current);
    setIsPlaying(true);
    startRaf();
  };

  const handleSeek = (e: React.MouseEvent<HTMLCanvasElement>) => {
    const buffer = audioBufferRef.current;
    if (!buffer) return;
    const rect = (e.target as HTMLCanvasElement).getBoundingClientRect();
    const ratio = (e.clientX - rect.left) / rect.width;
    const seekSec = ratio * buffer.duration;

    if (isPlaying) {
      playFrom(seekSec);
    } else {
      playOffsetRef.current = seekSec;
      if (canvasRef.current) drawWaveform(canvasRef.current, peaks, ratio, !!isMyMessage);
      setCurrentMs(Math.max(0, (buffer.duration - seekSec) * 1000));
    }
  };

  useEffect(() => {
    return () => {
      stopSource();
      stopRaf();
      audioCtxRef.current?.close();
    };
  }, [stopRaf]); // eslint-disable-line react-hooks/exhaustive-deps

  const bg = isMyMessage ? 'bg-blue-500' : 'bg-white border border-gray-200';
  const textColor = isMyMessage ? 'text-white' : 'text-gray-700';

  return (
    <div className={`flex items-center gap-2 px-3 py-2 rounded-2xl ${bg} min-w-[220px] max-w-xs`}>
      <Button
        type="text"
        shape="circle"
        size="small"
        loading={loading}
        icon={
          isPlaying
            ? <span className={`text-lg ${isMyMessage ? 'text-white' : 'text-blue-500'}`}>⏸</span>
            : <span className={`text-lg ${isMyMessage ? 'text-white' : 'text-blue-500'}`}>▶</span>
        }
        onClick={togglePlay}
        className="flex-shrink-0"
      />
      <div className="flex-1 flex flex-col gap-1">
        <canvas
          ref={canvasRef}
          width={160}
          height={32}
          className="w-full cursor-pointer"
          onClick={handleSeek}
        />
        <span className={`text-xs ${textColor} opacity-80`}>{formatDuration(currentMs)}</span>
      </div>
    </div>
  );
};
