
export type ApiErrorKind = "http" | "network" | "abort" | "parse";

export type ApiError = {
    kind: ApiErrorKind;
    message: string;
    status?: number;
    correlationId?: string;
};

export function httpError(kind: ApiErrorKind, message: string, status?: number, correlationId?: string): ApiError {
    return { kind, message, status, correlationId };
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

export function haveTraceId(apiError: ApiError): boolean {
    return apiError.correlationId !== undefined;
}
