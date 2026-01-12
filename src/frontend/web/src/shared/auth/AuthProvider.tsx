import { useEffect, useMemo, useState } from "react";
import { clearAccessToken, getAccessToken, onUnauthorized, setAccessToken, TOKEN_KEY } from "./authStorage";
import { AuthContext } from "@/shared/auth/useAuth";
import { useNavigate } from "react-router-dom";

export type AuthState = {
  isAuthenticated: boolean;
  accessToken: string | null;
};

export type AuthApi = {
  state: AuthState;
  login: (accessToken: string) => void;
  logout: () => void;
};

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [accessToken, setToken] = useState<string | null>(() => getAccessToken());
  const navigate = useNavigate();

  useEffect(() => {
    // MVP: on unauthorized/forbidden -> logout (if needed) and redirect to /courses with reason.
    const unsub = onUnauthorized((reason) => {
      clearAccessToken();
      setToken(null);
      navigate("/courses", {
        replace: true,
        state: { accessDenied: reason },
      });
    });
    return unsub;
  }, [navigate]);

  useEffect(() => {
    const onStorage = (e: StorageEvent) => {
      // реагируем только на изменения ключа токена в localStorage
      if (e.storageArea !== localStorage || e.key !== TOKEN_KEY) return;

      // если в другой вкладке токен удалили -> разлогиниваемся здесь
      if (e.newValue == null) {
        setToken(null);
        navigate("/courses", { replace: true });
      }
    };

    window.addEventListener("storage", onStorage);
    return () => window.removeEventListener("storage", onStorage);
  }, [navigate]);


  const api = useMemo<AuthApi>(() => {
    return {
      state: {
        isAuthenticated: Boolean(accessToken),
        accessToken,
      },
      login: (token) => {
        setAccessToken(token);
        setToken(token);
      },
      logout: () => {
        clearAccessToken();
        setToken(null);
      },
    };
  }, [accessToken]);

  return <AuthContext.Provider value={api}>{children}</AuthContext.Provider>;
}


