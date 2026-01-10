import { Avatar, Button, Dropdown, Tag } from 'antd';
import { 
  UserOutlined, 
  MoreOutlined, 
  ArrowLeftOutlined,
  InfoCircleOutlined,
  SearchOutlined,
  BellOutlined,
  LogoutOutlined
} from '@ant-design/icons';
import type { MenuProps } from 'antd';
import { ChatType, IChatMessageResponse } from '../../../../types/messaging';
import { useAppDispatch } from '../../../../store/hooks';
import { setCurrentChatId } from '../../../../store/uiSlice';
import { getChatName, getChatAvatar, currentUser } from '../../mocks/fakeData';
import { roleMapping } from '../../../../types/auth';

interface ChatHeaderProps {
  chat: IChatMessageResponse;
}

const ChatHeader: React.FC<ChatHeaderProps> = ({ chat }) => {
  const dispatch = useAppDispatch();

  // Получаем роль собеседника (для OneToOne)
  const getOtherUserRole = () => {
    if (chat.chat.type === 'OneToOne') {
      const otherUser = chat.chat.participants.find((p) => p.user.id !== currentUser.id);
      if (otherUser && otherUser.user.roleNames.length > 0) {
        return roleMapping[otherUser.user.roleNames[0]];
      }
    }
    return null;
  };

  const isOnline = false; // TODO: implement real online status

  // Меню действий
  const menuItems: MenuProps['items'] = [
    {
      key: 'info',
      label: 'Информация о чате',
      icon: <InfoCircleOutlined />,
    },
    {
      type: 'divider',
    },
    {
      key: 'leave',
      label: 'Покинуть чат',
      icon: <LogoutOutlined />,
      danger: true,
    },
  ];

  const handleMenuClick: MenuProps['onClick'] = ({ key }) => {
    switch (key) {
      case 'info':
        console.log('Show chat info');
        // TODO: открыть модалку с информацией
        break;
      case 'leave':
        console.log('Leave chat');
        // TODO: покинуть чат
        break;
    }
  };

  const handleBack = () => {
    dispatch(setCurrentChatId(null));
  };

  const otherUserRole = getOtherUserRole();

  return (
    <div className="flex items-center justify-between px-4 py-3 border-b border-gray-200 bg-white shadow-sm">
      {/* Left side - Avatar and info */}
      <div className="flex items-center gap-3">
        {/* Back button (видна только на мобильных) */}
        <Button
          type="text"
          icon={<ArrowLeftOutlined />}
          onClick={handleBack}
          className="lg:hidden"
        />

        {/* Avatar with online status */}
        <div className="relative">
          <Avatar 
            size={48} 
            src={getChatAvatar(chat)} 
            icon={<UserOutlined />}
            className="border-2 border-gray-200"
          />
          {/* Online indicator */}
          {isOnline && chat.chat.type === 'OneToOne' && (
            <span className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></span>
          )}
        </div>

        {/* Chat info */}
        <div className="flex flex-col">
          <div className="flex items-center gap-2">
            <h2 className="font-semibold text-gray-900 text-base">
              {getChatName(chat)}
            </h2>
            {/* Role badge (для OneToOne) */}
            {otherUserRole && (
              <Tag color="blue" className="text-xs m-0">
                {otherUserRole}
              </Tag>
            )}
          </div>
          {/* Subtitle */}
          <p className="text-xs text-gray-500">
            {chat.chat.type === ChatType.Group
            // TODO: добавить возможность добавления участников для CMS Admin
              ? `${chat.chat.participants.length} участников`
              : isOnline 
                ? 'В сети' 
                : 'Был(а) недавно'
            }
          </p>
        </div>
      </div>

      {/* Right side - Actions */}
      <div className="flex items-center gap-2">
        {/* Search button */}
        {/* <Button
          type="text"
          icon={<SearchOutlined className="text-lg" />}
          onClick={() => console.log('Search')}
          className="hidden sm:inline-flex"
        /> */}

        {/* More menu
        <Dropdown 
          menu={{ items: menuItems, onClick: handleMenuClick }} 
          trigger={['click']}
          placement="bottomRight"
        >
          <Button 
            type="text" 
            icon={<MoreOutlined className="text-lg" />}
          />
        </Dropdown> */}
      </div>
    </div>
  );
};

export default ChatHeader;
