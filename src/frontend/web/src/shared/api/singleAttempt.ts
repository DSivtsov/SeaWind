import { type RequestOptions, buildUrl, httpError, isAbortError, isApiError } from "@/shared/api/apiRequests";
import { readErrorMessage } from "@/shared/api/readErrorMessage";
import { emitUnauthorized, getAccessToken } from "@/shared/auth/authStorage";


export async function singleAttempt<T>(path: string, opts: RequestOptions): Promise<T> {
    const parse = opts.parse ?? "json";

    const headers: Record<string, string> = {
        Accept: parse === "text" ? "text/plain, */*" : "application/json",
    };

    const token = getAccessToken();
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

        if (res.status === 401) {
            // MVP contract: 401 -> redirect to /courses and show info ("unauthorized")
            emitUnauthorized("unauthorized");
            throw httpError("http", "Unauthorized", 401);
        }

        if (res.status === 403) {
            // MVP contract: 403 -> redirect to /courses and show info ("forbidden")
            emitUnauthorized("forbidden");
            throw httpError("http", "Forbidden", 403);
        }

        if (!res.ok) {
            const msg = await readErrorMessage(res);
            throw httpError("http", msg, res.status);
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
                throw httpError("parse", "Expected empty response", res.status);
            }
            return undefined as T;
        }

        if (parse === "json") {
            if (!trimmed) {
                throw httpError("parse", "Expected JSON response", res.status);
            }
            try {
                return JSON.parse(text) as T;
            } catch {
                throw httpError("parse", "Failed to parse JSON", res.status);
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
