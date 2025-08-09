export interface VideoTimestamp {
  id: string;
  time: number; // В секундах
  title: string;
}

export interface VideoPlayerProps {
  videoUrl: string;
  timestamps: VideoTimestamp[];
}