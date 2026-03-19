import { putChangeStatusThreadAsync, type ExerciseChatDto, type StudentExerciseMark, type StudentExerciseStatus } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import type { DashboardChatInput } from "@/pages/courses/exerciseChat/ExerciseChatShell";
import { getMarkInfo, tooltipTextStudent, checkEnableSendOnCheck, tooltipTextMentor } from "@/pages/courses/exerciseChat/chatStatus";
import { Stack, Card, Badge, Divider, Button, SegmentedControl, Tooltip, Title } from "@mantine/core";
import { useEffect, useMemo, useRef, useState } from "react";
import { useAuthContext } from "@/shared/auth/authContext";

export type ExerciseChatStatusProps = {
    data: ExerciseChatDto
    dashboardChat: DashboardChatInput
    clientSeq: number;
    onReload: () => Promise<void>;
    isMentor: boolean;
};

export function ExerciseChatStatus({ data, dashboardChat, clientSeq, onReload, isMentor }: ExerciseChatStatusProps) {
    const authCtx = useAuthContext();
    const token = authCtx.state.token;
    const controllerChangeStatusRef = useRef<AbortController | null>(null);
    const [mark, setMark] = useState<StudentExerciseMark | null>(data.exercise.mark ?? null);

    const enabledButtonSendOnCheck = useMemo<boolean>(() => {
        return checkEnableSendOnCheck(dashboardChat.role, data.threadLocks, clientSeq);
    }, [dashboardChat.role, data.threadLocks, clientSeq]);

    useEffect(() => {
        return () => {
            controllerChangeStatusRef.current?.abort();
            controllerChangeStatusRef.current = null;
        };
    }, []);

    async function pushChangeStatusToServer(newStatus: StudentExerciseStatus): Promise<void> {
        controllerChangeStatusRef.current?.abort();

        const ac = new AbortController();
        controllerChangeStatusRef.current = ac;

        if (!token) return;

        try {
            const response = await putChangeStatusThreadAsync(data.exercise.threadId, newStatus, mark, token, ac.signal);

            await onReload();
            // С учетом того, что по бизнес логике после putChangeStatusThreadAsync делается onReload()
            //  не нужно использовать значение response.serverSeq для актуализации списка локальных сообщений (<>clientSeq),
            //  так как после onReload() оно будет перезаписано на актуальное значение с сервера.
            // Поэтому просто игнорируем response и не используем его для изменения состояния компонента.
            void response;

        } catch (e) {
            if (ac.signal.aborted) return;

            console.error("Failed to change status thread", e);
        }
    }

    function handleChangeStatus(): void {
        let newStatus: StudentExerciseStatus;
        if (isMentor) {
            newStatus = "OnStudent";
        }
        else { // Student
            newStatus = "OnMentor";

        }
        pushChangeStatusToServer(newStatus);
    }

    const { markText, buttonText } = getMarkInfo(data.exercise.mark);

    const IsFinished = mark === 2;
    return (
        <>
            {!isMentor &&
                <Card withBorder>
                    <Stack gap="sm">
                        <Stack align="flex-start" gap="xs">
                            <Title order={4}>Текущая оценка</Title>
                            <Badge variant="light">{markText}</Badge>
                        </Stack>
                        <Divider />
                        <Tooltip label={tooltipTextStudent} multiline
                            disabled={enabledButtonSendOnCheck || IsFinished} >
                            <Button fullWidth size="md"
                                onClick={handleChangeStatus}
                                disabled={!enabledButtonSendOnCheck}>
                                {buttonText}
                            </Button>
                        </Tooltip>
                    </Stack>
                </Card>}

            {isMentor &&
                <>
                    <Card withBorder>
                        <Stack gap="sm">
                            <Title order={4}>Оценка упражнения</Title>

                            <SegmentedControl
                                value={mark !== null ? String(mark) : ""}
                                onChange={(value) => setMark(value === '' ? null : Number(value) as StudentExerciseMark)}
                                fullWidth
                                data={[
                                    { label: "—", value: "" },
                                    { label: "0", value: "0" },
                                    { label: "1", value: "1" },
                                    { label: "2", value: "2" },
                                ]} />

                            <Tooltip label={tooltipTextMentor} multiline
                                disabled={enabledButtonSendOnCheck || IsFinished}>
                                <Button fullWidth size="md"
                                    disabled={!enabledButtonSendOnCheck}
                                    onClick={handleChangeStatus}>
                                    {IsFinished ? <>Упражнение полностью <br /> выполнено</> : <>Отправить результат<br />проверки</>}
                                </Button>
                            </Tooltip>
                        </Stack>
                    </Card>
                </>}
        </>);
}
