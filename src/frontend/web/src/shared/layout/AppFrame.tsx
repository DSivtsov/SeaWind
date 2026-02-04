import { HEADER_HEIGHT_NARROW, HEADER_HEIGHT, FOOTER_HEIGHT } from "@/common/constants";
import { AppCtx } from "@/shared/layout/appCtx";
import { AppShell, Burger, Drawer } from "@mantine/core";
import { useDisclosure, useMediaQuery } from "@mantine/hooks";
import { useMemo, type ReactNode } from "react";
import type React from "react";

const APP_SHELL_STYLES = {
    root: {
        height: "100vh",
        display: "flex",
        flexDirection: "column",
        "--app-shell-border-color": "#2a3c62",
    },
    main: {
        flex: 1,
        minHeight: 0,
        overflowY: "auto",
        paddingTop: "var(--app-shell-header-offset)",
        paddingBottom: "var(--app-shell-footer-offset)",
    },
} as const;

export type AppFrameProps =
    React.PropsWithChildren<{
        header: ReactNode;
        navbar?: ReactNode;
        footer?: ReactNode;
    }>

export type AppFrameContext =
    {
        isNarrow: boolean,
    };

export function AppFrame({ header, navbar, footer, children }: AppFrameProps) {
    const [burgerOpened, { toggle }] = useDisclosure();
    const isNarrowCurrent = useMediaQuery("(max-width: 750px)");
    const WIDTH_MENU_NAVBAR = 200;
    const appCtx = useMemo<AppFrameContext>(() => {
        return { isNarrow: isNarrowCurrent };
    }, [isNarrowCurrent]);

    const headerHeight = isNarrowCurrent ? HEADER_HEIGHT_NARROW : HEADER_HEIGHT;
    const hasFooter = footer != null;

    const hasNavbarCommon = navbar != null && !isNarrowCurrent;
    const hasNavDrawer = navbar != null && isNarrowCurrent;

    return (
        <div className="layout-publicBg">
            <AppShell
                padding={0}// отключаем дефолтные padding Mantine — все отступы контролируем вручную
                header={{ height: headerHeight }}
                footer={hasFooter ? { height: FOOTER_HEIGHT } : undefined}
                navbar={hasNavbarCommon ? { width: WIDTH_MENU_NAVBAR, breakpoint: "xs" } : undefined}
                styles={APP_SHELL_STYLES}
            >
                <AppShell.Header >
                    {header}
                </AppShell.Header>

                <AppShell.Main>
                    <AppCtx.Provider value={appCtx}>
                        {hasNavDrawer &&
                            <Burger size="sm" opened={burgerOpened} onClick={toggle} />}
                        {children}
                    </AppCtx.Provider>
                </AppShell.Main>

                {hasNavbarCommon && (
                    <AppShell.Navbar className="layout-publicHeader">
                        {navbar}
                    </AppShell.Navbar>)}

                {hasFooter && (
                    <AppShell.Footer >
                        {footer}
                    </AppShell.Footer>)}
            </AppShell>
            {hasNavDrawer &&
                <Drawer opened={burgerOpened} onClose={toggle} size={WIDTH_MENU_NAVBAR} className="layout-publicDrawer">
                    {navbar}
                </Drawer>}
        </div >
    );
}
