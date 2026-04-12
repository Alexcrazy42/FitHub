import { CalendarOutlined, HomeOutlined, ToolOutlined, UserOutlined, VideoCameraOutlined, WechatOutlined } from "@ant-design/icons";
import { ShoppingCartOutlined } from "@ant-design/icons";
import { MenuItem } from "./MenuItem";
import GymAdminDashboard from "../pages/gym-admin/GymAdminDashboard";
import { Users } from "../pages/gym-admin/users/Users";
import { Equipments } from "../pages/gym-admin/equipments/Equipments";
import { Schedule } from "../pages/gym-admin/schedules/Schedule";
import { ChatLayout } from "../pages/chat/components/ChatLayout";
import { GymAdminVideosPage } from "../pages/gym-admin/videos/GymAdminVideosPage";
import { MarketplaceCatalogPage } from "../pages/marketplace/catalog/MarketplaceCatalogPage";

export const gymAdminMenuConfig: MenuItem[] = [
  {
    key: 'dashboard',
    label: 'Главная',
    icon: <HomeOutlined />,
    path: '/gym-admin/home',
    element: <GymAdminDashboard />,
  },
  {
    key: 'users',
    label: 'Пользователи',
    icon: <UserOutlined />,
    path: '/gym-admin/users',
    element: <Users />
  },
  {
    key: 'equipments',
    label: 'Тренажеры',
    icon: <ToolOutlined />,
    path: '/gym-admin/equipments',
    element: <Equipments />
  },
  {
    key: 'schedule',
    label: 'Расписание',
    icon: <CalendarOutlined />,
    path: '/gym-admin/schedule',
    element: <Schedule />
  },
  {
    key: 'videos',
    label: 'Видео',
    icon: <VideoCameraOutlined />,
    path: '/gym-admin/videos',
    element: <GymAdminVideosPage />,
  },
  {
    key: 'marketplace',
    label: 'Магазин',
    icon: <ShoppingCartOutlined />,
    path: '/gym-admin/marketplace/catalog',
    element: <MarketplaceCatalogPage />,
  },
  {
    key: 'chat',
    label: 'Чат',
    icon: <WechatOutlined />,
    path: '/gym-admin/chat',
    element: <ChatLayout />
  },
];
