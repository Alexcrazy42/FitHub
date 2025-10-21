import { createBrowserRouter, RouteObject } from 'react-router-dom';
import NotFound from '../pages/NotFound';
import DashboardLayout from '../layouts/DashboardLayout';
import ProtectedRoute from './ProtectedRoute';
import {adminMenuConfig} from './adminMenuConfig'
import Login from '../pages/Auth/Login';
import { MenuItem } from './MenuItem';
import { userMenuConfig } from './userMenuConfig';
import OrderDetailsPage from '../pages/user/TestGrid/OrderDetailsPage';
import { Register } from '../pages/Auth/Register';
import AccessDenied from '../pages/AccessDenied';
import { MapCard } from '../pages/Card';


const getAdminRoutePath = (fullPath: string): string => {
  return fullPath.replace(/^\/admin\//, '');
};

const getUserRoutePath = (fullPath: string) : string => {
  return fullPath.replace(/^\/user\//, '');
}

export enum UserType {
  Admin,
  User
}

const extractRoutesFromMenu = (items: MenuItem[], userType: UserType): { path: string; element: React.ReactNode }[] => {
  let routes: { path: string; element: React.ReactNode }[] = [];

  for (const item of items) {
    if (item.path && item.element) {
      if(userType === UserType.Admin) {
        routes.push({
          path: getAdminRoutePath(item.path),
          element: item.element,
        });
      } else if (userType === UserType.User) {
        routes.push({
          path: getUserRoutePath(item.path),
          element: item.element,
        });
      }
      
    }
    if (item.children) {
      routes = routes.concat(extractRoutesFromMenu(item.children, userType));
    }
  }

  return routes;
};

export const routes: RouteObject[] = [
  { path: '/login', element: <Login /> },
  { path: '/register', element: <Register /> },
  { path: '/access-denied', element: <AccessDenied /> },
  {path: "/card", element: <MapCard />},
  {
    element: <ProtectedRoute allowedRoles={['GymAdmin', 'GymVisitor']} />,
    children: [
      {
        element: <DashboardLayout />,
        children: [
          {
            path: '/admin/*',
            element: <ProtectedRoute allowedRoles={['GymAdmin']} />,
            children: extractRoutesFromMenu(adminMenuConfig, UserType.Admin)
          },
          {
            path: '/user/*',
            element: <ProtectedRoute allowedRoles={['GymVisitor']} />,
            children: [
              ...extractRoutesFromMenu(userMenuConfig, UserType.User),
              { path: 'home/:orderId/order', element: <OrderDetailsPage /> },
            ],
          },
        ],
      },
    ],
  },
  { path: '*', element: <NotFound /> },
];

export const router = createBrowserRouter(routes);