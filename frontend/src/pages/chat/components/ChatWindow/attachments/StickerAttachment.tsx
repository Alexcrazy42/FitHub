import React from 'react';
import { Tooltip } from 'antd';
import { IMessageAttachmentResponse, IMessageResponse } from '../../../../../types/messaging';
import { getFileRoute } from '../../../../../api/files';

interface StickerAttachmentData {
  name?: string;
  fileId: {
    value?: string;
  };
  stickerId: {
    value?: string;
  };
}

interface StickerAttachmentProps {
  message: IMessageResponse;
  attachment: IMessageAttachmentResponse;
}

export const StickerAttachment: React.FC<StickerAttachmentProps> = ({ attachment }) => {
  let data: StickerAttachmentData;
  
  try {
    data = JSON.parse(attachment.data || '{}');
  } catch {
    console.warn('Invalid sticker data:', attachment.data);
    return null; // Или fallback UI
  }

  // Безопасный доступ к fileId.value
  const fileUrl = data.fileId?.value ? getFileRoute(data.fileId.value) : '';
  const stickerName = data.name || 'Sticker';

  if (!fileUrl) {
    return null; // Не рендерим если нет URL
  }

  return (
    <Tooltip title={stickerName}>
      <img
        src={fileUrl}
        alt={stickerName}
        className="w-32 h-32 object-contain select-none"
        draggable={false}
        onError={(e) => {
          console.warn('Sticker load failed:', fileUrl);
          (e.target as HTMLImageElement).style.display = 'none';
        }}
      />
    </Tooltip>
  );
};
