import React from 'react';
import { IMessageResponse, MessageAttachmentType } from '../../../../types/messaging';
import { CreateGroupAttachment } from './attachments/CreateGroupAttachment';
import { StickerAttachment } from './attachments/StickerAttachment';
import { DocumentAttachmentPreview } from './attachments/DocumentAttachmentPreview';

interface CustomMessageAttachmentProps {
  message: IMessageResponse;
  isMyMessage?: boolean;
}

export const CustomMessageAttachment: React.FC<CustomMessageAttachmentProps> = ({ message, isMyMessage }) => {
  // Берем первый attachment (системное сообщение имеет только один всегда)
  const attachment = message.attachments[0];

  if (!attachment) {
    return null;
  }

  // Роутинг на конкретный компонент в зависимости от типа
  switch (attachment.type) {
    case MessageAttachmentType.CreateGroup:
      return <CreateGroupAttachment message={message} attachment={attachment} />;

    case MessageAttachmentType.Sticker:
      return <StickerAttachment message={message} attachment={attachment} />;

    case MessageAttachmentType.Document:
      return <DocumentAttachmentPreview attachment={attachment} isMyMessage={isMyMessage} />;

    default:
      return (
        <div className="text-center py-2 px-4 bg-gray-100 rounded text-sm text-gray-600">
          Не поддерживается вашей версией
        </div>
      );
  }
};
