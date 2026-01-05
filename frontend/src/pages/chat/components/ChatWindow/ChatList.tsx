import React, { useEffect, useState } from 'react';
import { Input, Spin, Empty } from 'antd';
import { SearchOutlined } from '@ant-design/icons';
import InfiniteScroll from 'react-infinite-scroll-component';
import ChatListItem from './ChatListItem';
import ChatListSkeleton from './ChatListSkeleton';
import { useAppDispatch, useAppSelector } from '../../../../store/hooks';
import { selectChats, selectChatsLoading, selectHasMoreChats } from '../../../../store/selectors';
import { setChats, setLoading } from '../../../../store/chatSlice';
import { fakeChatList } from '../../mocks/fakeData';

const ChatList: React.FC = () => {
  const dispatch = useAppDispatch();
  const chats = useAppSelector(selectChats);
  const loading = useAppSelector(selectChatsLoading);
  const hasMore = useAppSelector(selectHasMoreChats);
  
  const [searchQuery, setSearchQuery] = useState('');

  // Load chats on mount
  useEffect(() => {
    loadChats();
  }, []);

  const loadChats = async () => {
    dispatch(setLoading(true));
    
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    // Load fake chats
    dispatch(setChats(fakeChatList));
  };

  const loadMoreChats = async () => {
    // TODO: implement pagination
    console.log('Load more chats');
  };

  // Фильтрация чатов по поиску
  const filteredChats = chats.filter((chat) => {
    const chatName = chat.chat.type === 'Group'
      ? chat.chat.participants.map((p) => `${p.user.name} ${p.user.surname}`).join(' ')
      : chat.chat.participants.find((p) => p.user.id !== 'current-user')?.user.name || '';
    
    return chatName.toLowerCase().includes(searchQuery.toLowerCase());
  });

  if (loading && chats.length === 0) {
    return <ChatListSkeleton />;
  }

  return (
    <div className="flex flex-col h-full bg-white">
      {/* Header with search */}
      <div className="p-4 border-b border-gray-200">
        <h1 className="text-2xl font-bold mb-4">Чаты</h1>
        
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
    </div>
  );
};

export default ChatList;
