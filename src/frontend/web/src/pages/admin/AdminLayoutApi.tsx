import { apiRequest } from "@/shared/api/apiRequests";
import type { Role } from "@/shared/auth/meApi";

export type UserRowDto = { id: string, userName: string; role: Role };

export function buildUsersUrl(params: { userName: string; role: Role | null }): string {
    const sp = new URLSearchParams();

    if (params.userName != null && params.userName.trim()) sp.set("userName", params.userName.trim());

    if (params.role) sp.set("role", params.role);

    const qs = sp.toString();
    return qs ? `/api/users?${qs}` : "/api/users";
}

export async function fetchUsers(args: {
    token: string | null;
    userName: string;
    role: Role | null;
    signal?: AbortSignal;
}): Promise<UserRowDto[]> {

    const url = buildUsersUrl({ userName: args.userName, role: args.role });

    return apiRequest<UserRowDto[]>(url, { method: "GET", parse: "json", signal: args.signal }, args.token);
}
export async function putUserRole(args: {
    token: string | null;
    userId: string;
    role: Role;
}): Promise<void> {

    const url = `/api/users/${encodeURIComponent(args.userId)}/role`;

    const body = { role: args.role };

    return apiRequest<void>(url, { method: "PUT", parse: "empty", body }, args.token);
}
