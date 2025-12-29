import { FOOTER_HEIGHT, HEADER_HEIGHT } from "@/common/constants";
import { CoursesListPage } from "@/pages/courses/list/CoursesListPage";
import { ActionIcon, AppShell, Avatar, Text, Button, Flex, Stack, Anchor } from "@mantine/core";

export function CoursesLayout() {

    return (
        <div className="layout-publicBg">
            <AppShell
                padding={0} // отключаем дефолтные padding Mantine — все отступы контролируем вручную
                header={{ height: HEADER_HEIGHT }} // высота нужна Mantine для расчёта header offset
                footer={{ height: FOOTER_HEIGHT }} // высота нужна Mantine для расчёта footer offset
                styles={{
                    root: {
                        height: "100vh", // фиксируем layout по высоте viewport (иначе main растёт по контенту)
                        display: "flex", // делаем корень flex-контейнером
                        flexDirection: "column", // вертикальная колонка: header / main / footer
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
                <AppShell.Header className="layout-publicHeader" >
                    <Flex h="100%" align="center" justify="space-between" px="xl">
                        <Stack gap={2}>
                            <Text c="gray.2" size="xl" fw={700}>Доступные лекции и упражнения</Text>
                            <Text c="gray.4" size="sm" fw={500}>Переходи к интересующим материалам курса</Text>
                        </Stack>
                        <Flex h="100%" justify="flex-end" align="center" gap="md" pr="xl">
                            <Button variant="filled" color="green"
                                onClick={() => {
                                    console.log("OnClick Registration");
                                }}>Registration</Button>

                            <Button variant="default"
                                onClick={() => {
                                    console.log("OnClick Login");
                                }}> Login</Button>

                            <ActionIcon variant="transparent" size="xl" radius="xl" disabled={true}
                                onClick={() => console.log("OnClick Avatar")}
                            >
                                <Avatar variant="outline" size="lg" radius="xl" />
                            </ActionIcon>
                        </Flex>
                    </Flex>
                </AppShell.Header>

                <AppShell.Main>
                    <CoursesListPage />
                </AppShell.Main>

                <AppShell.Footer className="layout-publicFooter" >
                    <Flex h="100%" align="center" justify="space-between" px="xl">
                        <Stack gap={2}>
                            <Text c="gray.2">© WorkshopCode 2025</Text>
                            <Text c="gray.6">
                                Связаться с администратором:{' '}
                                <Anchor c="green.5" href="mailto:admin@workshopcode.app">
                                    admin@workshopcode.app
                                </Anchor>
                            </Text>
                        </Stack>
                        <Button variant="filled" color="green">Support Chat</Button>
                    </Flex>
                </AppShell.Footer>
            </AppShell>
        </div>
    );
}
