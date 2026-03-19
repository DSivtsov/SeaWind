import { useAuthContext } from "@/shared/auth/authContext";
import { AppFrame } from "@/shared/layout/AppFrame";
import {
    Stack, Box, Badge, Card, Divider, Group, ScrollArea, Tabs,
    Text, Image
} from "@mantine/core";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Navigate, useLocation, useNavigate, useParams, useSearchParams } from "react-router-dom";
import { AppHeaderSimple } from "@/shared/layout/AppHeaderSimple";
import {
    getExerciseContentBlock, getExerciseData, loadCode, type ExerciseContentBlockDto,
    type ExerciseDto
} from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { ContentError, ContentSkeleton, ZoneShell, type UiZoneState } from "@/shared/components/ZoneShell";
import { type ApiError } from "@/shared/api/apiError";
import { CodeHighlight, CodeHighlightAdapterProvider, createShikiAdapter } from '@mantine/code-highlight';
import { loadShiki } from '@/shared/functions/loadShiki';
import { ExerciseChatShellmemo, type DashboardChatInput, type ExerciseChatProps } from "@/pages/courses/exerciseChat/ExerciseChatShell";
import type { RedirectInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";

const heightExerciseContent = 160;
const shikiAdapter = createShikiAdapter(loadShiki);

export function ExerciseDashboard() {
    const location = useLocation();
    const navigate = useNavigate();

    const { exerciseId } = useParams();
    const authCtx = useAuthContext();
    const token = authCtx.state.token;
    const [searchParams] = useSearchParams();
    //на ExerciseDashboard неиспользуем, он прокидывается  в ExerciseChatShell для получения данных чата
    const studentId = searchParams.get("studentId");

    const [courseExerciseState, setCourseExerciseState] = useState<UiZoneState<ExerciseDto>>({ kind: "loading" });
    const [exerciseContentBlockState, setExerciseContentBlockState] = useState<UiZoneState<ExerciseContentBlockDto>>({ kind: "loading" });

    const controllerRef = useRef<AbortController | null>(null);

    const [codeContentBlockState, setCodeContentBlockState] = useState<UiZoneState<string | null>>({ kind: "empty" });
    const [picContentBlockState, setPicContentBlockState] = useState<UiZoneState<string | null>>({ kind: "empty" });

    const userRole = authCtx.me.kind === "ready" ? authCtx.me.user.role : null;
    const isStudentOrMentor = userRole === "Student" || userRole === "Mentor";

    function handleBack() {
        const from = location.state?.from as string | undefined;

        if (typeof from === "string" && from.length > 0) {
            navigate(from);
        } else {
            navigate("/courses"); // fallback
        }
    }

    const loadExerciseData = useCallback(async () => {
        controllerRef.current?.abort();

        const abortController = new AbortController();
        controllerRef.current = abortController;

        if (!exerciseId || !token) {
            // Defensive: RouteGuard should prevent entering here without token.
            setCourseExerciseState({ kind: "error", message: "Missing data or token" });
            setExerciseContentBlockState({ kind: "error", message: "Missing data or token" });
            return;
        }

        try {
            setCourseExerciseState({ kind: "loading" });

            const courseExercise: ExerciseDto = await getExerciseData(exerciseId, token, abortController.signal);
            setCourseExerciseState({ kind: "ready", data: courseExercise });
        } catch (e) {
            if (abortController.signal.aborted) return;

            const err = e as ApiError;
            if (err.status === 404) {
                setCourseExerciseState({ kind: "error", message: "404 — Failed to load exercise" });
                return;
            }

            setCourseExerciseState({ kind: "error", message: err.message });
        }

        try {
            setExerciseContentBlockState({ kind: "loading" });

            const exerciseContentBlockDto: ExerciseContentBlockDto = await getExerciseContentBlock(exerciseId, token,
                abortController.signal);
            setExerciseContentBlockState({ kind: "ready", data: exerciseContentBlockDto });

        } catch (e) {
            if (abortController.signal.aborted) return;

            const err = e as ApiError;
            if (err.status === 404) {
                setExerciseContentBlockState({ kind: "error", message: "404 — Failed to load exercise content" });
                return;
            }
            setExerciseContentBlockState({ kind: "error", message: err.message });
        }

    }, [exerciseId, token]);

    const onRetry = loadExerciseData;

    useEffect(() => {
        loadExerciseData();
        return () => controllerRef.current?.abort();
    }, [loadExerciseData]);

    const contentBlock = useMemo(() => {
        let picUrl: string | null = null;
        let codeUrl: string | null = null;

        if (exerciseContentBlockState.kind === "ready") {
            for (const b of exerciseContentBlockState.data.blocks) {
                if (b.kind === "Picture") picUrl = b.urlFile;
                if (b.kind === "Code") codeUrl = b.urlFile;
            }
        }
        return { picUrl, codeUrl };
    }, [exerciseContentBlockState]);

    useEffect(() => {
        const abortController = new AbortController();

        const run = async () => {
            if (!token) {
                setCodeContentBlockState({ kind: "empty" });
                setPicContentBlockState({ kind: "empty" });
                return;
            }
            // ставим loading только тем, что реально будем грузить/показывать
            if (contentBlock.codeUrl) setCodeContentBlockState({ kind: "loading" });
            else setCodeContentBlockState({ kind: "empty" });

            if (contentBlock.picUrl) setPicContentBlockState({ kind: "loading" });
            else setPicContentBlockState({ kind: "empty" });

            try {
                if (contentBlock.codeUrl) {
                    const code = await loadCode(contentBlock.codeUrl, abortController.signal);
                    if (abortController.signal.aborted) return;
                    setCodeContentBlockState({ kind: "ready", data: code });
                }

                if (contentBlock.picUrl) {
                    if (abortController.signal.aborted) return;
                    // сохраняем url для тега <img>, который сам уже будет грузить картинку.
                    setPicContentBlockState({ kind: "ready", data: contentBlock.picUrl });
                }
            } catch (e) {
                if (abortController.signal.aborted) return;
                const message = (e as ApiError).message;
                setCodeContentBlockState({ kind: "error", message });
                setPicContentBlockState({ kind: "error", message });
            }
        };

        void run();

        return () => abortController.abort();
    }, [contentBlock.codeUrl, contentBlock.picUrl, token]);

    const headerSimple =
        <AppHeaderSimple
            headerTitle="Exercise Dashboard"
            headerDescription="For Student and Mentor Only"
            exitBackOnClick={handleBack}
        />;

    const showTitleVm = (data: ExerciseDto) =>
        <Stack gap="xs">
            <Badge variant="outline">Exercise #{data.orderNo}</Badge>
            <Text size="sm" c="dimmed">
                {data.title}
            </Text>
        </Stack>;

    const showExerciseDetail = (data: ExerciseContentBlockDto) =>
        <ScrollArea h="100%" type="auto">
            <Text size="sm" c="dimmed" style={{ whiteSpace: "pre-line" }} >
                {data.details}
            </Text>
        </ScrollArea>;

    const showCodeContentBlock = (data: string | null) =>
        <ScrollArea h={heightExerciseContent} type="auto">
            {data && <CodeHighlight code={data} language="csharp" radius="md" />}
        </ScrollArea>;

    const showPicContentBlock = (data: string | null) =>
        <>
            {data && <Image src={data} fit="contain" h={heightExerciseContent} />}
        </>;

    const exerciseChatProps = useMemo<ExerciseChatProps>(() => {
        // Ментор должен открывать только если есть studentId, иначе не понятно чей чат открывать.
        const dashboardChat: DashboardChatInput = userRole === "Student" ? { role: "Student" } : { role: "Mentor", studentId: studentId };
        return {
            exerciseId: exerciseId,
            dashboardChat
        };
    }, [userRole, studentId, exerciseId]);

    // это проверка поломки guard т.к. он не должен пропустить на страницу другие роли
    // также он превращает type Role | null -> ChatRole
    if (!isStudentOrMentor) {
        const info: RedirectInfo = {
            reason: "invalid_state",
            fromLocation: location.pathname
        };
        return <Navigate to="/courses" replace state={info} />;
    }

    return (
        <div className="layout-publicBg">
            <AppFrame header={headerSimple} >
                <CodeHighlightAdapterProvider adapter={shikiAdapter}>
                    <Group align="stretch" wrap="nowrap" gap="md" h="100%" p="md">
                        {/* LEFT: Exercise */}
                        <Stack flex={1} mih={0} miw={0}>
                            <Card withBorder>
                                <ZoneShell<ExerciseDto> state={courseExerciseState}
                                    loadingView={<ContentSkeleton size="xs" />}
                                    error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetry}
                                        title="Ошибка. Не удалось загрузить данные задания."
                                    />}
                                >
                                    {showTitleVm}
                                </ZoneShell>
                            </Card >

                            <Card withBorder flex={1} mih={0} >
                                <Stack gap="sm" pb="sm">
                                    <Text fw={600}>Описание задания</Text>
                                    <Divider />
                                </Stack>
                                <Box flex={1} mih={0}>
                                    <ZoneShell state={exerciseContentBlockState}
                                        loadingView={<ContentSkeleton size="xs" />}
                                        error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetry}
                                            title="Ошибка. Не смог загрузить подробное описание задания"
                                        />}
                                    >
                                        {showExerciseDetail}
                                    </ZoneShell>
                                </Box>
                            </Card>

                            <Card withBorder>
                                <Stack gap="sm">
                                    <Text fw={600}>Дополнительная информация</Text>

                                    <Tabs defaultValue="code" keepMounted={false}>
                                        <Tabs.List>
                                            <Tabs.Tab value="code">Code</Tabs.Tab>
                                            <Tabs.Tab value="picture">Picture</Tabs.Tab>
                                        </Tabs.List>

                                        <Tabs.Panel value="code" pt="sm">
                                            <Card withBorder radius="md">
                                                <ScrollArea type="auto"  >
                                                    <ZoneShell<string | null> state={codeContentBlockState}
                                                        loadingView={<ContentSkeleton size="xs" />}
                                                        error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetry}
                                                            title="Ошибка. Не смог загрузить данные задания"
                                                        />}
                                                    >
                                                        {showCodeContentBlock}
                                                    </ZoneShell>
                                                </ScrollArea>
                                            </Card>
                                        </Tabs.Panel>

                                        <Tabs.Panel value="picture" pt="sm">
                                            <Card withBorder radius="md">
                                                <ZoneShell<string | null> state={picContentBlockState}
                                                    loadingView={<ContentSkeleton size="xs" />}
                                                    error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetry}
                                                        title="Ошибка. Не смог загрузить данные задания"
                                                    />}
                                                >
                                                    {showPicContentBlock}
                                                </ZoneShell>
                                            </Card>
                                        </Tabs.Panel>
                                    </Tabs>
                                </Stack>
                            </Card>
                        </Stack >

                        {/* CENTER: Chat  and RIGHT: Status & Time */}
                        <ExerciseChatShellmemo {...exerciseChatProps} />
                    </Group >
                </CodeHighlightAdapterProvider >
            </AppFrame >
        </div >
    );
}


