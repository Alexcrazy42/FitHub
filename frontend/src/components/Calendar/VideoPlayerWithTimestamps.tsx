import React, { useRef, useState } from 'react';
import { Button, List, Card } from 'antd';
import { VideoPlayerProps } from '../../types/videoTypes';

const VideoPlayerWithTimestamps: React.FC<VideoPlayerProps> = ({ videoUrl, timestamps }) => {
  const videoRef = useRef<HTMLVideoElement>(null);
  const [currentTime, setCurrentTime] = useState(0);

  // Переход к указанному таймкоду
  const jumpToTimestamp = (seconds: number) => {
    if (videoRef.current) {
      videoRef.current.currentTime = seconds;
      videoRef.current.play();
    }
  };

  // Форматирование времени (00:00)
  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
  };

  // Обновление текущего времени видео
  const handleTimeUpdate = () => {
    if (videoRef.current) {
      setCurrentTime(videoRef.current.currentTime);
    }
  };

  return (
    <Card title="Видео с таймкодами" style={{ width: '100%' }}>
      <div style={{ display: 'flex', gap: '20px' }}>
        {/* Видеоплеер */}
        <div style={{ flex: 2 }}>
          <video
            ref={videoRef}
            src={videoUrl}
            controls
            style={{ width: '100%', borderRadius: '8px' }}
            onTimeUpdate={handleTimeUpdate}
          />
          <div style={{ marginTop: '10px' }}>
            Текущее время: {formatTime(currentTime)}
          </div>
        </div>

        {/* Список таймкодов */}
        <div style={{ flex: 1 }}>
          <List
            header={<div>Таймкоды</div>}
            bordered
            dataSource={timestamps}
            renderItem={(item) => (
              <List.Item
                onClick={() => jumpToTimestamp(item.time)}
                style={{
                  cursor: 'pointer',
                  backgroundColor: currentTime >= item.time ? '#f0f0f0' : 'transparent'
                }}
              >
                <Button type="text">
                  {formatTime(item.time)}
                </Button>
                <span>{item.title}</span>
              </List.Item>
            )}
          />
        </div>
      </div>
    </Card>
  );
};

export default VideoPlayerWithTimestamps;