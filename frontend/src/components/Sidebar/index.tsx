// Sidebar.tsx
import React from 'react';
import { Menu, MenuProps } from 'antd';
import { Link, useLocation } from 'react-router-dom';
import { UserRole } from '../../types/auth';
import { adminMenuConfig } from '../../routes/adminMenuConfig';
import { useAuth } from '../../context/useAuth';
import { MenuItem } from '../../routes/MenuItem';
import { userMenuConfig } from '../../routes/userMenuConfig';
import { gymAdminMenuConfig } from '../../routes/gymAdminMenuConfig';

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
  if (user?.currentRole === 'CmsAdmin') {
    menuConfig = adminMenuConfig;
  } else if (user?.currentRole === 'GymVisitor') {
    menuConfig = userMenuConfig;
  } else if (user?.currentRole === 'GymAdmin') {
    menuConfig = gymAdminMenuConfig;
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