import { isApiError } from "@/shared/api/apiError";
import { singleAttempt } from "@/shared/api/singleAttempt";

const RETRY_DELAYS = [200, 500, 1000]; // ms

type JsonBody = Record<string, unknown> | unknown[] | null;

type JsonRequestOptions = {
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
  parse?: "json" | "empty";
  body?: JsonBody;        // string запрещён
  signal?: AbortSignal;
};

type TextRequestOptions = {
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
  parse: "text";
  body?: string;          // string разрешён
  signal?: AbortSignal;
};

type BlobRequestOptions = {
  method: "GET";
  parse: "blob";
  signal?: AbortSignal;
};

type FormRequestOptions = {
  method: "POST" | "PUT" | "PATCH";
  parse?: "json" | "empty";
  body: FormData;
  signal?: AbortSignal;
};

export type RequestOptions = JsonRequestOptions | TextRequestOptions | BlobRequestOptions | FormRequestOptions;

export function buildUrl(path: string): string {
  return path;
}

function sleep(ms: number) {
  return new Promise(r => setTimeout(r, ms));
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

// Делает Retry только для Get см. shouldRetry()
export async function apiRequest<T>(path: string, opts: RequestOptions, token: string | null): Promise<T> {
  let attempt = 0;

  while (true) {
    try {
      return await singleAttempt<T>(path, opts, token);
    } catch (e: unknown) {
      if (shouldRetry(opts.method, attempt, e, opts.signal)) {
        await sleep(RETRY_DELAYS[attempt++]);
        continue;
      }
      throw e;
    }
  }
}
