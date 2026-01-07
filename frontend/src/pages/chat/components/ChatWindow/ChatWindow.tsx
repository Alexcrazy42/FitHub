import React, { useEffect } from 'react';
import { Spin } from 'antd';
import ChatHeader from './ChatHeader';
import { MessageList } from './MessageList';
import MessageInput from './MessageInput';
import TypingIndicator from './TypingIndicator';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import {
  selectCurrentChat,
  selectAllChatMessages,
  selectMessagesLoading,
  selectTypingUsers,
} from '../../../../store/selectors';
import { setMessages, setMessagesLoading } from '../../../../store/messagesSlice';
import { fakeMessages } from '../../mocks/fakeData';

interface ChatWindowProps {
  chatId: string;
}

const ChatWindow: React.FC<ChatWindowProps> = ({ chatId }) => {
  const dispatch = useAppDispatch();
  const currentChat = useAppSelector(selectCurrentChat);
  const messages = useAppSelector(selectAllChatMessages(chatId));
  const loading = useAppSelector(selectMessagesLoading(chatId));
  const typingUsers = useAppSelector(selectTypingUsers(chatId));

  useEffect(() => {
    loadMessages();
  }, [chatId]);

  const loadMessages = async () => {
    // TODO: сделать апи запрос

    dispatch(setMessagesLoading({ chatId, loading: true }));
    await new Promise(resolve => setTimeout(resolve, 500));
    const chatMessages = fakeMessages[chatId] || [];
    dispatch(setMessages({
      chatId,
      messages: chatMessages,
      hasMore: false,
      nextCursor: undefined,
    }));
  };

  if (!currentChat) {
    return (
      <div className="flex items-center justify-center h-full bg-gray-50">
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div className="flex flex-col h-full bg-white">  {/* ← h-full */}
      {/* Header - FIXED */}
      <ChatHeader chat={currentChat} />

      {/* Messages - SCROLLABLE */}
      <div className="flex-1 overflow-hidden bg-gray-50">  {/* ← flex-1 overflow-hidden */}
        {loading && messages.length === 0 ? (
          <div className="flex items-center justify-center h-full">
            <Spin size="large" />
          </div>
        ) : (
          <MessageList chatId={chatId} messages={messages} />
        )}
      </div>

      {/* Typing indicator - FIXED */}
      {typingUsers.length > 0 && <TypingIndicator users={typingUsers} />}

      {/* Input - FIXED */}
      <MessageInput chatId={chatId} />
    </div>
  );
};

export default ChatWindow;
