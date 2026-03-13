import React from 'react';
import { Button, Progress } from 'antd';
import { CheckCircleOutlined, CloseOutlined, ExclamationCircleOutlined, LoadingOutlined, VideoCameraOutlined } from '@ant-design/icons';
import { useVideoUpload } from '../../context/VideoUploadContext';

export const VideoUploadProgress: React.FC = () => {
  const { uploadState, dismissUpload } = useVideoUpload();
  const { status, title, fileName, progress } = uploadState;

  if (status === 'idle') return null;

  const canDismiss = status === 'done' || status === 'failed';

  return (
    <div
      style={{
        position: 'fixed',
        bottom: 24,
        right: 24,
        width: 320,
        zIndex: 1050,
        borderRadius: 8,
        boxShadow: '0 4px 16px rgba(0,0,0,0.18)',
        background: '#fff',
        padding: '14px 16px',
        display: 'flex',
        flexDirection: 'column',
        gap: 8,
      }}
    >
      <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
        <VideoCameraOutlined style={{ fontSize: 18, color: '#3b82f6' }} />
        <div style={{ flex: 1, minWidth: 0 }}>
          <div style={{ fontWeight: 600, fontSize: 13, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
            {title ?? fileName ?? 'Видео'}
          </div>
          <div style={{ fontSize: 11, color: '#6b7280', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
            {fileName}
          </div>
        </div>
        {canDismiss && (
          <Button type="text" size="small" icon={<CloseOutlined />} onClick={dismissUpload} />
        )}
      </div>

      {(status === 'uploading' || status === 'completing') && (
        <div>
          <Progress
            percent={progress}
            size="small"
            status="active"
            format={(pct) => (status === 'completing' ? 'Завершение…' : `${pct}%`)}
          />
          <div style={{ fontSize: 11, color: '#6b7280', marginTop: 2 }}>
            {status === 'uploading' ? 'Загрузка частями…' : 'Сохранение в облаке…'}
            <LoadingOutlined style={{ marginLeft: 4 }} />
          </div>
        </div>
      )}

      {status === 'done' && (
        <div style={{ display: 'flex', alignItems: 'center', gap: 6, color: '#16a34a', fontSize: 13 }}>
          <CheckCircleOutlined />
          Загружено, поставлено в очередь на обработку
        </div>
      )}

      {status === 'failed' && (
        <div style={{ color: '#dc2626', fontSize: 12 }}>
          <ExclamationCircleOutlined style={{ marginRight: 4 }} />
          {uploadState.error ?? 'Ошибка загрузки'}
        </div>
      )}
    </div>
  );
};
