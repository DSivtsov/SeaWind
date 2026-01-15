import { apiRequest } from "@/shared/api/apiRequests";

export type Role = "Guest" | "FreeStudent" | "Student" | "Mentor" | "Admin";

export type User = {
    role: Role;
    email: string;
};

export type Me =
    | { kind: "empty" }          // нет токена
    | { kind: "loading" }       // дергаем /me
    | { kind: "error" }         // 500 / network
    | { kind: "ready"; user: User };


export async function fetchMe(token: string, signal?: AbortSignal): Promise<User> {
    return apiRequest<User>("/api/users/me", { method: "GET", parse: "json", signal }, token);
}
