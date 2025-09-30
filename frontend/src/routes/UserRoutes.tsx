import { RouteObject } from 'react-router-dom';
import UserHome from '../pages/user/Home';
import UserProfile from '../pages/user/Profile';

export const userRoutes: RouteObject[] = [
  { path: '', element: <UserHome /> },
  { path: 'profile', element: <UserProfile /> },
];