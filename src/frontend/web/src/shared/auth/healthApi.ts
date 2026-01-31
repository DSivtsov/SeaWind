import { apiRequest } from "@/shared/api/apiRequests";

const API_BASE = import.meta.env.VITE_API_BASE_URL || "";

export async function getHealth(signal?: AbortSignal): Promise<void> {
    return apiRequest<void>(`${API_BASE}/health`, { method: "GET", parse: "text", signal }, null);
}
