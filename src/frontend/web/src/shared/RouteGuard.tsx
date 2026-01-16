import type { AccessDeniedInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";
import type { Role } from "@/shared/auth/meApi";
import { useAuth } from "@/shared/auth/useAuth";
import { PagePlaceholder } from "@/shared/PagePlaceholder";
import { Navigate, Outlet, useLocation } from "react-router-dom";

type Requirement =
  | { kind: "auth-only" }
  | { kind: "role-only"; roles: Role[] };

function pickRequirement(pathname: string): Requirement {
  // Order matters (top-down): first match wins.

  // Admin area
  if (/^\/admin(\/|$)/.test(pathname)) {
    return { kind: "role-only", roles: ["Admin"] };
  }

  // Mentor area
  if (/^\/mentor(\/|$)/.test(pathname)) {
    return { kind: "role-only", roles: ["Mentor"] };
  }

  // Exercise chat (student + mentor)
  if (/^\/courses\/[^/]+\/exercises\/[^/]+\/chat$/.test(pathname)) {
    return { kind: "role-only", roles: ["Student", "Mentor"] };
  }

  // Workshops tab (student + mentor + admin)
  if (/^\/courses\/[^/]+\/workshop-sessions(\/|$)/.test(pathname)) {
    return { kind: "role-only", roles: ["Student", "Mentor", "Admin"] };
  }

  // Course tabs that are auth-only
  if (/^\/courses\/[^/]+\/(lectures|exercises)(\/|$)/.test(pathname)) {
    return { kind: "auth-only" };
  }

  // Profile area (auth-only)
  if (/^\/profile(\/|$)/.test(pathname)) {
    return { kind: "auth-only" };
  }

  // Fallback for unknown guarded routes: treat as auth-only.
  return { kind: "auth-only" };
}

export function RouteGuard() {
  const auth = useAuth();
  const location = useLocation();

  if (!auth.isAuthenticated) {

    const info: AccessDeniedInfo = { reason: "unauthorized", fromLocation: location.pathname };
    //console.log(`[RouteGuard] ${JSON.stringify(info, null, 2)}`);
    //return <Navigate to="/courses" replace state={info} />;
    return <Navigate to="/courses" replace state={info} />;
  }

  // below only if auth.isAuthenticated = true

  // Layout-level loading: we have a token, but role isn't known yet.
  if (auth.me.kind === "loading") {
    return <PagePlaceholder title="Loading..." />;
  }

  if (auth.me.kind === "ready") {
    const requirement = pickRequirement(location.pathname);

    if (requirement.kind === "role-only" && !requirement.roles.includes(auth.me.user.role)) {
      return <Navigate to="/403" replace />;
    }

    return <Outlet />;
  }

  //if (auth.me.kind === "error" || auth.me.kind === "empty") or "other any"
  return <PagePlaceholder title="Error..." />;
}

