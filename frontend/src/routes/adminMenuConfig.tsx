import { HomeOutlined, OrderedListOutlined, RocketOutlined } from "@ant-design/icons";
import AdminDashboard from "../pages/admin/Dashboard";
import { MenuItem } from "./MenuItem";
import { Gyms } from "../pages/admin/gyms/Gyms";

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