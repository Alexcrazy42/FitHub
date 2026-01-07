// components/ChatList.tsx
import React, { useEffect, useState } from 'react';
import { Input, Spin, Empty, Button, message as antMessage } from 'antd';
import { SearchOutlined, PlusOutlined } from '@ant-design/icons';
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
import { ChatType, ICreateChatRequest } from "../../../../types/messaging";
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
    fetchChats();

    const interval = setInterval(() => {
      fetchChats();
    }, 15000); // 15 секунд

    return () => clearInterval(interval);
  }, []);

  const fetchChats = async (requestPage = null) => {
    dispatch(setLoading(true));

    const currentPage = requestPage ?? page;

    const response = await messageService.getChatMessagesList(currentPage, 20);

    if (response.success && response.data) {
      const action = {
        chats: response.data.items,
        hasMore: currentPage < (response.data?.totalPages ?? 0)
      };
      dispatch(addChats(action));
      
      if (action.hasMore) {
        setPage((page) => page + 1);
      }
    } else {
      setError(response.error?.title || 'Failed to load chats');
    }
    dispatch(setLoading(false));
  }

  const loadMoreChats = async () => {
    if(hasMore) {
      await fetchChats();
    }
  };

  const handleCreateChat = async (values: ICreateChatRequest) => {
    setCreating(true);
    
    try {
      const response = await chatService.createChat(values);
      
      if (response.success && response.data) {
        toast.success('Чат успешно создан!');
        
        await fetchChats();
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
    const chatName = chat.chat.type === ChatType.Group
      ? chat.chat.name ?? ""
      : chat.chat.participants.find((p) => p.user.id !== user?.id)?.user.name || '';
    
    return chatName.toLowerCase().includes(searchQuery.toLowerCase());
  });

  if (loading && chats.length === 0) {
    return <ChatListSkeleton />;
  }

  if (error && chats.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center h-64 text-center px-4">
        <svg className="w-16 h-16 text-red-500 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          Не удалось загрузить чаты
        </h3>
        <p className="text-sm text-gray-600 mb-4">
          Произошла ошибка при загрузке. Попробуйте обновить страницу.
        </p>
        <button 
          onClick={() => fetchChats()} 
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
        >
          Обновить
        </button>
      </div>
    );
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
