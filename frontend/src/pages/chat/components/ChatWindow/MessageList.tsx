// src/features/chat/components/ChatWindow/MessageList.tsx

import React, { useEffect, useRef } from 'react';
import { Spin } from 'antd';
import MessageItem from './MessageItem';
import { IMessageResponse } from '../../../../types/messaging';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import { selectHasMoreMessages, selectMessagesLoading } from '../../../../store/selectors';

interface MessageListProps {
  chatId: string;
  messages: IMessageResponse[];
}

export const MessageList: React.FC<MessageListProps> = ({ chatId, messages }) => {
  const dispatch = useAppDispatch();
  const hasMore = useAppSelector(selectHasMoreMessages(chatId));
  const loading = useAppSelector(selectMessagesLoading(chatId));
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const scrollContainerRef = useRef<HTMLDivElement>(null);

  // Scroll to bottom on new message
  useEffect(() => {
    scrollToBottom();
  }, [messages.length]);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  const loadMoreMessages = async () => {
    // TODO: Load older messages (scroll up pagination)
    console.log('Load more messages');
  };

  if (messages.length === 0 && !loading) {
    return (
      <div className="flex items-center justify-center h-full text-gray-400">
        <div className="text-center">
          <div className="text-5xl mb-4">📭</div>
          <p className="text-lg">Пока нет сообщений</p>
          <p className="text-sm">Начните беседу!</p>
        </div>
      </div>
    );
  }

  return (
    <div
      ref={scrollContainerRef}
      id={`message-list-${chatId}`}
      className="h-full overflow-y-auto bg-gray-50 flex flex-col-reverse"
    >
      <div className="px-4 py-4 space-y-3">
        {messages.map((message, index) => {
          const prevMessage = index > 0 ? messages[index - 1] : null;
          const showAvatar =
            !prevMessage || prevMessage.createdBy.id !== message.createdBy.id;

          return (
            <MessageItem
              key={message.id}
              message={message}
              showAvatar={showAvatar}
            />
          );
        })}
        <div ref={messagesEndRef} />
      </div>

      {/* Loader for pagination */}
      {loading && (
        <div className="text-center py-4">
          <Spin />
        </div>
      )}
    </div>
  );
};
