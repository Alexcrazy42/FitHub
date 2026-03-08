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
  const audioRef = useRef<HTMLAudioElement | null>(null);
  const animRef = useRef<number>();

  const [isPlaying, setIsPlaying] = useState(false);
  const [progress, setProgress] = useState(0);
  const [currentMs, setCurrentMs] = useState(data.durationMs);
  const [blobUrl, setBlobUrl] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const peaks = data.peaks?.length ? data.peaks : Array(40).fill(0.3);

  const redrawCanvas = useCallback(() => {
    if (canvasRef.current) {
      drawWaveform(canvasRef.current, peaks, progress, !!isMyMessage);
    }
  }, [peaks, progress, isMyMessage]);

  useEffect(() => { redrawCanvas(); }, [redrawCanvas]);

  const loadAudio = async (): Promise<HTMLAudioElement | null> => {
    if (blobUrl) return audioRef.current;
    setLoading(true);
    const blob = await apiService.getBlob(`/v1/files/${data.fileId.value}`);
    setLoading(false);
    if (!blob) return null;
    // Force the correct MIME type — the server may return application/octet-stream
    const typedBlob = new Blob([blob], { type: data.mimeType || 'audio/mpeg' });
    const url = URL.createObjectURL(typedBlob);
    setBlobUrl(url);
    const audio = new Audio(url);
    audioRef.current = audio;

    audio.ontimeupdate = () => {
      const p = audio.duration ? audio.currentTime / audio.duration : 0;
      setProgress(p);
      setCurrentMs(Math.max(0, (audio.duration - audio.currentTime) * 1000));
    };
    audio.onended = () => {
      setIsPlaying(false);
      setProgress(0);
      setCurrentMs(data.durationMs);
    };
    return audio;
  };

  const togglePlay = async () => {
    let audio = audioRef.current;
    if (!audio) {
      audio = await loadAudio();
      if (!audio) return;
    }
    if (isPlaying) {
      audio.pause();
      setIsPlaying(false);
    } else {
      try {
        await audio.play();
        setIsPlaying(true);
      } catch (err) {
        if ((err as Error).name !== 'AbortError') console.error(err);
      }
    }
  };

  useEffect(() => {
    return () => {
      audioRef.current?.pause();
      if (blobUrl) URL.revokeObjectURL(blobUrl);
      if (animRef.current) cancelAnimationFrame(animRef.current);
    };
  }, [blobUrl]);

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
          onClick={(e) => {
            if (!audioRef.current || !audioRef.current.duration) return;
            const rect = (e.target as HTMLCanvasElement).getBoundingClientRect();
            const ratio = (e.clientX - rect.left) / rect.width;
            audioRef.current.currentTime = ratio * audioRef.current.duration;
          }}
        />
        <span className={`text-xs ${textColor} opacity-80`}>{formatDuration(currentMs)}</span>
      </div>
    </div>
  );
};
