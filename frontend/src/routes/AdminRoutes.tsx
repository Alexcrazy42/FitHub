import { RouteObject } from 'react-router-dom';
import AdminDashboard from '../pages/admin/Dashboard';
import AdminSettings from '../pages/admin/Settings';

export const adminRoutes: RouteObject[] = [
  { path: '', element: <AdminDashboard /> },
  { path: 'settings', element: <AdminSettings /> },
];