import axios from 'axios';
import { ApiService } from './ApiService';
import {
  IInitVideoMultipartUploadResponse,
  IInitVideoUploadResponse,
  IVideoResolutionUrlResponse,
  IVideoResponse,
} from '../types/videos';
import { ListResponse } from '../types/common';

const BASE = '/v1/videos';

export const useVideoApi = (api: ApiService) => ({
  getAll: (pageNumber = 1, pageSize = 20) =>
    api.get<ListResponse<IVideoResponse>>(`${BASE}?pageNumber=${pageNumber}&pageSize=${pageSize}`),

  getById: (id: string) => api.get<IVideoResponse>(`${BASE}/${id}`),

  getResolutions: (id: string) =>
    api.get<{ items: IVideoResolutionUrlResponse[] }>(`${BASE}/${id}/resolutions`),

  initUpload: (title: string, fileExtension: string) =>
    api.post<IInitVideoUploadResponse>(`${BASE}/init-upload`, { title, fileExtension }),

  // not use
  uploadToS3: (presignedUrl: string, file: File, onProgress?: (pct: number) => void) =>
    axios.put(presignedUrl, file, {
      headers: { 'Content-Type': file.type },
      onUploadProgress: (e) => {
        if (onProgress && e.total) onProgress(Math.round((e.loaded / e.total) * 100));
      },
    }),

  initMultipartUpload: (title: string, fileExtension: string, fileSizeBytes: number) =>
    api.post<IInitVideoMultipartUploadResponse>(`${BASE}/init-multipart-upload`, { title, fileExtension, fileSizeBytes }),

  completeMultipart: (id: string, parts: { partNumber: number; eTag: string }[]) =>
    api.post(`${BASE}/${id}/complete-multipart`, { parts }),

  uploadPart: (url: string, chunk: Blob) =>
    axios.put(url, chunk, { headers: { 'Content-Type': 'application/octet-stream' } }),

  delete: (id: string) => api.delete(`${BASE}/${id}`),
});
