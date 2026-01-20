import { HEADER_HEIGHT_NARROW, HEADER_HEIGHT, FOOTER_HEIGHT } from "@/common/constants";
import { AppFooterDefault } from "@/shared/layout/AppFooterDefault";
import { AppShell } from "@mantine/core";
import { useMediaQuery } from "@mantine/hooks";
import type { ReactNode } from "react";
import type React from "react";

export type AppFrameProps =
    React.PropsWithChildren<{
        header: ReactNode;
        footer?: ReactNode
    }>

export function AppFrame({ header, footer = <AppFooterDefault />, children }: AppFrameProps) {

    const isNarrow = useMediaQuery("(max-width: 650px)");

    return (
        <div className="layout-publicBg">
            <AppShell
                padding={0} // отключаем дефолтные padding Mantine — все отступы контролируем вручную
                // высота нужна Mantine для расчёта header offset
                // высота разная для разной ширины экрана
                header={{ height: isNarrow ? HEADER_HEIGHT_NARROW : HEADER_HEIGHT }}
                footer={{ height: FOOTER_HEIGHT }} // высота нужна Mantine для расчёта footer offset
                //withBorder={false}
                styles={{
                    root: {
                        height: "100vh", // фиксируем layout по высоте viewport (иначе main растёт по контенту)
                        display: "flex", // делаем корень flex-контейнером
                        flexDirection: "column", // вертикальная колонка: header / main / footer
                        "--app-shell-border-color": "#2a3c62",
                    },
                    main: {
                        flex: 1, // main занимает всё оставшееся место между header и footer
                        minHeight: 0, // критично для flex: позволяет main сжиматься и включать overflow
                        overflowY: "auto", // скролл ТОЛЬКО внутри main

                        // Компенсация overlay header/footer. Offsets рассчитываются Mantine автоматически.
                        // Контентные отступы (pt/pb/px) задаются на уровне страницы (CoursesListPage).
                        paddingTop: "var(--app-shell-header-offset)",
                        paddingBottom: "var(--app-shell-footer-offset)",
                    },
                }}
            >
                <AppShell.Header>
                    {header}
                </AppShell.Header>

                <AppShell.Main>
                    {children}
                </AppShell.Main>

                <AppShell.Footer >
                    {footer}
                </AppShell.Footer>
            </AppShell>
        </div >
    );
}
