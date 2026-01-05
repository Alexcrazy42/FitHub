import React from 'react';
import { Avatar, Badge, Tag } from 'antd';
import { UserOutlined, TeamOutlined, CheckOutlined } from '@ant-design/icons';
import { format, isToday, isYesterday } from 'date-fns';
import { ru } from 'date-fns/locale';
import { IChatMessageResponse } from '../../../../types/messaging';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import { setCurrentChatId } from '../../../../store/uiSlice';
import { resetUnreadCount } from '../../../../store/chatSlice';
import { selectCurrentChatId } from '../../../../store/selectors';
import { getChatName, getChatAvatar, currentUser, getFirstName } from '../../mocks/fakeData';
import { roleMapping } from '../../../../types/auth';
import { useAuth } from '../../../../context/useAuth';

interface ChatListItemProps {
  chat: IChatMessageResponse;
}

const ChatListItem: React.FC<ChatListItemProps> = ({ chat }) => {
  const {user} = useAuth();
  
  const dispatch = useAppDispatch();
  const currentChatId = useAppSelector(selectCurrentChatId);
  
  const isActive = currentChatId === chat.id;

  // Форматирование времени
  const formatTime = (dateString: string) => {
    const date = new Date(dateString);
    
    if (isToday(date)) {
      return format(date, 'HH:mm');
    } else if (isYesterday(date)) {
      return 'Вчера';
    } else if (date.getFullYear() === new Date().getFullYear()) {
      return format(date, 'd MMM', { locale: ru });
    } else {
      return format(date, 'dd.MM.yyyy');
    }
  };

  const handleClick = () => {
    dispatch(setCurrentChatId(chat.id));
    
    if (chat.unreadCount > 0) {
      // TODO: надо это вызывать когда дошел до конца в чате или батчами (потихоньку когда скроллишь вниз)
      // TODO: сделать апи запрос
      dispatch(resetUnreadCount(chat.id));
    }
  };

  // Форматируем последнее сообщение
  const formatLastMessage = () => {
    if (!chat.lastMessage) return 'Нет сообщений';
    
    const isMyMessage = chat.lastMessage.createdBy.id === user?.id;
    const authorName = isMyMessage ? 'Вы' : getFirstName(chat.lastMessage.createdBy);
    
    // Ограничиваем длину сообщения
    const maxLength = 40;

    // TODO: переделать на attachment если есть
    const text = chat.lastMessage.messageText;
    const truncatedText = text.length > maxLength 
      ? `${text.substring(0, maxLength)}...` 
      : text;
    
    return (
      <>
        <span className={isMyMessage ? 'text-gray-500' : ''}>
          {authorName}:{' '}
        </span>
        {truncatedText}
      </>
    );
  };

  const getOtherUserRole = () => {
    if (chat.chat.type === 'OneToOne') {
      const otherUser = chat.chat.participants.find((p) => p.user.id !== currentUser.id);
      if (otherUser && otherUser.user.roleNames.length > 0) {
        return roleMapping[otherUser.user.roleNames[0]];
      }
    }
    return null;
  };

  const otherUserRole = getOtherUserRole();

  return (
    <div
      onClick={handleClick}
      className={`
        relative flex items-center gap-3 p-4 cursor-pointer transition-all duration-200
        ${isActive 
          ? 'bg-blue-50 border-r-4 border-blue-500' 
          : 'hover:bg-gray-50 border-r-4 border-transparent'
        }
      `}
    >
      {/* Avatar */}
      <div className="relative flex-shrink-0">
        {chat.chat.type === 'Group' ? (
          <Avatar
            size={48}
            icon={<TeamOutlined />}
            className="bg-purple-500"
          />
        ) : (
          <Avatar
            size={48}
            src={getChatAvatar(chat)}
            icon={<UserOutlined />}
            className="bg-blue-500"
          />
        )}
        
        {/* Online indicator (можно добавить позже) */}
        {/* <span className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></span> */}
      </div>

      {/* Chat info */}
      <div className="flex-1 min-w-0">
        {/* Name, role badge, and time */}
        <div className="flex items-center justify-between mb-1">
          <div className="flex items-center gap-2 min-w-0 flex-1">
            <h3 
              className={`font-semibold truncate ${
                isActive ? 'text-blue-600' : 'text-gray-900'
              }`}
            >
              {chat.chat.name}
            </h3>
            {/* Role badge (только для OneToOne) */}
            {otherUserRole && (
              <Tag 
                color="blue" 
                className="text-xs m-0 flex-shrink-0 hidden sm:inline-block"
              >
                {otherUserRole}
              </Tag>
            )}
          </div>
          {/* Time */}
          {chat.lastMessageTime && (
            <span className="text-xs text-gray-500 ml-2 flex-shrink-0">
              {formatTime(chat.lastMessageTime)}
            </span>
          )}
        </div>

        {/* Last message and unread badge */}
        <div className="flex items-center justify-between gap-2">
          <p
            className={`text-sm truncate flex-1 ${
              chat.unreadCount > 0 
                ? 'font-semibold text-gray-900' 
                : 'text-gray-600'
            }`}
          >
            {formatLastMessage()}
          </p>

          {/* Unread badge */}
          {chat.unreadCount > 0 && (
            <Badge
              count={chat.unreadCount}
              className="flex-shrink-0"
              style={{ backgroundColor: '#1890ff' }}
              overflowCount={99}
            />
          )}
          
          {/* Read indicator (двойная галочка для прочитанных) */}
          {chat.unreadCount === 0 && chat.lastMessage?.createdBy.id === currentUser.id && (
            <CheckOutlined className="text-blue-500 text-xs flex-shrink-0" />
          )}
        </div>
      </div>
    </div>
  );
};

export default ChatListItem;
