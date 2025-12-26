import { type ApiError, isAbort, isUnauthorized } from "./apiRequests";
import { logoutAndOpenAuthModal } from "../auth/auth";

export async function loadPageData<T>(
  action: () => Promise<T>,
  onSuccess: (data: T) => void,
  onError: (message: string) => void
): Promise<void> {
  try {
    const data = await action();
    onSuccess(data);
  } catch (e: unknown) {
    if (isAbort(e)) return;

    if (isUnauthorized(e)) {
      logoutAndOpenAuthModal();
      return;
    }

    const msg = (e as ApiError).message ?? "Request failed";
    onError(msg);
  }
}
