import { useCallback, useEffect, useMemo, useState } from "react";
import { clearAccessPack, getAccessPack, setAccessPack, ACCESS_PACK_KEY } from "./authStorage";
import { AuthContext } from "@/shared/auth/authContext";
import { useNavigate } from "react-router-dom";
import { fetchMe, type Me } from "@/shared/auth/meApi";
import type { RedirectInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";
import { onAccessDenied, type RedirectReason } from "@/shared/auth/authListeners";

export type AuthState = {
  initials: string;
  token: string | null;
};

export type AuthApi = {
  state: AuthState;
  isAuthenticated: boolean;

  me: Me;

  login: (token: string, email: string) => void;
  logout: () => void;
};

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const navigate = useNavigate();

  const [state, setState] = useState<AuthState>(() => {
    const accessPack = getAccessPack();
    return {
      initials: accessPack?.initials ?? "",
      token: accessPack?.token ?? null,
    };
  });

  const [me, setMe] = useState<Me>(() => {
    const accessPack = getAccessPack();
    return accessPack?.token
      ? { kind: "loading" }
      : { kind: "empty" };
  });

  // Handle action at any AccessDeniedReason
  const handleOnAccessDeniedCases = useCallback((reason?: RedirectReason) => {
    const info: RedirectInfo | undefined = reason ? { reason: reason } : undefined;
    navigate("/courses", {
      replace: true,
      state: info,
    });
  }, [navigate]);

  useEffect(() => {
    const unsubscribe = onAccessDenied((reason) => {
      if (reason === "unauthorized") {
        clearAccessPack();
        setMe({ kind: "empty" });
        setState({ initials: "", token: null });
      }

      handleOnAccessDeniedCases(reason);
    });
    return unsubscribe;
  }, [handleOnAccessDeniedCases]);

  // Handle auto-logout from all pages if it occurs
  useEffect(() => {
    const onStorage = (e: StorageEvent) => {
      // реагируем только на изменения ключа токена в localStorage
      if (e.storageArea !== localStorage || e.key !== ACCESS_PACK_KEY) return;

      // если в другой вкладке токен удалили -> разлогиниваемся здесь
      if (e.newValue == null) {
        setMe({ kind: "empty" });
        setState({ initials: "", token: null });
        handleOnAccessDeniedCases();
      }
    };

    window.addEventListener("storage", onStorage);
    return () => window.removeEventListener("storage", onStorage);
  }, [handleOnAccessDeniedCases]);

  // Handle State of object Me on any token changes
  useEffect(() => {
    const token = state.token;
    if (!token) {
      setMe({ kind: "empty" });
      return;
    }

    const ctrl = new AbortController();

    const loadMeAsync = async () => {
      setMe({ kind: "loading" });

      try {
        const user = await fetchMe(token, ctrl.signal);
        setMe({ kind: "ready", user });
      } catch {
        // сюда попадают network/5xx и также 401/403 (которые уже обработаны через emit+redirect)
        if (ctrl.signal.aborted) return;   // ← выход при “нормальном” прерывании
        setMe({ kind: "error" });    // ← только реальная ошибка
      }
    };

    loadMeAsync();

    return () => ctrl.abort();
  }, [state.token]);

  const login = useCallback((token: string, email: string) => {
    const namePart = email.split("@")[0];
    const initials = namePart.slice(0, 2).toUpperCase();

    setState({ initials, token });
    setAccessPack({ initials, token });
  }, []);

  const logout = useCallback(() => {
    clearAccessPack();
    setMe({ kind: "empty" });
    setState({ initials: "", token: null });
  }, []);

  // Handle State of object AuthApi on any changes
  const api = useMemo<AuthApi>(() => {
    return {
      state,
      isAuthenticated: Boolean(state.token),
      me,
      login,
      logout,
    };
  }, [state, me, login, logout]);

  return <AuthContext.Provider value={api}>{children}</AuthContext.Provider>;
}
