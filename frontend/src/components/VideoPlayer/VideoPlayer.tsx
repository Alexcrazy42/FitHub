import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Select, Spin } from 'antd';
import { IVideoResolutionUrlResponse } from '../../types/videos';

interface Props {
  resolutions: IVideoResolutionUrlResponse[];
  posterUrl?: string | null;
  durationSeconds?: number | null;
}

type QualityKey = 'auto' | string; // 'auto' | '360' | '720' | '1080'

// Bandwidth thresholds to pick initial quality automatically (bits/sec)
const AUTO_QUALITY_THRESHOLDS: { minBps: number; label: number }[] = [
  { minBps: 4_000_000, label: 1080 },
  { minBps: 1_500_000, label: 720 },
  { minBps: 0, label: 360 },
];

async function estimateBandwidthBps(url: string): Promise<number> {
  const PROBE_BYTES = 200_000; // 200 KB
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 5000);
  try {
    const t0 = performance.now();
    const res = await fetch(url, {
      headers: { Range: `bytes=0-${PROBE_BYTES - 1}` },
      signal: controller.signal,
    });
    const buf = await res.arrayBuffer();
    const elapsed = (performance.now() - t0) / 1000; // seconds
    return (buf.byteLength * 8) / elapsed; // bps
  } catch {
    return 0;
  } finally {
    clearTimeout(timeout);
  }
}

export const VideoPlayer: React.FC<Props> = ({ resolutions, posterUrl, durationSeconds }) => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const [selectedQuality, setSelectedQuality] = useState<QualityKey>('auto');
  const [resolvedLabel, setResolvedLabel] = useState<number | null>(null);
  const [isSwitching, setIsSwitching] = useState(false);

  const sortedRes = [...resolutions].sort((a, b) => b.qualityLabel - a.qualityLabel);

  const findResolution = useCallback(
    (label: number) => sortedRes.find((r) => r.qualityLabel === label),
    [sortedRes]
  );

  // Pick the best auto quality based on bandwidth
  const pickAutoResolution = useCallback(async (): Promise<IVideoResolutionUrlResponse | null> => {
    if (sortedRes.length === 0) return null;
    // Probe with the lowest quality (smallest file) to estimate bandwidth
    const probe = sortedRes[sortedRes.length - 1];
    const bps = await estimateBandwidthBps(probe.url);

    for (const threshold of AUTO_QUALITY_THRESHOLDS) {
      if (bps >= threshold.minBps) {
        const res = findResolution(threshold.label);
        if (res) return res;
      }
    }
    return sortedRes[sortedRes.length - 1]; // fallback to lowest
  }, [sortedRes, findResolution]);

  // Switch video source while preserving playback position
  const switchTo = useCallback(async (res: IVideoResolutionUrlResponse) => {
    const video = videoRef.current;
    if (!video) return;

    setIsSwitching(true);
    const savedTime = video.currentTime;
    const wasPlaying = !video.paused;

    video.pause();
    video.src = res.url;
    video.load();

    await new Promise<void>((resolve) => {
      const onCanPlay = () => {
        video.removeEventListener('canplay', onCanPlay);
        resolve();
      };
      video.addEventListener('canplay', onCanPlay);
    });

    video.currentTime = savedTime;
    if (wasPlaying) await video.play().catch(() => {});
    setResolvedLabel(res.qualityLabel);
    setIsSwitching(false);
  }, []);

  // Initial load: pick resolution by bandwidth
  useEffect(() => {
    if (sortedRes.length === 0) return;
    let cancelled = false;

    const init = async () => {
      const res = await pickAutoResolution();
      if (!res || cancelled) return;

      const video = videoRef.current;
      if (!video) return;
      video.src = res.url;
      setResolvedLabel(res.qualityLabel);
    };

    init();
    return () => { cancelled = true; };
  }, [sortedRes]); // eslint-disable-line react-hooks/exhaustive-deps

  // Buffer watcher: auto-upgrade quality when buffering is healthy
  useEffect(() => {
    if (selectedQuality !== 'auto') return;
    const video = videoRef.current;
    if (!video) return;

    const check = setInterval(async () => {
      if (video.paused || isSwitching || !resolvedLabel) return;
      const buffered = video.buffered;
      if (buffered.length === 0) return;
      const ahead = buffered.end(buffered.length - 1) - video.currentTime;

      // Upgrade if buffer > 15 s and we're not at highest quality
      if (ahead > 15 && resolvedLabel < sortedRes[0].qualityLabel) {
        const higherRes = sortedRes.find((r) => r.qualityLabel > resolvedLabel);
        if (higherRes) await switchTo(higherRes);
      }
      // Downgrade if buffer < 3 s and we're not at lowest quality
      if (ahead < 3 && resolvedLabel > sortedRes[sortedRes.length - 1].qualityLabel) {
        const lowerRes = [...sortedRes].reverse().find((r) => r.qualityLabel < resolvedLabel);
        if (lowerRes) await switchTo(lowerRes);
      }
    }, 4000);

    return () => clearInterval(check);
  }, [selectedQuality, resolvedLabel, sortedRes, isSwitching, switchTo]);

  const handleQualityChange = async (value: QualityKey) => {
    setSelectedQuality(value);
    if (value === 'auto') {
      const res = await pickAutoResolution();
      if (res) await switchTo(res);
    } else {
      const res = findResolution(Number(value));
      if (res) await switchTo(res);
    }
  };

  const qualityOptions = [
    { label: 'Авто', value: 'auto' },
    ...sortedRes.map((r) => ({ label: `${r.qualityLabel}p`, value: String(r.qualityLabel) })),
  ];

  const displayLabel = selectedQuality === 'auto'
    ? `Авто (${resolvedLabel ?? '…'}p)`
    : `${selectedQuality}p`;

  return (
    <div className="relative w-full bg-black rounded-xl overflow-hidden group">
      <video
        ref={videoRef}
        controls
        poster={posterUrl ?? undefined}
        className="w-full max-h-[70vh] object-contain"
        preload="metadata"
      />

      {/* Quality switcher — visible on hover or always on mobile */}
      <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity z-10">
        <div className="flex items-center gap-1 bg-black/70 rounded-lg px-2 py-1">
          {isSwitching && <Spin size="small" className="text-white" />}
          <Select
            value={selectedQuality}
            onChange={handleQualityChange}
            options={qualityOptions}
            size="small"
            style={{ width: 120 }}
            className="[&_.ant-select-selector]:!bg-transparent [&_.ant-select-selector]:!border-0 [&_.ant-select-selection-item]:!text-white"
            placeholder={displayLabel}
            dropdownStyle={{ minWidth: 100 }}
          />
        </div>
      </div>

      {sortedRes.length === 0 && (
        <div className="absolute inset-0 flex items-center justify-center text-white/60 text-sm">
          Видео ещё обрабатывается...
        </div>
      )}
    </div>
  );
};
