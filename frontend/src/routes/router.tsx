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
import { EquipmentPage } from '../pages/admin/equipments/EquipmentPage';
import { ResetPassword } from '../pages/Auth/ResetPassword';
import { ConfirmEmail } from '../pages/Auth/ConfirmEmail';
import { SetPassword } from '../pages/Auth/SetPassword';
import { Main } from '../pages/Main';
import { gymAdminMenuConfig } from './gymAdminMenuConfig';
import UserProfilePage from '../pages/profile/UserProfilePage';
import MyAccountPage from '../pages/account/MyAccountPage';


const getAdminRoutePath = (fullPath: string): string => {
  return fullPath.replace(/^\/admin\//, '');
};

const getUserRoutePath = (fullPath: string) : string => {
  return fullPath.replace(/^\/user\//, '');
}

const getGymAdminRoutePath = (fullPath: string) : string => {
  return fullPath.replace(/^\/gym-admin\//, '');
}


export enum UserType {
  Admin,
  User,
  GymAdmin
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
      } else if (userType === UserType.GymAdmin) {
        routes.push({
          path: getGymAdminRoutePath(item.path),
          element: item.element
        })
      }
      
    }
    if (item.children) {
      routes = routes.concat(extractRoutesFromMenu(item.children, userType));
    }
  }

  return routes;
};

export const routes: RouteObject[] = [
  {path: '', element: <Main />},
  { path: '/login', element: <Login /> },
  { path: '/register', element: <Register /> },
  { path: '/access-denied', element: <AccessDenied /> },
  { path: "/card", element: <MapCard /> },
  { path: "/confirm-email", element: <ConfirmEmail /> },
  { path: "/reset-password", element: <ResetPassword /> },
  { path: "/set-password", element: <SetPassword /> },
  {
    element: <ProtectedRoute allowedRoles={['CmsAdmin', 'GymVisitor', 'GymAdmin']} />,
    children: [
      {
        element: <DashboardLayout />,
        children: [
          {
            path: '/profile/:userId',
            element: <UserProfilePage />,
          },
          {
            path: '/my-account',
            element: <MyAccountPage />,
          },
          {
            path: '/admin/*',
            element: <ProtectedRoute allowedRoles={['CmsAdmin']} />,
            children: [
              ...extractRoutesFromMenu(adminMenuConfig, UserType.Admin),
              {path: 'equipments/:equipmentId', element: <EquipmentPage />}
            ]
          },
          {
            path: '/user/*',
            element: <ProtectedRoute allowedRoles={['GymVisitor']} />,
            children: [
              ...extractRoutesFromMenu(userMenuConfig, UserType.User),
              { path: 'home/:orderId/order', element: <OrderDetailsPage /> },
            ],
          },
          {
            path: '/gym-admin/*',
            element: <ProtectedRoute allowedRoles={['GymAdmin']} />,
            children: [
              ...extractRoutesFromMenu(gymAdminMenuConfig, UserType.GymAdmin)
            ]
          }
        ],
      },
    ],
  },
  { path: '*', element: <NotFound /> },
];

export const router = createBrowserRouter(routes);