import { useState } from 'react';
import { Avatar, Dropdown, Button } from 'antd';
import { UserOutlined, MoreOutlined, EditOutlined, DeleteOutlined } from '@ant-design/icons';
import type { MenuProps } from 'antd';
import { format } from 'date-fns';
import { IMessageResponse, MessageAttachmentType } from '../../../../types/messaging';
import { useAppDispatch } from '../../../../store/hooks';
import { setReplyingToMessage, setEditingMessage } from '../../../../store/uiSlice';
import { getMostImportantRoleName } from '../../../../types/auth';
import { useAuth } from '../../../../context/useAuth';
import { getFirstName, getFullName } from '../../mocks/fakeData';
import { CustomMessageAttachment } from './CustomMessageAttachment';
import { StickerAttachment } from './attachments/StickerAttachment';
import { isSystemMessage } from '../../../../types/utilities/messageUtilities';

interface MessageItemProps {
  message: IMessageResponse;
  showAvatar?: boolean;
}

const MessageItem: React.FC<MessageItemProps> = ({ message, showAvatar = true }) => {
  const dispatch = useAppDispatch();
  const [isHovered, setIsHovered] = useState(false);
  const [contextMenuOpen, setContextMenuOpen] = useState(false); // ✅ для ПКМ меню

  const { user } = useAuth();
  
  const isMyMessage = message.createdBy.id === user?.id;
  const isCustomMessageAttachment = isSystemMessage(message);
  const stickerAttachment = message.attachments.find(
    (a) => a.type === MessageAttachmentType.Sticker
  );

  const formatTime = (dateString: string) => {
    return format(new Date(dateString), 'HH:mm');
  };

  const handleReply = () => {
    dispatch(setReplyingToMessage({ chatId: message.chatId, messageId: message.id }));
  };

  const handleEdit = () => {
    dispatch(setEditingMessage({ chatId: message.chatId, messageId: message.id }));
  };

  const handleDelete = () => {
    // TODO: implement delete
    console.log('Delete message:', message.id);
  };

  // ✅ Обработчик двойного клика
  const handleDoubleClick = () => {
    handleReply();
  };

  // ✅ Обработчик правой кнопки мыши
  const handleContextMenu = (e: React.MouseEvent) => {
    e.preventDefault(); // Отключаем стандартное контекстное меню
    setContextMenuOpen(true);
  };

  const menuItems: MenuProps['items'] = [
    {
      key: 'reply',
      label: 'Ответить',
      icon: <MoreOutlined />,
      onClick: () => {
        handleReply();
        setContextMenuOpen(false); // ✅ закрываем меню
      },
    },
    ...(isMyMessage
      ? [
          {
            key: 'edit',
            label: 'Редактировать',
            icon: <EditOutlined />,
            onClick: () => {
              handleEdit();
              setContextMenuOpen(false);
            },
          },
          {
            key: 'delete',
            label: 'Удалить',
            icon: <DeleteOutlined />,
            danger: true,
            onClick: () => {
              handleDelete();
              setContextMenuOpen(false);
            },
          },
        ]
      : []),
  ];

  if (isCustomMessageAttachment) {
    return <CustomMessageAttachment message={message} />;
  }

  if (stickerAttachment) {
    return (
      <div className={`flex gap-3 ${isMyMessage ? 'flex-row-reverse' : 'flex-row'}`}>
        <div className="flex-shrink-0">
          {showAvatar ? (
            <Avatar
              size={36}
              src={`https://ui-avatars.com/api/?name=${message.createdBy.name[0]}${message.createdBy.surname[0]}&background=random`}
              icon={<UserOutlined />}
            />
          ) : (
            <div className="w-9" />
          )}
        </div>
        <div className={`flex flex-col ${isMyMessage ? 'items-end' : 'items-start'}`}>
          {!isMyMessage && showAvatar && (
            <div className="text-sm font-semibold text-gray-700 mb-1 px-1">
              {getFullName(message.createdBy)}
            </div>
          )}
          <StickerAttachment message={message} attachment={stickerAttachment} />
          <div className="text-xs text-gray-400 mt-0.5 px-1">
            {formatTime(message.createdAt)}
          </div>
        </div>
      </div>
    );
  }

  return (
    <Dropdown
      menu={{ items: menuItems }}
      trigger={['contextMenu']} // ✅ открывается по ПКМ
      open={contextMenuOpen}
      onOpenChange={setContextMenuOpen}
    >
      <div
        className={`flex gap-3 ${isMyMessage ? 'flex-row-reverse' : 'flex-row'}`}
        onMouseEnter={() => setIsHovered(true)}
        onMouseLeave={() => setIsHovered(false)}
        onDoubleClick={handleDoubleClick}
        onContextMenu={handleContextMenu}
      >
        {/* Avatar */}
        <div className="flex-shrink-0">
          {showAvatar ? (
            <Avatar
              size={36}
              src={`https://ui-avatars.com/api/?name=${message.createdBy.name[0]}${message.createdBy.surname[0]}&background=random`}
              icon={<UserOutlined />}
            />
          ) : (
            <div className="w-9" />
          )}
        </div>

        {/* Message content */}
        <div className={`flex-1 max-w-2xl ${isMyMessage ? 'items-end' : 'items-start'} flex flex-col`}>
          {/* Author name (only for others' messages) */}
          {!isMyMessage && showAvatar && (
            <div className="flex items-center gap-2 mb-1 px-3">
              <span className="text-sm font-semibold text-gray-700">
                {getFullName(message.createdBy)}
              </span>
              {message.createdBy.roleNames.length > 0 && (
                <span className="text-xs text-gray-500">
                  {getMostImportantRoleName(message.createdBy.roleNames)}
                </span>
              )}
            </div>
          )}

          <div className="relative group">
            {/* Reply preview */}
            {message.replyMessage && (
              <div
                className={`mb-1 px-3 py-2 border-l-2 text-sm cursor-pointer hover:opacity-80 ${
                  isMyMessage
                    ? 'bg-blue-100 border-blue-400'
                    : 'bg-gray-200 border-gray-400'
                } rounded`}
                onClick={() => {
                  // TODO: прокрутить к сообщению на которое отвечают
                  console.log('Scroll to message:', message.replyMessage?.id);
                }}
              >
                <div className="font-semibold text-xs text-gray-600">
                  {getFirstName(message.replyMessage.createdBy)}
                </div>
                <div className="text-gray-700 truncate">
                  {message.replyMessage.messageText}
                </div>
              </div>
            )}

            {/* Message bubble */}
            <div
              className={`relative px-4 py-2 rounded-2xl select-text ${
                isMyMessage
                  ? 'bg-blue-500 text-white'
                  : 'bg-white text-gray-900 border border-gray-200'
              }`}
            >
              {/* Message text */}
              <div className="break-words whitespace-pre-wrap">
                {message.messageText}
              </div>

              {/* Time */}
              <div
                className={`text-xs mt-1 ${
                  isMyMessage ? 'text-blue-100' : 'text-gray-500'
                }`}
              >
                {formatTime(message.createdAt)}
                {message.updatedAt !== message.createdAt && (
                  <span className="ml-1">(изменено)</span>
                )}
              </div>

              {/* Actions dropdown (при hover) */}
              {isHovered && (
                <div
                  className={`absolute top-0 ${
                    isMyMessage ? 'left-0 -translate-x-10' : 'right-0 translate-x-10'
                  }`}
                >
                  <Dropdown menu={{ items: menuItems }} trigger={['click']}>
                    <Button
                      type="text"
                      size="small"
                      icon={<MoreOutlined />}
                      className="bg-white shadow-md hover:bg-gray-100"
                      onClick={(e) => e.stopPropagation()}
                    />
                  </Dropdown>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </Dropdown>
  );
};

export default MessageItem;
