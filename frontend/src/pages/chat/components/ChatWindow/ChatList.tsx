// components/ChatList.tsx
import React, { useEffect, useState } from 'react';
import { Input, Spin, Empty, Button, Modal, Select, Form, message as antMessage } from 'antd';
import { SearchOutlined, PlusOutlined, UserAddOutlined } from '@ant-design/icons';
import InfiniteScroll from 'react-infinite-scroll-component';
import ChatListItem from './ChatListItem';
import ChatListSkeleton from './ChatListSkeleton';
import CreateChatModal from './CreateChatModal';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import { selectChats, selectChatsLoading, selectHasMoreChats } from '../../../../store/selectors';
import { addChats, setLoading } from '../../../../store/chatSlice';
import { useApiService } from "../../../../api/useApiService";
import { useMessageService } from "../../../../api/services/messageService";
import { useChatService } from "../../../../api/services/chatService";
import { ICreateChatRequest, IChatMessageResponse } from "../../../../types/messaging";
import { useAuth } from '../../../../context/useAuth';
import { toast } from 'react-toastify';

const ChatList: React.FC = () => {
  const apiService = useApiService();
  const messageService = useMessageService(apiService);
  const chatService = useChatService(apiService);

  const {user} = useAuth();

  const [page, setPage] = useState(1);
  const [error, setError] = useState<string | null>(null);
  const dispatch = useAppDispatch();
  const chats = useAppSelector(selectChats);
  const loading = useAppSelector(selectChatsLoading);
  const hasMore = useAppSelector(selectHasMoreChats);
  
  const [searchQuery, setSearchQuery] = useState('');
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    loadChats();
  }, []);

  // TODO: раз в какое то время fallback на получение read-models на отображение чатов

  const loadChats = async () => {
    dispatch(setLoading(true));

    try {
      const response = await messageService.getChatMessagesList(page, 20);

      if(response.success && response.data) {
        dispatch(addChats({
          chats: response.data.items,
          hasMore: page < (response.data?.totalPages ?? 0)
        }));
      } else {
        setError(response.error?.title || 'Failed to load chats');
      }
    } catch {
      setError('Network error');
    } finally {
      dispatch(setLoading(false));
    }
  };

  const loadMoreChats = async () => {
    console.log('Load more chats');
  };

  const handleCreateChat = async (values: ICreateChatRequest) => {
    setCreating(true);
    
    try {
      const response = await chatService.createChat(values);
      
      if (response.success && response.data) {
        toast.success('Чат успешно создан!');
        
        await loadChats();
        setIsCreateModalOpen(false);
      } else {
        toast.error(response.error?.title || 'Не удалось создать чат');
      }
    } catch (err) {
      console.error('Error creating chat:', err);
      toast.error('Ошибка сети');
    } finally {
      setCreating(false);
    }
  };

  // Фильтрация чатов по поиску
  const filteredChats = chats.filter((chat) => {
    const chatName = chat.chat.type === 'Group'
      ? chat.chat.name ?? ""
      : chat.chat.participants.find((p) => p.user.id !== user?.id)?.user.name || '';
    
    return chatName.toLowerCase().includes(searchQuery.toLowerCase());
  });

  if (loading && chats.length === 0) {
    return <ChatListSkeleton />;
  }

  if (error && chats.length === 0) {
    return <div>{error}</div>;
  }

  return (
    <div className="flex flex-col h-full bg-white">
      {/* Header with search */}
      <div className="p-4 border-b border-gray-200">
        <div className="flex items-center justify-between mb-4">
          <h1 className="text-2xl font-bold">Чаты</h1>
          
          {/* Create chat button */}

          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={() => setIsCreateModalOpen(true)}
            size="large"
          >
            Создать группу
          </Button>
        </div>
        
        {/* Search input */}
        <Input
          placeholder="Поиск чатов..."
          prefix={<SearchOutlined className="text-gray-400" />}
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="rounded-lg"
          size="large"
          allowClear
        />
      </div>

      {/* Chat list with infinite scroll */}
      <div id="chat-list-scrollable" className="flex-1 overflow-y-auto">
        {filteredChats.length === 0 ? (
          <Empty
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            description={searchQuery ? 'Чатов не найдено' : 'У вас пока нет чатов'}
            className="mt-20"
          />
        ) : (
          <InfiniteScroll
            dataLength={filteredChats.length}
            next={loadMoreChats}
            hasMore={hasMore && searchQuery === ''}
            loader={
              <div className="text-center py-4">
                <Spin />
              </div>
            }
            scrollableTarget="chat-list-scrollable"
            endMessage={
              filteredChats.length > 10 && (
                <div className="text-center py-4 text-gray-400 text-sm">
                  Все чаты загружены
                </div>
              )
            }
          >
            {filteredChats.map((chat) => (
              <ChatListItem key={chat.id} chat={chat} />
            ))}
          </InfiniteScroll>
        )}
      </div>
      
      {/* Create Chat Modal */}
      <CreateChatModal
        open={isCreateModalOpen}
        onCancel={() => setIsCreateModalOpen(false)}
        onSubmit={handleCreateChat}
        loading={creating}
      />
    </div>
  );
};

export default ChatList;
