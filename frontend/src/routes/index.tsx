import { createBrowserRouter, RouteObject } from 'react-router-dom';
import NotFound from '../pages/NotFound';
import DashboardLayout from '../layouts/DashboardLayout';
import ProtectedRoute from './ProtectedRoute';
import { adminRoutes } from './AdminRoutes';
import { userRoutes } from './UserRoutes';
import Login from '../pages/Auth/Login';

const routes: RouteObject[] = [
  { path: '/login', element: <Login /> },
  {
    element: (
      <ProtectedRoute allowedRoles={['admin', 'user']} />
    ),
    children: [
      {
        element: <DashboardLayout />,
        children: [
          {
            path: '/admin/*',
            element: <ProtectedRoute allowedRoles={['admin']} />,
            children: adminRoutes.map((route) => ({
              ...route,
              path: route.path,
            })),
          },
          {
            path: '/user/*',
            element: <ProtectedRoute allowedRoles={['user']} />,
            children: userRoutes.map((route) => ({
              ...route,
              path: route.path,
            })),
          },
        ],
      },
    ],
  },
  { path: '*', element: <NotFound /> },
];

export const router = createBrowserRouter(routes);