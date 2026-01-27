import type { AppFrameContext } from "@/shared/layout/AppFrame";
import { createContext, useContext } from "react";

export const AppCtx = createContext<AppFrameContext | null>(null);

export function useAppCtx(): AppFrameContext {
    const appCtx = useContext(AppCtx);
    if (!appCtx) throw new Error("useAppCtx must be used within <AppFrame> (AppCtx.Provider)");
    return appCtx;
}
