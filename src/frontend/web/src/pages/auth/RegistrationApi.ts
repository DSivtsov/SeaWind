import { apiRequest } from "@/shared/api/apiRequests";

export async function registerRequest(email: string, password: string, signal?: AbortSignal): Promise<void> {
    const body = { email, password };
    return apiRequest<void>("/api/auth/register", { method: "POST", parse: "empty", body, signal });
}
