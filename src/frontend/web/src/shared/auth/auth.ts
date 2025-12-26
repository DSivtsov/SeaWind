/**
 * WorkshopCode MVP auth glue.
 * Choose simple approach:
 * - token stored in memory (or localStorage if you already do that)
 * - on 401: clear token + open AuthModal
 *
 * Replace internals with your actual state/store later.
 */

let token: string | null = null;

export function getToken(): string | null {
  return token;
}

export function setToken(next: string | null): void {
  token = next;
}

export function logoutAndOpenAuthModal(): void {
  token = null;
  // MVP: trigger AuthModal in your app shell.
  // Replace with your real implementation (context/store/event).
  window.dispatchEvent(new CustomEvent("auth:required"));
}
