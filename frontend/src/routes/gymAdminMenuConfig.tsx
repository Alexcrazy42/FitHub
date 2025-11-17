import { HomeOutlined, UserOutlined } from "@ant-design/icons";
import { MenuItem } from "./MenuItem";
import GymAdminDashboard from "../pages/gym-admin/GymAdminDashboard";
import { Users } from "../pages/gym-admin/users/Users";

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
];