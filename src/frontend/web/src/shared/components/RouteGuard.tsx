import type { AccessDeniedInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";
import { useAuthContext } from "@/shared/auth/authContext";
import { Navigate, Outlet, useLocation } from "react-router-dom";
import { pickRequirement } from "@/shared/components/pickRequirement";

export function RouteGuard() {
  const authCtx = useAuthContext();
  const location = useLocation();

  if (!authCtx.isAuthenticated) {

    const info: AccessDeniedInfo = { reason: "unauthorized", fromLocation: location.pathname };
    return <Navigate to="/courses" replace state={info} />;
  }

  // below only if auth.isAuthenticated = true

  if (authCtx.me.kind === "ready") {
    const requirement = pickRequirement(location.pathname);

    if (requirement.kind === "role-only" && !requirement.roles.includes(authCtx.me.user.role)) {
      return <Navigate to="/403" replace />;
    }

    return <Outlet />;
  }

  // other cases must catch by BootstrapGuard
  return null;
}


