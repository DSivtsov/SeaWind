import { buildUrl } from "@/shared/api/httpWrappers";
import { readErrorMessage } from "@/shared/api/readErrorMessage";

const RETRY_DELAYS = [200, 500, 1000]; // ms

function shouldRetry(method: string, attempt: number, e: unknown, signal?: AbortSignal): boolean {
  if (signal?.aborted) return false;
  if (method !== "GET") return false;
  if (attempt >= RETRY_DELAYS.length) return false;

  if (isApiError(e) && e.kind === "network") return true;
  if (isApiError(e) && e.kind === "http" && e.status && [502, 503, 504].includes(e.status)) return true;

  return false;
}

function sleep(ms: number) {
  return new Promise(r => setTimeout(r, ms));
}

export type ApiErrorKind = "http" | "network" | "abort" | "parse";

export type ApiError = {
  kind: ApiErrorKind;
  message: string;
  status?: number;
};

function httpError(kind: ApiErrorKind, message: string, status?: number): ApiError {
  return { kind, message, status };
}

function isAbortError(e: unknown): boolean {
  return e instanceof DOMException && e.name === "AbortError";
}

export function isApiError(e: unknown): e is ApiError {
  return typeof e === "object" && e !== null && "kind" in e && "message" in e;
}

export function isUnauthorized(e: unknown): boolean {
  return isApiError(e) && e.kind === "http" && e.status === 401;
}

export function isAbort(e: unknown): boolean {
  return isApiError(e) && e.kind === "abort";
}

type RequestOptions = {
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
  body?: unknown;
  signal?: AbortSignal;
  token?: string | null;
};

export async function apiRequest<T>(path: string, opts: RequestOptions): Promise<T> {
  let attempt = 0;

  while (true) {
    try {
      return await singleAttempt<T>(path, opts);
    } catch (e: unknown) {
      if (shouldRetry(opts.method, attempt, e, opts.signal)) {
        await sleep(RETRY_DELAYS[attempt++]);
        continue;
      }
      throw e;
    }
  }
}

export async function singleAttempt<T>(path: string, opts: RequestOptions): Promise<T> {
  const headers: Record<string, string> = {
    Accept: "application/json",
  };

  const hasBody = opts.body !== undefined;
  if (hasBody) headers["Content-Type"] = "application/json";

  if (opts.token) headers["Authorization"] = `Bearer ${opts.token}`;

  try {
    const res = await fetch(buildUrl(path), {
      method: opts.method,
      headers,
      body: hasBody ? JSON.stringify(opts.body) : undefined,
      signal: opts.signal,
    });

    if (!res.ok) {
      //const msg = await safeReadText(res);
      const msg = await readErrorMessage(res);
      throw httpError("http", msg, res.status);
    }

    // For 204 No Content etc.
    if (res.status === 204) return undefined as T;

    try {
      return (await res.json()) as T;
    } catch {
      throw httpError("parse", "Failed to parse JSON", res.status);
    }
  } catch (e: unknown) {
    if (isAbortError(e)) throw httpError("abort", "Aborted");
    if (isApiError(e)) throw e;

    const msg = e instanceof Error ? e.message : "Network error";
    throw httpError("network", msg);
  }
}
