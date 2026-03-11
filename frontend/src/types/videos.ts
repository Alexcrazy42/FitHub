export type VideoStatus = 'Pending' | 'Processing' | 'Ready' | 'Failed';

export interface IVideoResolutionResponse {
  quality: string;       // 'Q360p' | 'Q720p' | 'Q1080p'
  qualityLabel: number;  // 360 | 720 | 1080
  widthPx: number;
  heightPx: number;
  bitrateKbps: number;
  fileSizeBytes: number;
}

export interface IVideoResponse {
  id: string;
  title: string;
  status: VideoStatus;
  durationSeconds: number | null;
  posterUrl: string | null;
  failureReason: string | null;
  createdAt: string;
  resolutions: IVideoResolutionResponse[];
}

export interface IInitVideoUploadResponse {
  videoId: string;
  presignedPutUrl: string;
}

export interface IMultipartPartUrl {
  partNumber: number;
  url: string;
}

export interface IInitVideoMultipartUploadResponse {
  videoId: string;
  parts: IMultipartPartUrl[];
}

export interface IVideoResolutionUrlResponse {
  quality: string;
  qualityLabel: number;
  widthPx: number;
  heightPx: number;
  bitrateKbps: number;
  url: string;
}
