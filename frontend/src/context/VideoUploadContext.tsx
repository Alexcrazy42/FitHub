import React, { createContext, useCallback, useContext, useRef, useState } from 'react';
import { useApiService } from '../api/useApiService';
import { useVideoApi } from '../api/videoApi';
import { IMultipartPartUrl } from '../types/videos';

const CHUNK_SIZE = 10 * 1024 * 1024; // 10 MB
const CONCURRENCY = 3;

export type VideoUploadStatus = 'idle' | 'uploading' | 'completing' | 'done' | 'failed';

export interface VideoUploadState {
  status: VideoUploadStatus;
  videoId: string | null;
  title: string | null;
  fileName: string | null;
  progress: number;
  error: string | null;
}

const initialState: VideoUploadState = {
  status: 'idle',
  videoId: null,
  title: null,
  fileName: null,
  progress: 0,
  error: null,
};

interface VideoUploadContextValue {
  uploadState: VideoUploadState;
  startUpload: (file: File, title: string) => Promise<void>;
  dismissUpload: () => void;
}

const VideoUploadContext = createContext<VideoUploadContextValue | null>(null);

export const useVideoUpload = () => {
  const ctx = useContext(VideoUploadContext);
  if (!ctx) throw new Error('useVideoUpload must be used inside VideoUploadProvider');
  return ctx;
};

async function uploadPartsWithConcurrency(
  parts: IMultipartPartUrl[],
  file: File,
  uploadPart: (url: string, chunk: Blob) => Promise<{ headers: Record<string, string> }>,
  onPartDone: (count: number, total: number) => void,
): Promise<{ partNumber: number; eTag: string }[]> {
  const completed: { partNumber: number; eTag: string }[] = [];
  let doneCount = 0;

  const uploadOne = async (part: IMultipartPartUrl) => {
    const start = (part.partNumber - 1) * CHUNK_SIZE;
    const chunk = file.slice(start, start + CHUNK_SIZE);
    const res = await uploadPart(part.url, chunk);
    const rawEtag = (res.headers['etag'] as string | undefined) ?? '';
    completed.push({ partNumber: part.partNumber, eTag: rawEtag });
    doneCount++;
    onPartDone(doneCount, parts.length);
  };

  for (let i = 0; i < parts.length; i += CONCURRENCY) {
    await Promise.all(parts.slice(i, i + CONCURRENCY).map(uploadOne));
  }

  return completed;
}

export const VideoUploadProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const apiService = useApiService();
  const videoApi = useVideoApi(apiService);
  const [uploadState, setUploadState] = useState<VideoUploadState>(initialState);
  const abortRef = useRef(false);

  const startUpload = useCallback(async (file: File, title: string) => {
    abortRef.current = false;
    setUploadState({
      status: 'uploading',
      videoId: null,
      title,
      fileName: file.name,
      progress: 0,
      error: null,
    });

    try {
      const ext = file.name.split('.').pop() ?? 'mp4';
      const initRes = await videoApi.initMultipartUpload(title.trim(), ext, file.size);
      if (!initRes.success || !initRes.data) {
        throw new Error(initRes.error?.detail ?? 'Ошибка инициализации загрузки');
      }

      const { videoId, parts } = initRes.data;
      setUploadState((s) => ({ ...s, videoId }));

      const completedParts = await uploadPartsWithConcurrency(
        parts,
        file,
        (url, chunk) => videoApi.uploadPart(url, chunk) as Promise<{ headers: Record<string, string> }>,
        (done, total) => {
          if (!abortRef.current) {
            setUploadState((s) => ({ ...s, progress: Math.round((done / total) * 90) }));
          }
        },
      );

      if (abortRef.current) return;

      setUploadState((s) => ({ ...s, status: 'completing', progress: 95 }));

      completedParts.sort((a, b) => a.partNumber - b.partNumber);
      const completeRes = await videoApi.completeMultipart(videoId, completedParts);
      if (!completeRes.success) {
        throw new Error(completeRes.error?.detail ?? 'Ошибка завершения загрузки');
      }

      setUploadState((s) => ({ ...s, status: 'done', progress: 100 }));
    } catch (err: unknown) {
      if (abortRef.current) return;
      const msg = err instanceof Error ? err.message : 'Ошибка загрузки';
      setUploadState((s) => ({ ...s, status: 'failed', error: msg }));
    }
  }, [videoApi]);

  const dismissUpload = useCallback(() => {
    abortRef.current = true;
    setUploadState(initialState);
  }, []);

  return (
    <VideoUploadContext.Provider value={{ uploadState, startUpload, dismissUpload }}>
      {children}
    </VideoUploadContext.Provider>
  );
};
