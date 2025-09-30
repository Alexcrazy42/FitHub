// src/components/Sidebar/index.tsx
import { Menu } from 'antd';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import { adminRoutes } from '../../routes/AdminRoutes';
import { userRoutes } from '../../routes/UserRoutes';
import { useTheme } from '../../context/ThemeContext';

interface SidebarProps {
  collapsed: boolean;
}

const Sidebar: React.FC<SidebarProps> = ({ collapsed }) => {
  const { user } = useAuth();
  const location = useLocation();
   const { theme } = useTheme();

   const isDark = theme === 'dark';

  const getMenuItems = () => {
    if (user?.role === 'admin') {
      return adminRoutes.map((route) => {
        const path = route.path === '' ? '' : `/${route.path}`;
        return {
          key: `/admin${path}`,
          label: !collapsed ? (
            <Link to={`/admin${path}`}>
              {route.path === '' ? 'Dashboard' : route.path.charAt(0).toUpperCase() + route.path.slice(1)}
            </Link>
          ) : null,
          icon: route.path === '' ? '📊' : '⚙️', // можно заменить на Ant Design иконки
        };
      });
    }
    

    if (user?.role === 'user') {
      return userRoutes.map((route) => {
        const path = route.path === '' ? '' : `/${route.path}`;
        return {
          key: `/user${path}`,
          label: !collapsed ? (
            <Link to={`/user${path}`}>
              {route.path === '' ? 'Home' : route.path.charAt(0).toUpperCase() + route.path.slice(1)}
            </Link>
          ) : null,
          icon: route.path === '' ? '🏠' : '👤',
        };
      });
    }

    return [];
  };

   const items = getMenuItems().map((item) => ({
    ...item,
    label: (
      <Link to={item.key} className="flex items-center">
        {item.icon}
        {!collapsed && <span className="ml-2">{item.label}</span>}
      </Link>
    ),
    icon: null,
  }));

  // const items = user?.role === 'admin'
  //   ? [
  //       { key: '/admin', icon: '🏠', label: 'Dashboard' },
  //       { key: '/admin/settings', icon: '👤', label: 'Settings' },
  //     ]
  //   : user?.role === 'user'
  //   ? [
  //       { key: '/user', icon: '📊', label: 'Home' },
  //       { key: '/user/profile', icon: '', label: 'Profile' },
  //     ]
  //   : [];

  return (
    <Menu
      theme={isDark ? 'dark' : 'light'} // ← важно!
      mode="inline"
      selectedKeys={[location.pathname]}
      items={items}
      inlineCollapsed={collapsed}
    />
  );
};

export default Sidebar;