export const TOKEN_KEY = "wc_access_token";
const STORAGE = localStorage;

export type LoginReason = "unauthorized" | "forbidden";

type Listener = (reason: LoginReason) => void;

// AuthProvider — главный обработчик,
// но в будущем могут появиться: логирование, метрики
let unauthorizedListeners: Listener[] = [];

export function getAccessToken(): string | null {
  try {
    return STORAGE.getItem(TOKEN_KEY);
  } catch {
    return null;
  }
}

export function setAccessToken(token: string): void {
  try {
    STORAGE.setItem(TOKEN_KEY, token);
  } catch {
    // ignore (private mode etc.)
  }
}

export function clearAccessToken(): void {
  try {
    STORAGE.removeItem(TOKEN_KEY);
  } catch {
    // ignore
  }
}

/**
 * Subscribe to a global "unauthorized" signal (when API detects unauthorized/forbidden (401/403) ).
 * Returns function to auto unsubscribe.
 */
export function onUnauthorized(listener: Listener): () => void {
  unauthorizedListeners = [...unauthorizedListeners, listener];
  return () => {
    unauthorizedListeners = unauthorizedListeners.filter((currentListener) => currentListener !== listener);
  };
}

/**
 * Called by apiRequest() when HTTP 401 is received.
 *
 * Emits a global "unauthorized" signal to all subscribers.
 * Interpretation and UI reaction are handled outside of the API layer
 * (e.g. AuthProvider decides how to handle logout, UI decides how to prompt login).
 *
 * MVP: fan-out notification only, no direct UI logic here.
 */
export function emitUnauthorized(reason: LoginReason): void {
  for (const listener of unauthorizedListeners) {
    listener(reason);
  }
}
