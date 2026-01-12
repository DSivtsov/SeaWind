import { apiRequest } from "@/shared/api/apiRequests";

export type AuthTokenResponseDto = {
    accessToken: string;
};

export async function registerRequest(email: string, password: string, signal?: AbortSignal): Promise<void> {
    const body = { email, password };
    return apiRequest<void>("/api/auth/register", { method: "POST", parse: "empty", body, signal }, null);
}

export async function loginRequest(email: string, password: string, signal?: AbortSignal): Promise<AuthTokenResponseDto> {
    const body = { email, password };
    return apiRequest<AuthTokenResponseDto>("/api/auth/login", { method: "POST", parse: "json", body, signal }, null);
}
