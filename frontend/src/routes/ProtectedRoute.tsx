import { Navigate, Outlet } from 'react-router-dom';
import { useAuth } from '../context/useAuth';
import { roleRoutes, UserRole } from '../types/auth';

interface ProtectedRouteProps {
  allowedRoles: UserRole[];
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ allowedRoles }) => {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/login" replace />;
  }


  if (!allowedRoles.includes(user.currentRole)) {
    //const target = roleRoutes[user.currentRole];
   // const safeTarget = target ? `${target}` : '/';
    return <Navigate to={'/'} replace />;
  }
  

  return <Outlet />;
};

export default ProtectedRoute;