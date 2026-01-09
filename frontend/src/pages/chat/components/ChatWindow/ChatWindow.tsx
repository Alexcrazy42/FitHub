import { Spin } from 'antd';
import ChatHeader from './ChatHeader';
import { MessageList } from './MessageList';
import MessageInput from './MessageInput';
import TypingIndicator from './TypingIndicator';
import { useAppSelector } from '../../../../store/hooks';
import {
  selectCurrentChat,
  selectAllChatMessages,
  selectMessagesLoading,
  selectTypingUsers,
} from '../../../../store/selectors';

interface ChatWindowProps {
  chatId: string;
}

const ChatWindow: React.FC<ChatWindowProps> = ({ chatId }) => {
  const currentChat = useAppSelector(selectCurrentChat);
  const messages = useAppSelector((state) => selectAllChatMessages(state, chatId));
  const loading = useAppSelector((state) => selectMessagesLoading(state, chatId));
  const typingUsers = useAppSelector((state) => selectTypingUsers(state, chatId));

  if (!currentChat) {
    return (
      <div className="flex items-center justify-center h-full bg-gray-50">
        <Spin size="large" />
      </div>
    );
  }

  return (
    <div className="flex flex-col h-full bg-white">  {/* ✅ h-full вместо h-screen */}
      {/* Header - FIXED */}
      <div className="flex-shrink-0">
        <ChatHeader chat={currentChat} />
      </div>

      {/* Messages - SCROLLABLE - займет все свободное пространство */}
      <div className="flex-1 overflow-hidden bg-gray-50 min-h-0">  {/* ✅ min-h-0 важно! */}
        {loading && messages.length === 0 ? (
          <div className="flex items-center justify-center h-full">
            <Spin size="large" />
          </div>
        ) : (
          <MessageList chatId={chatId} />
        )}
      </div>

      {/* Typing indicator - FIXED */}
      {typingUsers.length > 0 && (
        <div className="flex-shrink-0">
          <TypingIndicator users={typingUsers} />
        </div>
      )}

      {/* Input - FIXED - всегда внизу */}
      <div className="flex-shrink-0">
        <MessageInput chatId={chatId} />
      </div>
    </div>
  );
};

export default ChatWindow;
