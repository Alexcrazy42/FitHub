import { createBrowserRouter, RouteObject } from 'react-router-dom';
import NotFound from '../pages/NotFound';
import DashboardLayout from '../layouts/DashboardLayout';
import ProtectedRoute from './ProtectedRoute';
import {adminMenuConfig} from '../routes/adminMenuConfig'
import { userRoutes } from './UserRoutes';
import Login from '../pages/Auth/Login';
import { MenuItem } from './MenuItem';

const getAdminRoutePath = (fullPath: string): string => {
  return fullPath.replace(/^\/admin\//, '');
};


const extractRoutesFromMenu = (items: MenuItem[]): { path: string; element: React.ReactNode }[] => {
  let routes: { path: string; element: React.ReactNode }[] = [];

  for (const item of items) {
    // Если это лист (есть path и element) — добавляем
    if (item.path && item.element) {
      routes.push({
        path: getAdminRoutePath(item.path),
        element: item.element,
      });
    }

    // Если есть дети — рекурсивно обходим их
    if (item.children) {
      routes = routes.concat(extractRoutesFromMenu(item.children));
    }
  }

  return routes;
};

export const routes: RouteObject[] = [
  { path: '/login', element: <Login /> },
  {
    element: <ProtectedRoute allowedRoles={['admin', 'user']} />,
    children: [
      {
        element: <DashboardLayout />,
        children: [
          {
            path: '/admin/*',
            element: <ProtectedRoute allowedRoles={['admin']} />,
            children: extractRoutesFromMenu(adminMenuConfig)
          },
          {
            path: '/user/*',
            element: <ProtectedRoute allowedRoles={['user']} />,
            children: userRoutes,
          },
        ],
      },
    ],
  },
  { path: '*', element: <NotFound /> },
];

export const router = createBrowserRouter(routes);