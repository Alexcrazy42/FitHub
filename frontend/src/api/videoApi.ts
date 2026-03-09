import axios from 'axios';
import { ApiService } from './ApiService';
import {
  IInitVideoUploadResponse,
  IVideoResolutionUrlResponse,
  IVideoResponse,
} from '../types/videos';

const BASE = '/v1/videos';

export const useVideoApi = (api: ApiService) => ({
  getAll: () => api.get<{ items: IVideoResponse[] }>(BASE),

  getById: (id: string) => api.get<IVideoResponse>(`${BASE}/${id}`),

  getResolutions: (id: string) =>
    api.get<{ items: IVideoResolutionUrlResponse[] }>(`${BASE}/${id}/resolutions`),

  initUpload: (title: string, fileExtension: string) =>
    api.post<IInitVideoUploadResponse>(`${BASE}/init-upload`, { title, fileExtension }),

  uploadToS3: (presignedUrl: string, file: File, onProgress?: (pct: number) => void) =>
    axios.put(presignedUrl, file, {
      headers: { 'Content-Type': file.type },
      onUploadProgress: (e) => {
        if (onProgress && e.total) onProgress(Math.round((e.loaded / e.total) * 100));
      },
    }),

  confirmUpload: (id: string) => api.post(`${BASE}/${id}/confirm-upload`, {}),

  delete: (id: string) => api.delete(`${BASE}/${id}`),
});
