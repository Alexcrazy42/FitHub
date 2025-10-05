import TestOrderGrid from '../pages/user/TestGrid/TestOrderGrid';
import UserProfile from '../pages/user/Profile/Profile'
import { MenuItem } from './MenuItem';
import { CodeSandboxOutlined, HistoryOutlined, HomeOutlined, UserOutlined } from '@ant-design/icons';


export const userMenuConfig : MenuItem[] = [
  {
    key: '/user/home',
    label: 'Главная',
    icon: <HomeOutlined />,
    path: '/user/home',
    element: <UserProfile />
  },
  {
    key: '/user/profile',
    label: 'Профиль',
    icon: <UserOutlined />,
    path: '/user/profile',
    element: <UserProfile />
  },
  {
    key: '/user/orders',
    label: 'Песочница',
    icon: <CodeSandboxOutlined />,
    children: [
      { key: '/user/orders/history', 
        label: 'Заказы (тест)', 
        icon: <HistoryOutlined />, 
        path: '/user/orders/history',
        element: <TestOrderGrid />
      }
    ],
  },
];
