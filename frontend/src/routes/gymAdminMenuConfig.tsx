import { CalendarOutlined, HomeOutlined, ToolOutlined, UserOutlined } from "@ant-design/icons";
import { MenuItem } from "./MenuItem";
import GymAdminDashboard from "../pages/gym-admin/GymAdminDashboard";
import { Users } from "../pages/gym-admin/users/Users";
import { Equipments } from "../pages/gym-admin/equipments/Equipments";
import { Schedule } from "../pages/gym-admin/schedules/Schedule";
import { ScheduleOld } from "../pages/gym-admin/schedules/ScheduleOld";

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
];