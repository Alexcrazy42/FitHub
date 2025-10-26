import { FireOutlined, HomeOutlined, RocketOutlined, ToolOutlined } from "@ant-design/icons";
import AdminDashboard from "../pages/admin/Dashboard";
import { MenuItem } from "./MenuItem";
import { Gyms } from "../pages/admin/gyms/Gyms";
import { Equipments } from "../pages/admin/equipments/Equipments";
import { Trainings } from "../pages/admin/trainings/Trainings";

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
];