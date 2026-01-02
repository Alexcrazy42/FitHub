import { FireOutlined, HomeOutlined, RocketOutlined, ToolOutlined, UserOutlined, WechatOutlined } from "@ant-design/icons";
import { MenuItem } from "./MenuItem";
import { Gyms } from "../pages/admin/gyms/Gyms";
import { Equipments } from "../pages/admin/equipments/Equipments";
import { Trainings } from "../pages/admin/trainings/Trainings";
import { Users } from "../pages/admin/users/Users";
import AdminDashboard from "../pages/admin/AdminDashboard";
import { ChatLayout } from "../pages/chat/components/ChatLayout";
import { Badge } from "antd";

export const adminMenuConfig: MenuItem[] = [
  {
    key: 'dashboard',
    label: 'Главная',
    icon: <HomeOutlined />,
    path: '/admin/home',
    element: <AdminDashboard />,
  },
  {
    key: 'gyms',
    label: 'Залы',
    icon: <RocketOutlined />,
    path: '/admin/gyms',
    element: <Gyms />
  },
  {
    key: 'equipments',
    label: 'Тренажеры',
    icon: <ToolOutlined />,
    path: '/admin/equipments',
    element: <Equipments />
  },
  {
    key: 'trainings',
    label: 'Тренировки',
    icon: <FireOutlined />,
    path: '/admin/trainings',
    element: <Trainings />
  },
  {
    key: 'users',
    label: 'Пользователи',
    icon: <UserOutlined />,
    path: '/admin/users',
    element: <Users />
  },
  {
    key: 'chat',
    label: 'Чат',
    icon: (
      <Badge count={5} offset={[10, 0]}>
        <WechatOutlined />
      </Badge>
    ),
    path: '/admin/chat',
    element: <ChatLayout />
  },
];