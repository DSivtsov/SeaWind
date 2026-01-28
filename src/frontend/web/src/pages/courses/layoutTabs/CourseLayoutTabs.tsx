import { getCourseById } from "@/pages/courses/layoutTabs/CourseLayoutTabsApi";
import type { CourseDto } from "@/pages/courses/list/CoursesApi";
import { useAuthContext } from "@/shared/auth/authContext";
import { AppHeaderDefault } from "@/shared/layout/AppHeaderDefault";
import { AppFrame } from "@/shared/layout/AppFrame";
import { Stack, Box, Skeleton } from "@mantine/core";
import { useState, useEffect } from "react";
import { Outlet, useLocation, useNavigate, useParams } from "react-router-dom";
import { PageShell } from "@/shared/components/PageShell";
import { ReadyCourseView, } from "@/pages/courses/layoutTabs/ReadyCourseView";
import { getActiveTabText, isCourseTab, type CourseTab } from "./COURSE_TABS";
import { httpError, type ApiError } from "@/shared/api/apiError";

type CourseLayoutState =
    | { kind: "loading" }
    | { kind: "error"; error: ApiError }
    | { kind: "ready"; course: CourseDto };

const loadingCourseView = <Stack gap="xs">
    <Skeleton h={28} w={500} />
    <Skeleton h={52} />
</Stack>;

export function CourseLayoutTabs() {

    const { courseId } = useParams<{ courseId: string }>();
    const authCtx = useAuthContext();
    const token = authCtx.state.token ?? null;

    const location = useLocation();
    const navigate = useNavigate();
    const [activeTab, setActiveTab] = useState<CourseTab>("lectures");

    useEffect(() => {
        const last = location.pathname.split("/").at(-1);
        if (last && isCourseTab(last)) setActiveTab(last);
    }, [location.pathname]);

    const [courseLayoutState, setCourseLayoutState] = useState<CourseLayoutState>({ kind: "loading" });

    useEffect(() => {
        if (!courseId || !token) {
            // Defensive: RouteGuard should prevent entering here without token.
            setCourseLayoutState({ kind: "error", error: httpError("parse", "Missing courseId or token"), });
            return;
        }
        const abortController = new AbortController();

        (async () => {
            try {
                setCourseLayoutState({ kind: "loading" });

                const course: CourseDto = await getCourseById(courseId, token, abortController.signal);

                setCourseLayoutState({ kind: "ready", course });
            } catch (e) {
                if (abortController.signal.aborted) return;
                setCourseLayoutState({ kind: "error", error: e as ApiError });
            }
        })();

        return () => abortController.abort();
    }, [courseId, token]);

    const headerDefaultForActiveTab =
        <AppHeaderDefault
            headerTitle={getActiveTabText(activeTab) + " курса"}
            headerDescription="Переходи к нужной"
            allCoursesOnClick={() => {
                navigate("/courses");
            }}
        />;

    return (
        <div className="layout-publicBg">
            <AppFrame header={headerDefaultForActiveTab}>
                <Box p="xs" pos="sticky" top={0} className="layout-publicMainHeader">
                    <PageShell
                        state={courseLayoutState.kind}
                        loadingView={loadingCourseView}
                        errorText="Проблема с сервером. Не могу получить информацию о курсе."
                        error={courseLayoutState.kind === "error" ? courseLayoutState.error : undefined}
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
