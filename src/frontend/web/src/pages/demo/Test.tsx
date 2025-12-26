import { AppShell, Box, Text } from "@mantine/core";

export function Test() {
    const headerH = 60;
    const footerH = 50;
    return (
        <AppShell
            padding={0} // отключаем дефолтные padding Mantine — все отступы контролируем вручную
            header={{ height: headerH }}
            footer={{ height: footerH }}
            styles={{
                root: {
                    height: "100vh",
                    display: "flex",
                    flexDirection: "column"
                },
                main: {
                    flex: 1,
                    minHeight: 0,
                    overflowY: "auto",
                    paddingTop: "var(--app-shell-header-offset)",
                    paddingBottom: "var(--app-shell-footer-offset)"
                }
            }}
        >
            {/* Header */}
            <AppShell.Header>
                <Box
                    style={{
                        height: "100%",
                        display: "flex",
                        alignItems: "center",
                        padding: "0 16px",
                        background: "#1c1c1c",
                        color: "#fff",
                    }}
                >
                    Header
                </Box>
            </AppShell.Header>

            <AppShell.Main>
                <Box style={{ padding: "0 16px" }}>
                    {Array.from({ length: 50 }).map((_, i) => (
                        <Text key={i}>Line {i + 1}</Text>
                    ))}
                </Box>
            </AppShell.Main>

            {/* Footer */}
            <AppShell.Footer>
                <Box
                    style={{
                        height: "100%",
                        display: "flex",
                        alignItems: "center",
                        background: "#1c1c1c",
                        color: "#fff",
                    }}
                >
                    Footer
                </Box>
            </AppShell.Footer>
        </AppShell>
    );
}
