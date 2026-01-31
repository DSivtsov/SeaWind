import { type RequestOptions, buildUrl } from "@/shared/api/apiRequests";
import { httpError, isAbortError, isApiError } from "./apiError";
import { readErrorMessage } from "@/shared/api/readErrorMessage";
import { emitAccessDenied } from "@/shared/auth/authListeners";

export async function singleAttempt<T>(path: string, opts: RequestOptions, token: string | null): Promise<T> {
    const parse = opts.parse ?? "json";

    const headers: Record<string, string> = {
        Accept: parse === "text" ? "text/plain, */*" : "application/json",
    };

    if (token) headers.Authorization = `Bearer ${token}`;

    let body: string | undefined;

    if ("body" in opts && opts.body !== undefined) {
        if (typeof opts.body === "string") {
            // text request
            headers["Content-Type"] = "text/plain; charset=utf-8";
            body = opts.body;
        } else {
            // json request
            headers["Content-Type"] = "application/json";
            body = JSON.stringify(opts.body);
        }
    }

    try {
        const res = await fetch(buildUrl(path), {
            method: opts.method,
            headers,
            body,
            signal: opts.signal,
        });

        const correlationId = res.headers.get("x-correlation-id") ?? undefined;

        if (res.status === 401) {
            // MVP contract: 401 -> redirect to /courses and show info ("unauthorized")
            emitAccessDenied("unauthorized");
            throw httpError("http", "Unauthorized", 401, correlationId);
        }

        if (res.status === 403) {
            // MVP contract: 403 -> redirect to /courses and show info ("forbidden")
            emitAccessDenied("forbidden");
            throw httpError("http", "Forbidden", 403, correlationId);
        }

        if (!res.ok) {
            const msg = await readErrorMessage(res);
            throw httpError("http", msg, res.status, correlationId);
        }

        // For 204 No Content etc.
        if (res.status === 204) return undefined as T;

        const text = await res.text();
        const trimmed = text.trim();

        if (parse === "text") {
            return text as T;
        }

        if (parse === "empty") {
            if (trimmed) {
                throw httpError("parse", "Expected empty response", res.status, correlationId);
            }
            return undefined as T;
        }

        if (parse === "json") {
            if (!trimmed) {
                throw httpError("parse", "Expected JSON response", res.status, correlationId);
            }
            try {
                return JSON.parse(text) as T;
            } catch {
                throw httpError("parse", "Failed to parse JSON", res.status, correlationId);
            }
        }

        throw new Error(`unreachable parse type: ${parse}`);

    } catch (e: unknown) {
        if (isAbortError(e)) throw httpError("abort", "Aborted");
        if (isApiError(e)) throw e;

        const msg = e instanceof Error ? e.message : "Network error";
        throw httpError("network", msg);
    }
}
