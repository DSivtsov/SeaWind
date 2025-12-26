import { apiRequest } from "@/shared/api/apiRequests";

/**
 * MVP buildUrl:
 * - dev: keep "/api/..." (Vite proxy)
 * - prod: switch to import.meta.env.VITE_API_BASE_URL + path
 */

export function buildUrl(path: string): string {
    return path;
}

export async function apiGet<T>(path: string, signal?: AbortSignal, token?: string | null): Promise<T> {
    return apiRequest<T>(path, { method: "GET", signal, token });
}

export async function apiPost<T>(path: string, body: unknown, token?: string | null): Promise<T> {
    return apiRequest<T>(path, { method: "POST", body, token });
}

export async function apiPut<T>(path: string, body: unknown, token?: string | null): Promise<T> {
    return apiRequest<T>(path, { method: "PUT", body, token });
}

export async function apiPatch<T>(path: string, body: unknown, token?: string | null): Promise<T> {
    return apiRequest<T>(path, { method: "PATCH", body, token });
}

export async function apiDelete<T>(path: string, token?: string | null): Promise<T> {
    return apiRequest<T>(path, { method: "DELETE", token });
}
