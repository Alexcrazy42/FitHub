// Sidebar.tsx
import React from 'react';
import { Menu, MenuProps } from 'antd';
import { Link, useLocation } from 'react-router-dom';
import {
  DashboardOutlined,
  SettingOutlined,
  HomeOutlined,
  UserOutlined,
  OrderedListOutlined,
  ShopOutlined,
  FileTextOutlined,
  LikeOutlined,
} from '@ant-design/icons';
import { UserRole } from '../../types/auth';
import { adminMenuConfig } from '../../routes/adminMenuConfig';
import { useAuth } from '../../context/useAuth';

interface SidebarProps {
  collapsed: boolean;
  user?: { role: UserRole } | null;
  isDark?: boolean;
}

interface MenuItem {
  key: string;
  label: string;
  icon?: React.ReactNode;
  path?: string; // если есть — это ссылка
  children?: MenuItem[]; // если есть — это папка
}

const userMenuItems: MenuItem[] = [
  {
    key: '/user/home',
    label: 'Главная',
    icon: <HomeOutlined />,
    path: '/user/home',
  },
  {
    key: '/user/profile',
    label: 'Профиль',
    icon: <UserOutlined />,
    path: '/user/profile',
  },
  {
    key: '/user/orders',
    label: 'Мои заказы',
    icon: <OrderedListOutlined />,
    children: [
      { key: '/user/orders/history', label: 'История', icon: <UserOutlined />, path: '/user/orders/history' },
      { key: '/user/orders/favorites', label: 'Избранное', icon: <LikeOutlined />, path: '/user/orders/favorites' },
    ],
  },
];

type AntdMenuItem = Required<MenuProps>['items'][number];

const transformToAntdMenu = (items: MenuItem[], collapsed: boolean): AntdMenuItem[] => {
  return items.map((item) => {
    const hasChildren = item.children && item.children.length > 0;

    const baseItem: AntdMenuItem = {
      key: item.key,
      icon: item.icon,
      title: item.label, // ← КЛЮЧЕВОЙ МОМЕНТ: title для tooltip и popup
      label: item.path && !hasChildren ? (
        <Link to={item.path} className="flex items-center">

          {!collapsed && item.label}
        </Link>
      ) : (
        item.label
      ),
    };

    if (hasChildren) {
      baseItem.children = transformToAntdMenu(item.children!, collapsed);
    }

    return baseItem;
  });
};

const Sidebar: React.FC<SidebarProps> = ({ collapsed, isDark = false }) => {
  const location = useLocation();
  const {user} = useAuth();

  let menuConfig: MenuItem[] = [];
  if (user?.role === 'admin') {
    menuConfig = adminMenuConfig;
  } else if (user?.role === 'user') {
    menuConfig = userMenuItems;
  }

  const menuItems = transformToAntdMenu(menuConfig, collapsed);

  return (
    <Menu
      theme={isDark ? 'dark' : 'light'}
      mode="inline"
      selectedKeys={[location.pathname]}
      inlineCollapsed={collapsed}
      items={menuItems}
    />
  );
};

export default Sidebar;