import { getCourse } from "@/pages/courses/layoutTabs/CourseLayoutTabsApi";
import type { CourseDto } from "@/pages/courses/list/CoursesApi";
import { useAuth } from "@/shared/auth/useAuth";
import { AppHeaderDefault } from "@/shared/layout/AppHeaderDefault";
import { AppFrame } from "@/shared/layout/AppFrame";
import { Text, Stack, Box, Grid, Skeleton, Group } from "@mantine/core";
import { useState, useEffect } from "react";
import { Outlet, useParams } from "react-router-dom";
import { PageShell } from "@/shared/PageShell";

type CourseLayoutState =
    | { kind: "loading" }
    | { kind: "error" }
    | { kind: "ready"; course: CourseDto };


export function CoursesLayoutTab() {
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

    const headerDefault = <AppHeaderDefault
        headerTitle="Лекции курса"
        headerDescription="Переходи к нужной"
        allCoursesOnClick={() => {
            console.log("[CoursesLayoutTab]: onClick [Все курсы]");
        }}
    />;

    return (
        <div className="layout-publicBg">
            <AppFrame header={headerDefault}>
                <Box p="lg" pos="sticky" top={0} className="layout-publicMainHeader">
                    <PageShell
                        state={courseLayoutState.kind}
                        loadingView={loadingView}
                        errorText="Проблема с сервером. Попробуйте позже."
                    >
                        {courseLayoutState.kind === "ready" && readyView(courseLayoutState.course)}
                    </PageShell>
                </Box>
                <Outlet />
            </AppFrame >
        </div >
    );
}

function readyView(course: CourseDto) {
    return <Grid gutter="xs">
        <Grid.Col span={{ base: 12, sm: 3 }}>
            <Group gap="sm">
                <Text size="xs" c="dimmed">Course code</Text>
                <Text fw={500}>{course.id}</Text>
            </Group>
        </Grid.Col>
        <Grid.Col span={{ base: 12, sm: 9 }}>
            <Group gap="sm">
                <Text size="xs" c="dimmed">Course Title</Text>
                <Text fw={500}>{course.title}</Text>
            </Group>
        </Grid.Col>
        <Grid.Col span={12}>
            <Group gap="sm">
                <Text size="xs" c="dimmed">Course description</Text>
                <Text fw={500}>{course.description ?? ""}</Text>
            </Group>
        </Grid.Col>
    </Grid>;
}

const loadingView = <Stack gap="xs">
    <Skeleton h={28} w={500} />
    <Skeleton h={52} />
</Stack>;

/* const errorView = <Stack gap={4}>
    <Title order={5}>Проблема с сервером...</Title>
    <Text c="dimmed">Не могу получить данные курса.</Text>
    <Text c="dimmed">Попробуйте позже.</Text>
</Stack>; */

