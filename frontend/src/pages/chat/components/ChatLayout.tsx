import React from 'react';
import { Layout } from 'antd';
import ChatList from './ChatWindow/ChatList';
import ChatWindow from './ChatWindow/ChatWindow';
import { useAppSelector } from '../../../store/hooks';
import { selectCurrentChatId, selectSidebarCollapsed } from '../../../store/selectors'

const { Sider, Content } = Layout;

export const ChatLayout: React.FC = () => {
  const currentChatId = useAppSelector(selectCurrentChatId);
  const sidebarCollapsed = useAppSelector(selectSidebarCollapsed);

  return (
    <Layout style={{ height: '90vh' }}>
      <Sider
        width={360}
        theme="light"
        collapsedWidth={0}
        collapsed={sidebarCollapsed}
        breakpoint="lg"
        className="border-r border-gray-200"
        style={{ 
          height: '100%', 
          overflow: 'hidden',
          display: 'flex',
          flexDirection: 'column'
        }}
      >
        <ChatList />
      </Sider>

      <Content 
        className="bg-gray-50"
        style={{ 
          height: '100%', 
          overflow: 'hidden',
          display: 'flex',
          flexDirection: 'column'
        }}
      >
        {currentChatId ? (
          <ChatWindow chatId={currentChatId} />
        ) : (
          <div className="flex items-center justify-center h-full">
            <div className="text-center text-gray-400">
              <div className="text-6xl mb-4">💬</div>
              <p className="text-xl">Выберите чат для начала общения</p>
            </div>
          </div>
        )}
      </Content>
    </Layout>
  );
};