import { singleAttempt } from "@/shared/api/singleAttempt";

const RETRY_DELAYS = [200, 500, 1000]; // ms

export function buildUrl(path: string): string {
  return path;
}

function shouldRetry(method: string, attempt: number, e: unknown, signal?: AbortSignal): boolean {
  if (signal?.aborted) return false;
  if (method !== "GET") return false;
  if (attempt >= RETRY_DELAYS.length) return false;

  if (!isApiError(e)) return false;

  if (e.kind === "network") return true;
  if (e.kind === "http" && e.status && [502, 503, 504].includes(e.status)) return true;

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

export function httpError(kind: ApiErrorKind, message: string, status?: number): ApiError {
  return { kind, message, status };
}

export function isAbortError(e: unknown): boolean {
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

type JsonBody = Record<string, unknown> | unknown[] | null;

type JsonRequestOptions = {
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
  parse?: "json" | "empty";
  body?: JsonBody;        // string запрещён
  signal?: AbortSignal;
  token?: string | null;
};

type TextRequestOptions = {
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
  parse: "text";
  body?: string;          // string разрешён
  signal?: AbortSignal;
  token?: string | null;
};

export type RequestOptions = JsonRequestOptions | TextRequestOptions;

// Делает Retry только для Get см. shouldRetry()
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
