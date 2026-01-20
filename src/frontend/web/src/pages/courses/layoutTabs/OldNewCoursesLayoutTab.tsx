import { FOOTER_HEIGHT, HEADER_HEIGHT } from "@/common/constants";
import { AvatarMenu } from "@/pages/avatar/AvatarMenu";
import { getCourse } from "@/pages/courses/layoutTabs/CourseLayoutTabsApi";
import type { CourseDto } from "@/pages/courses/list/CoursesApi";
import { useAuth } from "@/shared/auth/useAuth";
import { AppShell, Text, Button, Flex, Stack, Anchor, Box, Grid, Skeleton, Title, Group } from "@mantine/core";
import { useState, useEffect } from "react";
import { Outlet, useParams } from "react-router-dom";

type CourseLayoutState =
    | { kind: "loading" }
    | { kind: "error" }
    | { kind: "ready"; course: CourseDto };

export function OldNewCoursesLayoutTab() {
    const { courseId } = useParams<{ courseId: string }>();
    const auth = useAuth();
    const token = auth.state.accessToken ?? null;

    const [courseLayoutState, setCourseLayoutState] = useState<CourseLayoutState>({ kind: "loading" });

    useEffect(() => {
        if (!courseId || !token) {
            // Defensive: RouteGuard should prevent entering here without token.
            setCourseLayoutState({ kind: "error" });
            return;
        }
        const abortController = new AbortController();

        (async () => {
            try {
                setCourseLayoutState({ kind: "loading" });

                const course: CourseDto = await getCourse("", token, abortController.signal);

                setCourseLayoutState({ kind: "ready", course });
            } catch {
                if (abortController.signal.aborted) return;
                setCourseLayoutState({ kind: "error" });
            }
        })();

        return () => abortController.abort();
    }, [courseId, token]);

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
                            <Text c="gray.2" size="xl" fw={700}>Лекции курса</Text>
                            <Text c="gray.4" size="sm" fw={500}>Переходи к нужной</Text>
                        </Stack>
                        <Flex h="100%" justify="flex-end" align="center" gap="md" pr="xl">
                            <Button variant="filled" color="green"
                                onClick={() => {
                                    console.log("[CoursesLayoutTab]: onClick [Все курсы]");
                                }}>Все курсы</Button>

                            <AvatarMenu />
                        </Flex>
                    </Flex>
                </AppShell.Header>

                <AppShell.Main >
                    <CourseHead />
                    <Outlet />
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
        </div >
    );

    function CourseHead() {
        return <Box p="md" pos="sticky" top={0} className="layout-publicMainHeader">
            {courseLayoutState.kind === "loading" ? (
                <Stack gap="xs">
                    <Skeleton h={28} w={260} />
                    <Skeleton h={36} />
                    <Skeleton h={36} />
                    <Skeleton h={52} />
                </Stack>
            ) : courseLayoutState.kind === "error" ? (
                <Stack gap={4}>
                    <Title order={3}>Course Lectures</Title>
                    <Text c="dimmed">Проблема с сервером. Попробуйте позже.</Text>
                </Stack>
            ) : (
                <Grid gutter="xs">
                    <Grid.Col span={{ base: 12, sm: 3 }}>
                        {/* <TextInput label="Course Code" value={ui.course.code} readOnly /> */}
                        <Group gap="sm">
                            <Text size="xs" c="dimmed">Course code</Text>
                            <Text fw={500}>{courseLayoutState.course.id}</Text>
                        </Group>
                    </Grid.Col>
                    <Grid.Col span={{ base: 12, sm: 9 }}>
                        {/* <TextInput label="Course Title" value={ui.course.title} readOnly /> */}
                        <Group gap="sm">
                            <Text size="xs" c="dimmed">Course Title</Text>
                            <Text fw={500}>{courseLayoutState.course.title}</Text>
                        </Group>
                    </Grid.Col>
                    <Grid.Col span={12}>
                        <Group gap="sm">
                            <Text size="xs" c="dimmed">Course description</Text>
                            <Text fw={500}>{courseLayoutState.course.description ?? ""}</Text>
                        </Group>
                        {/* <TextInput label="Course description" value={ui.course.description ?? ""} readOnly /> */}
                    </Grid.Col>
                </Grid>
            )}
        </Box>;
    }
}
