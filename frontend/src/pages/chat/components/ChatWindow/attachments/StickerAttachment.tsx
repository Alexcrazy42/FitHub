import React from 'react';
import { Tooltip } from 'antd';
import { IMessageAttachmentResponse, IMessageResponse } from '../../../../../types/messaging';
import { getFileRoute } from '../../../../../api/files';

interface StickerAttachmentData {
  stickerId: string;
  fileId: string;
  name: string;
}

interface StickerAttachmentProps {
  message: IMessageResponse;
  attachment: IMessageAttachmentResponse;
}

export const StickerAttachment: React.FC<StickerAttachmentProps> = ({ attachment }) => {
  const data: StickerAttachmentData = JSON.parse(attachment.data || '{}');

  return (
    <Tooltip title={data.name}>
      <img
        src={getFileRoute(data.fileId)}
        alt={data.name}
        className="w-32 h-32 object-contain select-none"
        draggable={false}
      />
    </Tooltip>
  );
};
