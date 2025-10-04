import { HomeOutlined, OrderedListOutlined } from "@ant-design/icons";
import AdminDashboard from "../pages/admin/Dashboard";
import { MenuItem } from "./MenuItem";

export const adminMenuConfig: MenuItem[] = [
  {
    key: 'dashboard',
    label: 'Главная',
    icon: <HomeOutlined />,
    path: '/admin/dashboard',
    element: <AdminDashboard />,
  },
  {
    key: 'orders',
    label: 'Заказы',
    icon: <OrderedListOutlined />,
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