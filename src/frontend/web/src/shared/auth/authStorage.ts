import type { AuthState } from "@/shared/auth/AuthProvider";

export const ACCESS_PACK_KEY = "wc_access_pack";

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

export function getAccessPack(): AuthState | null {
  try {
    return STORAGE.getItem<AuthState>(ACCESS_PACK_KEY);
  } catch {
    return null;
  }
}

export function setAccessPack(accessPack: AuthState): void {
  try {
    STORAGE.setItem<AuthState>(ACCESS_PACK_KEY, accessPack);
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
