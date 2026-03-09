import React, { useEffect, useRef, useState } from 'react';
import {
  Badge,
  Button,
  Card,
  Form as AntForm,
  Input,
  Modal,
  Popconfirm,
  Progress,
  Space,
  Table,
  Tag,
  Tooltip,
} from 'antd';
import {
  DeleteOutlined,
  PlayCircleOutlined,
  PlusOutlined,
  ReloadOutlined,
} from '@ant-design/icons';
import { toast } from 'react-toastify';
import { Controller, useForm } from 'react-hook-form';
import { useApiService } from '../../../api/useApiService';
import { useVideoApi } from '../../../api/videoApi';
import { VideoPlayer } from '../../../components/VideoPlayer/VideoPlayer';
import { IVideoResponse, IVideoResolutionUrlResponse } from '../../../types/videos';

interface UploadFormValues {
  title: string;
}

const STATUS_COLORS: Record<string, string> = {
  Pending: 'default',
  Processing: 'processing',
  Ready: 'success',
  Failed: 'error',
};

const STATUS_LABELS: Record<string, string> = {
  Pending: 'Ожидает',
  Processing: 'Обработка',
  Ready: 'Готово',
  Failed: 'Ошибка',
};

export const VideosAdminPage: React.FC = () => {
  const apiService = useApiService();
  const videoApi = useVideoApi(apiService);

  const [videos, setVideos] = useState<IVideoResponse[]>([]);
  const [loading, setLoading] = useState(false);

  // Upload state
  const [uploadOpen, setUploadOpen] = useState(false);
  const [uploadLoading, setUploadLoading] = useState(false);
  const [uploadProgress, setUploadProgress] = useState(0);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const uploadForm = useForm<UploadFormValues>({ defaultValues: { title: '' } });

  // Player modal state
  const [playerOpen, setPlayerOpen] = useState(false);
  const [playerResolutions, setPlayerResolutions] = useState<IVideoResolutionUrlResponse[]>([]);
  const [playerPoster, setPlayerPoster] = useState<string | null>(null);
  const [playerLoading, setPlayerLoading] = useState(false);

  const fetchVideos = async () => {
    setLoading(true);
    try {
      const res = await videoApi.getAll();
      if (res.success && res.data) setVideos(res.data.items);
      else toast.error(res.error?.detail ?? 'Ошибка загрузки видео');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchVideos(); }, []);

  // Auto-refresh while any video is Processing
  useEffect(() => {
    const hasProcessing = videos.some((v) => v.status === 'Pending' || v.status === 'Processing');
    if (!hasProcessing) return;
    const timer = setInterval(fetchVideos, 8000);
    return () => clearInterval(timer);
  }, [videos]);

  const handleUpload = async (values: UploadFormValues) => {
    if (!selectedFile) {
      toast.error('Выберите файл');
      return;
    }

    const ext = selectedFile.name.split('.').pop() ?? 'mp4';
    setUploadLoading(true);
    setUploadProgress(0);

    try {
      // 1. Init upload → get presigned URL
      const initRes = await videoApi.initUpload(values.title.trim(), ext);
      if (!initRes.success || !initRes.data) throw new Error(initRes.error?.detail);

      const { videoId, presignedPutUrl } = initRes.data;

      // 2. PUT file directly to MinIO
      await videoApi.uploadToS3(presignedPutUrl, selectedFile, setUploadProgress);

      // 3. Confirm → trigger encoding
      await videoApi.confirmUpload(videoId);

      toast.success('Видео загружено и поставлено в очередь на обработку');
      setUploadOpen(false);
      uploadForm.reset();
      setSelectedFile(null);
      await fetchVideos();
    } catch (err: unknown) {
      const msg = err instanceof Error ? err.message : 'Ошибка загрузки';
      toast.error(msg);
    } finally {
      setUploadLoading(false);
      setUploadProgress(0);
    }
  };

  const handleOpenPlayer = async (video: IVideoResponse) => {
    if (video.status !== 'Ready') return;
    setPlayerLoading(true);
    setPlayerOpen(true);
    try {
      const res = await videoApi.getResolutions(video.id);
      if (res.success && res.data) {
        setPlayerResolutions(res.data.items);
        setPlayerPoster(video.posterUrl);
      } else {
        toast.error('Не удалось загрузить ссылки на видео');
      }
    } finally {
      setPlayerLoading(false);
    }
  };

  const handleDelete = async (video: IVideoResponse) => {
    const res = await videoApi.delete(video.id);
    if (res.success) {
      toast.success('Видео удалено');
      await fetchVideos();
    } else {
      toast.error(res.error?.detail ?? 'Ошибка удаления');
    }
  };

  const columns = [
    {
      title: 'Название',
      dataIndex: 'title',
      key: 'title',
    },
    {
      title: 'Статус',
      key: 'status',
      render: (_: unknown, v: IVideoResponse) => (
        <Badge
          status={STATUS_COLORS[v.status] as 'default' | 'processing' | 'success' | 'error'}
          text={STATUS_LABELS[v.status] ?? v.status}
        />
      ),
    },
    {
      title: 'Длительность',
      key: 'duration',
      render: (_: unknown, v: IVideoResponse) =>
        v.durationSeconds
          ? `${Math.floor(v.durationSeconds / 60)}:${String(v.durationSeconds % 60).padStart(2, '0')}`
          : '—',
    },
    {
      title: 'Разрешения',
      key: 'resolutions',
      render: (_: unknown, v: IVideoResponse) =>
        v.resolutions.length > 0 ? (
          <Space size={4}>
            {v.resolutions.map((r) => (
              <Tag key={r.qualityLabel} color="blue">{r.qualityLabel}p</Tag>
            ))}
          </Space>
        ) : '—',
    },
    {
      title: 'Дата',
      key: 'createdAt',
      render: (_: unknown, v: IVideoResponse) =>
        new Date(v.createdAt).toLocaleDateString('ru-RU'),
    },
    {
      title: 'Действия',
      key: 'actions',
      render: (_: unknown, v: IVideoResponse) => (
        <Space>
          <Tooltip title={v.status === 'Ready' ? 'Воспроизвести' : 'Видео ещё не готово'}>
            <Button
              type="text"
              icon={<PlayCircleOutlined />}
              disabled={v.status !== 'Ready'}
              onClick={() => handleOpenPlayer(v)}
            />
          </Tooltip>
          <Popconfirm
            title="Удалить видео и все разрешения?"
            okText="Удалить"
            cancelText="Отмена"
            okButtonProps={{ danger: true }}
            onConfirm={() => handleDelete(v)}
          >
            <Button type="text" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div className="p-6">
      <Card
        title="Видео"
        className="shadow-sm"
        extra={
          <Space>
            <Button icon={<ReloadOutlined />} onClick={fetchVideos}>Обновить</Button>
            <Button type="primary" icon={<PlusOutlined />} onClick={() => setUploadOpen(true)}>
              Загрузить видео
            </Button>
          </Space>
        }
      >
        <Table
          columns={columns}
          dataSource={videos}
          rowKey="id"
          loading={loading}
          pagination={{ pageSize: 20 }}
          locale={{ emptyText: 'Видео не загружены' }}
        />
      </Card>

      {/* Upload modal */}
      <Modal
        title="Загрузить видео"
        open={uploadOpen}
        onCancel={() => { setUploadOpen(false); uploadForm.reset(); setSelectedFile(null); }}
        onOk={uploadForm.handleSubmit(handleUpload)}
        confirmLoading={uploadLoading}
        okText="Загрузить"
        cancelText="Отмена"
        okButtonProps={{ disabled: !selectedFile }}
        destroyOnClose
      >
        <AntForm layout="vertical">
          <AntForm.Item
            label="Название"
            validateStatus={uploadForm.formState.errors.title ? 'error' : ''}
            help={uploadForm.formState.errors.title?.message}
          >
            <Controller
              name="title"
              control={uploadForm.control}
              rules={{ required: 'Введите название' }}
              render={({ field }) => <Input {...field} placeholder="Например: Разминка" autoFocus />}
            />
          </AntForm.Item>

          <AntForm.Item label="Файл">
            <input
              ref={fileInputRef}
              type="file"
              accept="video/*"
              className="hidden"
              onChange={(e) => setSelectedFile(e.target.files?.[0] ?? null)}
            />
            <div className="flex items-center gap-3">
              <Button onClick={() => fileInputRef.current?.click()}>
                {selectedFile ? 'Заменить файл' : 'Выбрать файл'}
              </Button>
              {selectedFile && (
                <span className="text-sm text-gray-600 truncate max-w-[200px]">
                  {selectedFile.name}
                </span>
              )}
            </div>
            {uploadLoading && uploadProgress > 0 && (
              <Progress percent={uploadProgress} className="mt-2" />
            )}
          </AntForm.Item>
        </AntForm>
      </Modal>

      {/* Player modal */}
      <Modal
        title="Воспроизведение"
        open={playerOpen}
        onCancel={() => { setPlayerOpen(false); setPlayerResolutions([]); }}
        footer={null}
        width={860}
        destroyOnClose
      >
        {playerLoading ? (
          <div className="flex justify-center py-12 text-gray-400">Загрузка...</div>
        ) : (
          <VideoPlayer resolutions={playerResolutions} posterUrl={playerPoster} />
        )}
      </Modal>
    </div>
  );
};
