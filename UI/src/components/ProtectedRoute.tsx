import { Navigate, Outlet } from "react-router-dom";
import { authStorage } from "../utils/authStorage";

interface ProtectedRouteProps {
  allowedRoles?: string[];
}

export default function ProtectedRoute({ allowedRoles }: ProtectedRouteProps) {
  const auth = authStorage.get();
  const token = authStorage.getToken();

  if (!auth || !token) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && !allowedRoles.some((role) => auth.roles.includes(role))) {
    return <Navigate to="/dashboard" replace />;
  }

  return <Outlet />;
}
