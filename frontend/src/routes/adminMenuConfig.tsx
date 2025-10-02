// config/adminMenu.ts

import { DashboardOutlined } from "@ant-design/icons";
import AdminDashboard from "../pages/admin/Dashboard";
import { MenuItem } from "./MenuItem";

export const adminMenuConfig: MenuItem[] = [
  {
    key: 'dashboard',
    label: 'Главная',
    icon: <DashboardOutlined />,
    path: '/admin/dashboard',
    element: <AdminDashboard />,
  },
  {
    key: 'orders',
    label: 'Заказы',
    icon: <DashboardOutlined />,
    children: [
      {
        key: 'orders-list',
        label: 'Список заказов',
        path: '/admin/orders/list',
        element: <AdminDashboard />,
      },
    ],
  },
];