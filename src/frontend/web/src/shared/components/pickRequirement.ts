import type { Role } from "@/shared/auth/meApi";

type Requirement = { kind: "auth-only"; } |
{ kind: "role-only"; roles: Role[]; };
export function pickRequirement(pathname: string): Requirement {
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
