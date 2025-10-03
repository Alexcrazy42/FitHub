import UserHome from '../pages/user/Home';
import OrderDetailsPage from '../pages/user/OrderDetailsPage';
import UserProfile from '../pages/user/Profile';
import { MenuItem } from './MenuItem';
import { HistoryOutlined, HomeOutlined, LikeOutlined, OrderedListOutlined, UserOutlined } from '@ant-design/icons';


export const userMenuConfig : MenuItem[] = [
  {
    key: '/user/home',
    label: 'Главная',
    icon: <HomeOutlined />,
    path: '/user/home',
    element: <UserHome />
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
    label: 'Мои заказы',
    icon: <OrderedListOutlined />,
    children: [
      { key: '/user/orders/history', 
        label: 'История', 
        icon: <HistoryOutlined />, 
        path: '/user/orders/history',
        element: <UserProfile />
      },
      { 
        key: '/user/orders/favorites', 
        label: 'Избранное', 
        icon: <LikeOutlined />, 
        path: '/user/orders/favorites',
        element: <UserProfile /> 
      }
    ],
  },
];
