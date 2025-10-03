// Sidebar.tsx
import React from 'react';
import { Menu, MenuProps } from 'antd';
import { Link, useLocation } from 'react-router-dom';
import { UserRole } from '../../types/auth';
import { adminMenuConfig } from '../../routes/adminMenuConfig';
import { useAuth } from '../../context/useAuth';
import { MenuItem } from '../../routes/MenuItem';
import { userMenuConfig } from '../../routes/userMenuConfig';

interface SidebarProps {
  collapsed: boolean;
  user?: { role: UserRole } | null;
  isDark?: boolean;
}

type AntdMenuItem = NonNullable<MenuProps['items']>[number];

const transformToAntdMenu = (items: MenuItem[], collapsed: boolean): AntdMenuItem[] => {
  return items.map((item) => {
    const hasChildren = item.children && item.children.length > 0;

    const isLeaf = item.path && !hasChildren;

    const baseItem: AntdMenuItem = {
      key: item.key,
      icon: item.icon,
      title: item.label,
      label: isLeaf ? (
        <Link to={item.path!} className="flex items-center">
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
    menuConfig = userMenuConfig;
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