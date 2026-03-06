import React from 'react';
import { IMessageResponse, MessageAttachmentType } from '../../../../types/messaging';
import { CreateGroupAttachment } from './attachments/CreateGroupAttachment';
import { StickerAttachment } from './attachments/StickerAttachment';

interface CustomMessageAttachmentProps {
  message: IMessageResponse;
}

export const CustomMessageAttachment: React.FC<CustomMessageAttachmentProps> = ({ message }) => {
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

    // TODO: доделать
    // case MessageAttachmentType.InviteUser:
    //   return <AddUserAttachment message={message} attachment={attachment} />;

    // case MessageAttachmentType.ExcludeUser:
    //   return <RemoveUserAttachment message={message} attachment={attachment} />;

    // case MessageAttachmentType.RenameGroup:
    //   return <RenameGroupAttachment message={message} attachment={attachment} />;

    // case MessageAttachmentType.LeaveGroup:
    //   return <LeaveGroupAttachment message={message} attachment={attachment} />;

    default:
      return (
        <div className="text-center py-2 px-4 bg-gray-100 rounded text-sm text-gray-600">
          Не поддерживается вашей версией
        </div>
      );
  }
};
