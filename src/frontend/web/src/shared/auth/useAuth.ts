import type { AuthApi } from "@/shared/auth/AuthProvider";
import { createContext, useContext } from "react";

export const AuthContext = createContext<AuthApi | null>(null);

export function useAuth(): AuthApi {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
