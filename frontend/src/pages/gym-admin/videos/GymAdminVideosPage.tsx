import React, { useEffect, useState } from 'react';
import { Badge, Card, Col, Modal, Pagination, Row, Spin, Tag, Tooltip, Typography } from 'antd';
import { PlayCircleOutlined } from '@ant-design/icons';
import { toast } from 'react-toastify';
import { useApiService } from '../../../api/useApiService';
import { useVideoApi } from '../../../api/videoApi';
import { VideoPlayer } from '../../../components/VideoPlayer/VideoPlayer';
import { IVideoResponse, IVideoResolutionUrlResponse, VideoStatus } from '../../../types/videos';

const { Text } = Typography;

const STATUS_COLORS: Record<VideoStatus, 'default' | 'processing' | 'success' | 'error'> = {
  Pending: 'default',
  Processing: 'processing',
  Ready: 'success',
  Failed: 'error',
};

const STATUS_LABELS: Record<VideoStatus, string> = {
  Pending: 'Ожидает',
  Processing: 'Обработка',
  Ready: 'Готово',
  Failed: 'Ошибка',
};

const formatDuration = (seconds: number): string => {
  const m = Math.floor(seconds / 60);
  const s = String(seconds % 60).padStart(2, '0');
  return `${m}:${s}`;
};

const PAGE_SIZE = 12;

export const GymAdminVideosPage: React.FC = () => {
  const apiService = useApiService();
  const videoApi = useVideoApi(apiService);

  const [videos, setVideos] = useState<IVideoResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);

  const [playerOpen, setPlayerOpen] = useState(false);
  const [playerTitle, setPlayerTitle] = useState('');
  const [playerResolutions, setPlayerResolutions] = useState<IVideoResolutionUrlResponse[]>([]);
  const [playerPoster, setPlayerPoster] = useState<string | null>(null);
  const [playerLoading, setPlayerLoading] = useState(false);

  const fetchVideos = async (p = page) => {
    setLoading(true);
    try {
      const res = await videoApi.getAll(p, PAGE_SIZE);
      if (res.success && res.data) {
        setVideos(res.data.items);
        setTotal(res.data.totalItems ?? 0);
      } else {
        toast.error(res.error?.detail ?? 'Ошибка загрузки видео');
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchVideos(); }, []);

  const handlePlay = async (video: IVideoResponse) => {
    if (video.status !== 'Ready') return;
    setPlayerTitle(video.title);
    setPlayerPoster(video.posterUrl);
    setPlayerResolutions([]);
    setPlayerLoading(true);
    setPlayerOpen(true);
    try {
      const res = await videoApi.getResolutions(video.id);
      if (res.success && res.data) {
        setPlayerResolutions(res.data.items);
      } else {
        toast.error('Не удалось загрузить ссылки на видео');
        setPlayerOpen(false);
      }
    } finally {
      setPlayerLoading(false);
    }
  };

  const handlePageChange = (p: number) => {
    setPage(p);
    fetchVideos(p);
  };

  const handleClosePlayer = () => {
    setPlayerOpen(false);
    setPlayerResolutions([]);
  };

  return (
    <div className="p-6">
      <Card title="Видео" className="shadow-sm" loading={loading}>
        {videos.length === 0 && !loading && (
          <div className="text-center py-12 text-gray-400">Видео не найдены</div>
        )}
        <Row gutter={[16, 16]}>
          {videos.map((video) => {
            const isReady = video.status === 'Ready';
            return (
              <Col key={video.id} xs={24} sm={12} md={8} lg={6}>
                <Card
                  hoverable={isReady}
                  cover={
                    video.posterUrl ? (
                      <div className="relative overflow-hidden" style={{ paddingBottom: '56.25%' }}>
                        <img
                          src={video.posterUrl}
                          alt={video.title}
                          className="absolute inset-0 w-full h-full object-cover"
                        />
                        {isReady && (
                          <div className="absolute inset-0 flex items-center justify-center bg-black/30 opacity-0 hover:opacity-100 transition-opacity">
                            <PlayCircleOutlined className="text-white text-5xl" />
                          </div>
                        )}
                      </div>
                    ) : (
                      <div
                        className="flex items-center justify-center bg-gray-100 text-gray-400"
                        style={{ height: 140 }}
                      >
                        <PlayCircleOutlined style={{ fontSize: 40 }} />
                      </div>
                    )
                  }
                  bodyStyle={{ padding: '12px' }}
                  onClick={() => handlePlay(video)}
                >
                  <div className="flex flex-col gap-1">
                    <Tooltip title={video.title}>
                      <Text strong className="truncate block">{video.title}</Text>
                    </Tooltip>
                    <div className="flex items-center justify-between">
                      <Badge status={STATUS_COLORS[video.status]} text={STATUS_LABELS[video.status]} />
                    </div>
                    {isReady && (
                      <div className="flex items-center justify-between mt-1">
                        {video.durationSeconds != null && (
                          <Text type="secondary" className="text-xs">
                            {formatDuration(video.durationSeconds)}
                          </Text>
                        )}
                        <div className="flex gap-1 flex-wrap">
                          {video.resolutions.map((r) => (
                            <Tag key={r.qualityLabel} color="blue" className="text-xs m-0">
                              {r.qualityLabel}p
                            </Tag>
                          ))}
                        </div>
                      </div>
                    )}
                  </div>
                </Card>
              </Col>
            );
          })}
        </Row>
        {total > PAGE_SIZE && (
          <div className="flex justify-center mt-6">
            <Pagination
              current={page}
              pageSize={PAGE_SIZE}
              total={total}
              onChange={handlePageChange}
              showTotal={(t) => `Всего ${t}`}
            />
          </div>
        )}
      </Card>

      <Modal
        title={playerTitle}
        open={playerOpen}
        onCancel={handleClosePlayer}
        footer={null}
        width={860}
        destroyOnClose
      >
        {playerLoading ? (
          <div className="flex justify-center py-12">
            <Spin size="large" />
          </div>
        ) : (
          <VideoPlayer resolutions={playerResolutions} posterUrl={playerPoster} />
        )}
      </Modal>
    </div>
  );
};
