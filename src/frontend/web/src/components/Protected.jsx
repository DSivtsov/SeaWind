import { Navigate, useLocation } from "react-router-dom";

export default function Protected({ children }) {
  const token = sessionStorage.getItem("token");
  const location = useLocation();
  if (!token) return <Navigate to="/login" replace state={{ from: location }} />;
  return children;
}
