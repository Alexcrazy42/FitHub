import { useState } from 'react';
import { Modal, Button, Avatar } from 'antd';
import { UserOutlined, MessageOutlined, ProfileOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import { UserResponse, roleMapping } from '../../../../types/auth';
import { ChatType, IChatMessageResponse } from '../../../../types/messaging';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import { setCurrentChatId } from '../../../../store/uiSlice';
import { addChat } from '../../../../store/chatSlice';
import { selectChats } from '../../../../store/selectors';
import { useApiService } from '../../../../api/useApiService';
import { useChatService } from '../../../../api/services/chatService';
import { useAuth } from '../../../../context/useAuth';

interface UserProfileModalProps {
  user: UserResponse | null;
  onClose: () => void;
}

export const UserProfileModal: React.FC<UserProfileModalProps> = ({ user, onClose }) => {
  const dispatch = useAppDispatch();
  const chats = useAppSelector(selectChats);
  const { user: currentUser } = useAuth();
  const apiService = useApiService();
  const chatService = useChatService(apiService);
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleWriteMessage = async () => {
    if (!user || !currentUser) return;

    // Check if OneToOne chat already exists with this user
    const existing = chats.find(
      (c) =>
        c.chat.type === ChatType.OneToOne &&
        c.chat.participants.some((p) => p.user.id === user.id)
    );

    if (existing) {
      dispatch(setCurrentChatId(existing.chat.id));
      onClose();
      return;
    }

    setLoading(true);
    try {
      const response = await chatService.createChat({
        type: ChatType.OneToOne,
        participantUserIds: [user.id, currentUser.id!],
      });

      if (response.success && response.data) {
        const newChat = response.data;
        const chatEntry: IChatMessageResponse = {
          id: newChat.id,
          chat: newChat,
          lastMessage: null!,
          unreadCount: 0,
          lastMessageTime: '',
        };
        dispatch(addChat(chatEntry));
        dispatch(setCurrentChatId(newChat.id));
        onClose();
      }
    } catch (err) {
      console.error('Failed to create chat:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal open={!!user} onCancel={onClose} footer={null} width={320} centered>
      {user && (
        <div className="flex flex-col items-center gap-4 py-4">
          <Avatar
            size={80}
            src={`https://ui-avatars.com/api/?name=${user.name[0]}${user.surname[0]}&background=random&size=80`}
            icon={<UserOutlined />}
          />
          <div className="text-center">
            <h3 className="text-lg font-semibold">
              {user.surname} {user.name}
            </h3>
            {user.roleNames.length > 0 && (
              <span className="text-sm text-gray-500">
                {roleMapping[user.roleNames[0]] ?? user.roleNames[0]}
              </span>
            )}
          </div>
          <Button
            icon={<ProfileOutlined />}
            onClick={() => { navigate(`/profile/${user.id}`); onClose(); }}
            block
          >
            Открыть профиль
          </Button>
          {currentUser?.id !== user.id && (
            <Button
              type="primary"
              icon={<MessageOutlined />}
              onClick={handleWriteMessage}
              loading={loading}
              block
            >
              Написать сообщение
            </Button>
          )}
        </div>
      )}
    </Modal>
  );
};
