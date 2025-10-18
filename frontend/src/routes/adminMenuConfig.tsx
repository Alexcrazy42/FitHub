import { HomeOutlined, RocketOutlined, ToolOutlined } from "@ant-design/icons";
import AdminDashboard from "../pages/admin/Dashboard";
import { MenuItem } from "./MenuItem";
import { Gyms } from "../pages/admin/gyms/Gyms";
import { Equipments } from "../pages/admin/equipments/Equipments";

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
  }
];