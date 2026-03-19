import { getChatPerms, getChatTitleInfo } from "@/pages/courses/exerciseChat/chatMessages";
import { getExerciseChatData, type ExerciseChatDto } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { ExerciseChatContent } from "@/pages/courses/exerciseChat/ExerciseChatContent";
import { ExerciseChatStatus } from "@/pages/courses/exerciseChat/ExerciseChatStatus";
import { MentorTimer } from "@/pages/courses/exerciseChat/MentorTimer";
import type { RedirectInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";
import type { ApiError } from "@/shared/api/apiError";
import { useAuthContext } from "@/shared/auth/authContext";
import { ContentError, ZoneShell, type UiZoneState } from "@/shared/components/ZoneShell";
import { Box, Card, Divider, Skeleton, Stack, Title, Text } from "@mantine/core";
import React from "react";
import { useCallback, useEffect, useRef, useState } from "react";
import { useNavigate } from "react-router-dom";

export type ExerciseChatProps = {
    exerciseId: string | undefined
    dashboardChat: DashboardChatInput
};

export type ChatRole = "Student" | "Mentor";

export type DashboardChatInput =
    | { role: "Student"; studentId?: never }
    | { role: "Mentor"; studentId: string | null };

export const ExerciseChatShellmemo = React.memo(ExerciseChatShell);

function ExerciseChatShell({ exerciseId, dashboardChat }: ExerciseChatProps) {
    const authCtx = useAuthContext();
    const navigate = useNavigate();
    const token = authCtx.state.token;
    const controllerRef = useRef<AbortController | null>(null);
    const [exerciseChatDto, setExerciseChatDto] = useState<UiZoneState<ExerciseChatDto>>({ kind: "loading" });
    const [clientSeq, setClientSeq] = useState(0);

    const loadExerciseChatStatus = useCallback(async () => {
        controllerRef.current?.abort();
        const abortController = new AbortController();
        controllerRef.current = abortController;

        if (!exerciseId || !token) {
            setExerciseChatDto({ kind: "error", message: "Missing data or token" });
            return;
        }

        try {
            setExerciseChatDto({ kind: "loading" });
            const chatServerMessages: ExerciseChatDto = await getExerciseChatData(exerciseId, dashboardChat, token, abortController.signal);

            setExerciseChatDto({ kind: "ready", data: chatServerMessages });

        } catch (e) {
            if (abortController.signal.aborted) return;

            const err = e as ApiError;
            if (err.status === 400 || err.status === 404 || err.status === 409) {
                const info: RedirectInfo = {
                    reason: "exercise_chat_mentor_error",
                    fromLocation: location.pathname
                };
                console.error("Access denied to exercise chat. Redirecting...", { error: err.message });
                navigate("/courses", { replace: true, state: info });
                return;
            }
            setExerciseChatDto({ kind: "error", message: err.message });
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [exerciseId, token, dashboardChat]);

    const onRetryExerciseChatStatus = loadExerciseChatStatus;

    useEffect(() => {
        loadExerciseChatStatus();
        return () => controllerRef.current?.abort();
    }, [loadExerciseChatStatus]);

    const loadingView =
        <Box h="100%" pt="md"  >
            <Skeleton height="100%" radius="sm" />
        </Box>;

    let textTitle: string = "";
    if (exerciseChatDto.kind === "ready") {
        const chatPerms = getChatPerms(exerciseChatDto.data, dashboardChat.role);
        textTitle = getChatTitleInfo(chatPerms.canWrite, dashboardChat.role);
    }

    return (
        <>
            {/* CENTER: Chat  */}
            <Stack flex={1} mih={0} miw={0}>
                <Card withBorder flex={1} mih={0}>
                    <Stack gap="sm">
                        <Title order={3}>Exercise Chat</Title>
                        <Text size="sm" c="dimmed">{textTitle}</Text>
                        <Divider />
                    </Stack>
                    <ZoneShell<ExerciseChatDto> state={exerciseChatDto}
                        loadingView={loadingView}
                        error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetryExerciseChatStatus}
                            title="Ошибка. Не смог загрузить метаданные чата"
                        />}
                    >{(data) => (<ExerciseChatContent dashboardChat={dashboardChat} threadId={data.exercise.threadId}
                        chatPerms={getChatPerms(data, dashboardChat.role)} clientSeq={clientSeq} onClientSeqChange={setClientSeq} />)}
                    </ZoneShell>
                </Card>
            </Stack>

            {/* RIGHT: Status & MentorTimer */}
            <ZoneShell<ExerciseChatDto> state={exerciseChatDto}
                loadingView={loadingView}
                error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetryExerciseChatStatus}
                    title="Ошибка. Не смог загрузить метаданные чата"
                />}
            >{(data) =>
            (<Box style={{ width: 300, flex: "0 0 300px" }}>
                <Stack gap="md">
                    <ExerciseChatStatus data={data} dashboardChat={dashboardChat} clientSeq={clientSeq}
                        onReload={loadExerciseChatStatus} isMentor={dashboardChat.role === "Mentor"} />
                    {dashboardChat.role === "Mentor" &&
                        <MentorTimer />
                    }
                </Stack>
            </Box>)}
            </ZoneShell>
        </>
    );
}

