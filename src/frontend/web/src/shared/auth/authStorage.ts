export type LoginReason = "unauthorized" | "forbidden";

type Listener = (reason: LoginReason) => void;

// AuthProvider — главный обработчик,
// но в будущем могут появиться: логирование, метрики
let unauthorizedListeners: Listener[] = [];

export const ACCESS_PACK_KEY = "wc_access_pack";
export type StoredAuth = { accessToken: string | null; initials: string };
//const STORAGE = localStorage;

export const STORAGE = {
  getItem<T>(key: string): T | null {
    const raw = localStorage.getItem(key);
    if (!raw) return null;

    try {
      return JSON.parse(raw) as T;
    } catch {
      return null;
    }
  },

  setItem<T>(key: string, value: T) {
    localStorage.setItem(key, JSON.stringify(value));
  },

  removeItem(key: string) {
    localStorage.removeItem(key);
  },
};

export function getAccessPack(): StoredAuth | null {
  try {
    return STORAGE.getItem<StoredAuth>(ACCESS_PACK_KEY);
  } catch {
    return null;
  }
}

export function setAccessPack(accessPack: StoredAuth): void {
  try {
    STORAGE.setItem<StoredAuth>(ACCESS_PACK_KEY, accessPack);
  } catch {
    // ignore (private mode etc.)
  }
}

export function clearAccessPack(): void {
  try {
    STORAGE.removeItem(ACCESS_PACK_KEY);
  } catch {
    // ignore
  }
}

/**
 * Subscribe to a global "unauthorized" signal (when API detects unauthorized/forbidden (401/403) ).
 * Returns function to auto unsubscribe.
 */
export function onAccessDenied(listener: Listener): () => void {
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
export function emitAccessDenied(reason: LoginReason): void {
  for (const listener of unauthorizedListeners) {
    listener(reason);
  }
}
