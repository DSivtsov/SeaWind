export type RedirectReason = "unauthorized" | "forbidden" | "invalid_state";

type Listener = (reason: RedirectReason) => void;

// AuthProvider — главный обработчик,
// но в будущем могут появиться: логирование, метрики
let unauthorizedListeners: Listener[] = [];

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
export function emitAccessDenied(reason: RedirectReason): void {
    for (const listener of unauthorizedListeners) {
        listener(reason);
    }
}
