import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { Avatar, Button, Spin } from 'antd';
import { UserOutlined, MessageOutlined } from '@ant-design/icons';
import { UserResponse, roleMapping } from '../../types/auth';
import { ChatType, IChatMessageResponse } from '../../types/messaging';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import { setCurrentChatId } from '../../store/uiSlice';
import { addChat } from '../../store/chatSlice';
import { selectChats } from '../../store/selectors';
import { useApiService } from '../../api/useApiService';
import { useUserService } from '../../api/services/userService';
import { useChatService } from '../../api/services/chatService';
import { useAuth } from '../../context/useAuth';
import { useNavigate } from 'react-router-dom';

export const UserProfilePage: React.FC = () => {
  const { userId } = useParams<{ userId: string }>();
  const dispatch = useAppDispatch();
  const chats = useAppSelector(selectChats);
  const { user: currentUser } = useAuth();
  const navigate = useNavigate();

  const apiService = useApiService();
  const userService = useUserService(apiService);
  const chatService = useChatService(apiService);

  const [profile, setProfile] = useState<UserResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [chatLoading, setChatLoading] = useState(false);

  useEffect(() => {
    if (!userId) return;
    const fetchUser = async () => {
      setLoading(true);
      try {
        const response = await userService.getUser(userId);
        if (response.success && response.data) {
          setProfile(response.data);
        }
      } finally {
        setLoading(false);
      }
    };
    fetchUser();
  }, [userId]);

  const handleWriteMessage = async () => {
    if (!profile || !currentUser) return;

    const existing = chats.find(
      (c) =>
        c.chat.type === ChatType.OneToOne &&
        c.chat.participants.some((p) => p.user.id === profile.id)
    );

    if (existing) {
      dispatch(setCurrentChatId(existing.chat.id));
      navigate(-1);
      return;
    }

    setChatLoading(true);
    try {
      const response = await chatService.createChat({
        type: ChatType.OneToOne,
        participantUserIds: [profile.id!, currentUser.id!],
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
        navigate(-1);
      }
    } finally {
      setChatLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Spin size="large" />
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="flex items-center justify-center h-64 text-gray-400">
        Пользователь не найден
      </div>
    );
  }

  const isOwnProfile = currentUser?.id === profile.id;

  return (
    <div className="max-w-lg mx-auto mt-10 p-8 bg-white rounded-2xl shadow-md">
      <div className="flex flex-col items-center gap-4">
        <Avatar
          size={96}
          src={`https://ui-avatars.com/api/?name=${profile.name[0]}${profile.surname[0]}&background=random&size=96`}
          icon={<UserOutlined />}
        />
        <div className="text-center">
          <h2 className="text-2xl font-bold">
            {profile.surname} {profile.name}
          </h2>
          {profile.roleNames.length > 0 && (
            <p className="text-gray-500 mt-1">
              {roleMapping[profile.roleNames[0]] ?? profile.roleNames[0]}
            </p>
          )}
          <p className="text-sm text-gray-400 mt-1">{profile.email}</p>
        </div>

        {!isOwnProfile && (
          <Button
            type="primary"
            icon={<MessageOutlined />}
            onClick={handleWriteMessage}
            loading={chatLoading}
            size="large"
          >
            Написать сообщение
          </Button>
        )}
      </div>
    </div>
  );
};

export default UserProfilePage;
