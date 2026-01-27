import { getCourseById } from "@/pages/courses/layoutTabs/CourseLayoutTabsApi";
import type { CourseDto } from "@/pages/courses/list/CoursesApi";
import { useAuthContext } from "@/shared/auth/authContext";
import { AppHeaderDefault } from "@/shared/layout/AppHeaderDefault";
import { AppFrame } from "@/shared/layout/AppFrame";
import { Text, Stack, Box, Grid, Skeleton, Group, Tabs } from "@mantine/core";
import { useState, useEffect } from "react";
import { Outlet, useLocation, useParams } from "react-router-dom";
import { PageShell } from "@/shared/components/PageShell";
import { TabsNavLinkTab } from "@/shared/components/TabsNavLinkTab";

type CourseLayoutState =
    | { kind: "loading" }
    | { kind: "error" }
    | { kind: "ready"; course: CourseDto };

type CourseTab = "lectures" | "exercises" | "workshops";
const COURSE_TAB_ORDER: CourseTab[] = ["lectures", "exercises", "workshops"];

const COURSE_TABS: Record<CourseTab, { text: string }> = {
    lectures: { text: "Лекции" },
    exercises: { text: "Упражнения" },
    workshops: { text: "Семинары" },
};

function isCourseTab(value: string): value is CourseTab {
    return value in COURSE_TABS;
}

function TabLabel(props: { tab: CourseTab; activeTab: CourseTab }) {
    const isActive = props.activeTab === props.tab;
    const text = COURSE_TABS[props.tab].text;
    return (
        <Text fw={isActive ? 600 : undefined} c={isActive ? undefined : "dimmed"} size="sm">
            {text}
        </Text>
    );
}

type ReadyCourseViewProps = {
    course: CourseDto,
    activeTab: CourseTab,
};

function ReadyCourseView({ course, activeTab }: ReadyCourseViewProps) {
    const { id, title, description } = course;
    return (
        <Group justify="space-between">
            <Box maw={600}>
                <Grid gutter="xs">
                    <Grid.Col span={{ base: 12, sm: 4 }}>
                        <Group gap="sm">
                            <Text size="xs" c="dimmed">Course code</Text>
                            <Text fw={500}>{id}</Text>
                        </Group>
                    </Grid.Col>
                    <Grid.Col span={{ base: 12, sm: 8 }}>
                        <Group gap="sm">
                            <Text size="xs" c="dimmed">Course Title</Text>
                            <Text fw={500}>{title}</Text>
                        </Group>
                    </Grid.Col>
                    <Grid.Col span={12}>
                        <Group gap="sm">
                            <Text size="xs" c="dimmed">Course description</Text>
                            <Text fw={500}>{description ?? ""}</Text>
                        </Group>
                    </Grid.Col>
                </Grid>
            </Box>
            <Tabs variant="pills" radius="xs" color="indigo" value={activeTab} orientation="vertical" placement="right">
                <Tabs.List>
                    {COURSE_TAB_ORDER.map((item) => (
                        <TabsNavLinkTab key={item} tabsTabPropsValue={item} navLinkPropsTo={item}>
                            <TabLabel tab={item} activeTab={activeTab} />
                        </TabsNavLinkTab>
                    ))}
                </Tabs.List>
            </Tabs>
        </Group>
    );
}

const loadingCourseView = <Stack gap="xs">
    <Skeleton h={28} w={500} />
    <Skeleton h={52} />
</Stack>;

function getHeaderDefaultForActiveTab(activeTab: CourseTab) {
    return (
        <AppHeaderDefault
            headerTitle={COURSE_TABS[activeTab].text + " курса"}
            headerDescription="Переходи к нужной"
            allCoursesOnClick={() => {
                console.log("[CoursesLayoutTab]: onClick [Все курсы]");
            }}
        />);
}

export function CourseLayoutTabs() {
    const { courseId } = useParams<{ courseId: string }>();
    const authCtx = useAuthContext();
    const token = authCtx.state.token ?? null;

    const location = useLocation();
    const [activeTab, setActiveTab] = useState<CourseTab>("lectures");


    useEffect(() => {
        const last = location.pathname.split("/").at(-1);
        if (last && isCourseTab(last)) setActiveTab(last);
    }, [location.pathname]);

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

                const course: CourseDto = await getCourseById(courseId, token, abortController.signal);

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
            <AppFrame header={getHeaderDefaultForActiveTab(activeTab)}>
                <Box p="xs" pos="sticky" top={0} className="layout-publicMainHeader">
                    <PageShell
                        state={courseLayoutState.kind}
                        loadingView={loadingCourseView}
                        errorText="Проблема с сервером. Попробуйте позже."
                    >
                        {courseLayoutState.kind === "ready"
                            && <ReadyCourseView course={courseLayoutState.course} activeTab={activeTab} />}
                    </PageShell>
                </Box>
                <Outlet />
            </AppFrame >
        </div >
    );
}



/* const errorView = <Stack gap={4}>
    <Title order={5}>Проблема с сервером...</Title>
    <Text c="dimmed">Не могу получить данные курса.</Text>
    <Text c="dimmed">Попробуйте позже.</Text>
</Stack>; */

/* import { useLocation, useResolvedPath } from "react-router-dom";

export function DebugPaths() {
    const location = useLocation();              // текущий URL
    const base = useResolvedPath(".");           // route-контекст
    const lectures = useResolvedPath("lectures");
    const exercises = useResolvedPath("exercises");

    console.log("location =", location.pathname);
    console.log("base =", base.pathname);
    console.log("lectures =", lectures.pathname);
    console.log("exercises =", exercises.pathname);

    return null;
}
 */
