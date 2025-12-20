type ProblemDetailsLike = {
  title?: unknown;
  detail?: unknown;
  message?: unknown;
};

export async function readErrorMessage(res: Response): Promise<string> {
  const contentType = (res.headers.get("content-type") ?? "").toLowerCase();

  // JSON / ProblemDetails
  if (
    contentType.includes("application/problem+json") ||
    contentType.includes("application/json")
  ) {
    try {
      const body = (await res.json()) as ProblemDetailsLike;

      const title = typeof body.title === "string" ? body.title.trim() : "";
      const detail = typeof body.detail === "string" ? body.detail.trim() : "";
      const message = typeof body.message === "string" ? body.message.trim() : "";

      // Prefer detail (usually more specific), fallback to title/message
      const combined = detail || title || message;
      return combined.length > 0 ? combined : `HTTP ${res.status}`;
    } catch {
      // fall through to Text / unknown
    }
  }

  // Text / unknown
  try {
    const text = await res.text();
    const trimmed = text.trim();
    return trimmed.length > 0 ? trimmed : `HTTP ${res.status}`;
  } catch {
    return `HTTP ${res.status}`;
  }
}
