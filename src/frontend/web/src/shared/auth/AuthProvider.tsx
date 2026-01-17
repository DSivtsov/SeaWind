import { useCallback, useEffect, useMemo, useState } from "react";
import { clearAccessPack, getAccessPack, onAccessDenied, setAccessPack, ACCESS_PACK_KEY, type StoredAuth, type LoginReason } from "./authStorage";
import { AuthContext } from "@/shared/auth/useAuth";
import { useNavigate } from "react-router-dom";
import { fetchMe, type Me } from "@/shared/auth/meApi";
import type { AccessDeniedInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";

export type AuthState = {
  initials: string;
  accessToken: string | null;
};

export type AuthApi = {
  state: AuthState;
  isAuthenticated: boolean;

  me: Me;

  login: (accessToken: string, email: string) => void;
  logout: () => void;
};

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const navigate = useNavigate();

  const handleLogoutRedirect = useCallback((reason?: LoginReason) => {
    const info: AccessDeniedInfo | undefined = reason ? { reason: reason } : undefined;
    navigate("/courses", {
      replace: true,
      state: info,
    });
  }, [navigate]);

  const [state, setState] = useState<AuthState>(() => {
    const accessPack = getAccessPack();
    return {
      initials: accessPack?.initials ?? "",
      accessToken: accessPack?.accessToken ?? null,
    };
  });

  const [me, setMe] = useState<Me>({ kind: "empty" });

  useEffect(() => {
    // MVP: on unauthorized/forbidden -> logout (if needed) and redirect to /courses with reason.
    const unsub = onAccessDenied((reason) => {
      if (reason === "unauthorized") {
        clearAccessPack();
        setMe({ kind: "empty" });
        setState({ initials: "", accessToken: null });
      }

      handleLogoutRedirect(reason);
    });
    return unsub;
  }, [handleLogoutRedirect]);

  useEffect(() => {
    const onStorage = (e: StorageEvent) => {
      // реагируем только на изменения ключа токена в localStorage
      if (e.storageArea !== localStorage || e.key !== ACCESS_PACK_KEY) return;

      // если в другой вкладке токен удалили -> разлогиниваемся здесь
      if (e.newValue == null) {
        setMe({ kind: "empty" });
        setState({ initials: "", accessToken: null });
        handleLogoutRedirect();
      }
    };

    window.addEventListener("storage", onStorage);
    return () => window.removeEventListener("storage", onStorage);
  }, [handleLogoutRedirect]);

  useEffect(() => {
    const accessToken = state.accessToken;

    if (!accessToken) {
      setMe({ kind: "empty" });
      return;
    }

    const ctrl = new AbortController();

    const loadMe = async () => {
      setMe({ kind: "loading" });

      try {
        const user = await fetchMe(accessToken, ctrl.signal);
        setMe({ kind: "ready", user });
      } catch {
        // сюда попадают network/5xx и также 401/403 (которые уже обработаны через emit+redirect)
        if (ctrl.signal.aborted) return;   // ← выход при “нормальном” прерывании
        setMe({ kind: "error" });    // ← только реальная ошибка
      }
    };

    loadMe();

    return () => ctrl.abort();
  }, [state.accessToken]);

  const api = useMemo<AuthApi>(() => {
    return {
      state,
      isAuthenticated: Boolean(state.accessToken),

      me,

      login: (token: string, email: string) => {
        const namePart = email.split("@")[0];
        const initials = namePart.slice(0, 2).toUpperCase();

        setState({ initials, accessToken: token });

        const accessPack: StoredAuth = { accessToken: token, initials };
        setAccessPack(accessPack);
      },
      logout: () => {
        clearAccessPack();
        setMe({ kind: "empty" });
        setState({ initials: "", accessToken: null });
      },
    };
  }, [state, me]);

  return <AuthContext.Provider value={api}>{children}</AuthContext.Provider>;
}
