import { createOptimisticMessage, findMessageByClientGuid, getClientMessages, markMessageAsError, markMessageAsLoaded, markMessageAsLoading, type CreateOptimisticMessageParams }
    from "@/pages/courses/exerciseChat/chatMessages";
import {
    type ChatMessageUi, type ChatMessageDto, getExerciseChatMessages, type ChatAttachmentDto, postAttachmentMessage,
    type AttachmentsState,
    postExerciseChatMessage
} from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { MessageChatExercise } from "@/pages/courses/exerciseChat/MessageChatExercise";
import { UploadAttachmentsStatus } from "@/pages/courses/exerciseChat/UploadAttachmentsStatus";
import type { RedirectInfo } from "@/pages/courses/list/CoursesAccessDeniedModal";
import { httpError, isAbort, isApiError, type ApiError } from "@/shared/api/apiError";
import { useAuthContext } from "@/shared/auth/authContext";
import { ZoneShell, ContentSkeleton, ContentError, type UiZoneState } from "@/shared/components/ZoneShell";
import { Box, Button, Card, Divider, FileInput, Group, ScrollArea, Stack, Textarea, Title } from "@mantine/core";
import { IconSend } from "@tabler/icons-react";
import { useState, useCallback, useEffect, useRef } from "react";
import { Navigate, useLocation } from "react-router-dom";

type ChatExercisesProps = {
    exerciseId: string | undefined
    userId: string | null
};

export function ExerciseChat({ exerciseId, userId }: ChatExercisesProps) {
    const authCtx = useAuthContext();
    const location = useLocation();
    const token = authCtx.state.token;

    const controllerRef = useRef<AbortController | null>(null);
    const controllerUploadAttachmentsRef = useRef<AbortController | null>(null);
    const controllerSendMsgRef = useRef<AbortController | null>(null);
    const [newMessage, setNewMessage] = useState('');
    const [exerciseChatState, setExerciseChatState] = useState<UiZoneState<ChatMessageUi[]>>({ kind: "empty" });
    const bottomRef = useRef<HTMLDivElement | null>(null);

    const [files, setFiles] = useState<File[]>([]);
    const [attachmentsState, setAttachmentsState] = useState<AttachmentsState>({ kind: "empty", attachments: [] });
    const isAttachmentReadyToSend = attachmentsState.kind === "empty" || attachmentsState.kind === "loaded";

    const userRole = authCtx.me.kind === "ready" ? authCtx.me.user.role : null;
    const isStudentOrMentor = userRole && ["Student", "Mentor"].includes(userRole);

    const isExerciseChatReady = exerciseChatState.kind === "ready";
    const isSendActive = newMessage.trim().length > 0 && isExerciseChatReady && isAttachmentReadyToSend;
    const msgCount = isExerciseChatReady ? exerciseChatState.data.length : 0;

    const loadExerciseChat = useCallback(async () => {
        controllerRef.current?.abort();

        const abortController = new AbortController();
        controllerRef.current = abortController;

        if (!exerciseId || !userId || !token) {
            setExerciseChatState({ kind: "error", message: "Missing data or token" });
            return;
        }

        try {
            setExerciseChatState({ kind: "loading" });
            const studentId: string = userId;
            const chatServerMessages: ChatMessageDto[] = await getExerciseChatMessages(exerciseId, studentId, token,
                abortController.signal);

            const clientMessages: ChatMessageUi[] = getClientMessages(chatServerMessages);

            setExerciseChatState({ kind: "ready", data: clientMessages });

        } catch (e) {
            if (abortController.signal.aborted) return;
            setExerciseChatState({ kind: "error", message: (e as ApiError).message });
        }
    }, [exerciseId, userId, token]);

    const onRetryExerciseChat = loadExerciseChat;

    useEffect(() => {
        loadExerciseChat();
        return () => controllerRef.current?.abort();
    }, [loadExerciseChat]);

    useEffect(() => {
        if (isExerciseChatReady)
            bottomRef.current?.scrollIntoView({ behavior: "smooth" });

    }, [isExerciseChatReady, msgCount]);

    function startSendMessage(): AbortSignal {
        // тут abort НЕ обязателен, но безопасно на случай бага
        controllerSendMsgRef.current?.abort();

        const c = new AbortController();
        controllerSendMsgRef.current = c;
        return c.signal;
    }

    function cancelSendMessage(): void {
        controllerSendMsgRef.current?.abort();
        controllerSendMsgRef.current = null;
    }

    // (опционально) на уходе со страницы abort controllerSendMsgRef
    useEffect(() => {
        return () => {
            controllerSendMsgRef.current?.abort();
            controllerSendMsgRef.current = null;
        };
    }, []);

    function handleKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    }

    function handleSend() {
        const textNewMsg = newMessage.trim();
        if (!textNewMsg || !userId || !isExerciseChatReady || !userRole || !isAttachmentReadyToSend) return;

        const paramsNewChatMessage: CreateOptimisticMessageParams = {
            author: { userId, role: userRole },
            text: textNewMsg,
            attachments: attachmentsState.attachments
        };

        const newChatMessage = createOptimisticMessage(paramsNewChatMessage);

        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            return { kind: "ready", data: [...prev.data, newChatMessage] };
        });

        const signal = startSendMessage();

        pushToServer(newChatMessage, signal);

        setFiles([]);
        setNewMessage("");
    }

    function handleCancel(clientGuid: string) {
        console.log(`onCancelSend[${clientGuid}]`);
        cancelSendMessage();

        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            const filtered = prev.data.filter(m => m.clientGuid !== clientGuid);

            return { ...prev, data: filtered };
        });
    }

    async function pushToServer(chatMessage: ChatMessageUi, signal: AbortSignal) {
        console.log(`clientGuid[${chatMessage.clientGuid}] text[${chatMessage.text}] role[${chatMessage.author.role}]`);

        if (!exerciseId || !token)
            return;

        const guid = chatMessage.clientGuid;

        let upd: ChatMessageUi;
        try {
            const messageId = await postExerciseChatMessage(exerciseId, chatMessage, token, signal);

            upd = markMessageAsLoaded(chatMessage, messageId);
        } catch (e) {
            if (isAbort(e)) return;

            upd = markMessageAsError(chatMessage);
        }

        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            const updated = prev.data.map((msg) => msg.clientGuid !== guid ? msg : upd);

            return { ...prev, data: updated };
        });
    }

    function onRetrySendMessage(clientGuid: string) {
        console.log(`onRetrySendMessage[${clientGuid}]`);

        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            const updated = prev.data.map((msg) => msg.clientGuid !== clientGuid ? msg : markMessageAsLoading(msg));

            return { ...prev, data: updated };
        });

        const msg = findMessageByClientGuid(exerciseChatState, clientGuid);
        if (!msg) return;

        const signal = startSendMessage();

        pushToServer(msg, signal);
    };

    const uploadAttachments = useCallback(async (arrfiles: File[]): Promise<void> => {
        controllerUploadAttachmentsRef.current?.abort();

        const abortController = new AbortController();
        controllerUploadAttachmentsRef.current = abortController;
        const signal: AbortSignal = abortController.signal;

        if (arrfiles.length === 0) {
            setAttachmentsState({ kind: "empty", attachments: [] });
            return;
        }

        if (!exerciseId || !token)
            throw httpError("abort", "[postAttachmentMessage]: Missing data");

        setAttachmentsState({ kind: "loading" });
        try {

            const attachmentsLoaded: ChatAttachmentDto[] = await Promise.all(
                arrfiles.map(file => postAttachmentMessage(exerciseId, file, token, signal))
            );
            if (signal.aborted) return;

            setAttachmentsState({ kind: "loaded", attachments: attachmentsLoaded });
        }
        catch (err) {
            if (signal.aborted) return;

            if (isApiError(err)) {
                setAttachmentsState({ kind: "error", error: err });
                return;
            }

            setAttachmentsState({
                kind: "error",
                error: httpError("http", "[postAttachmentMessage]")
            });
        }

    }, [exerciseId, token]);

    const retryUploadAttachmentFiles = () => uploadAttachments(files);

    useEffect(() => {
        uploadAttachments(files);

        return () => controllerUploadAttachmentsRef.current?.abort();
    }, [files, uploadAttachments]);

    const handleFilesChange = (value: File[] | null) => {
        if (!value || value.length === 0) {
            setFiles([]);
            return;
        }

        setFiles(value);
    };

    // это проверка поломки guard т.к. он не должен пропустить на страницу другие роли
    if (!isStudentOrMentor) {
        const info: RedirectInfo = {
            reason: "invalid_state",
            fromLocation: location.pathname
        };
        return <Navigate to="/courses" replace state={info} />;
    }

    return (
        <Stack flex={1} mih={0} miw={0}>
            <Card withBorder flex={1} mih={0}>
                <Stack gap="sm">
                    <Title order={4}>Exercise Chat</Title>
                    <Divider />
                </Stack>

                <Box flex={1} mih={0}>
                    <ZoneShell state={exerciseChatState}
                        loadingView={<ContentSkeleton size="xs" />}
                        error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetryExerciseChat}
                            title="Ошибка. Не смог загрузить чат"
                        />}
                    >
                        {(data: ChatMessageUi[]) =>
                            <ScrollArea h="100%" type="auto">
                                <Stack gap="sm" p="sm">
                                    {data.map((msg) => <MessageChatExercise key={msg.clientGuid} msg={msg} currentUserId={userId}
                                        onRetrySend={() => onRetrySendMessage((msg.clientGuid))}
                                        onCancelSend={() => handleCancel(msg.clientGuid)} />)}
                                </Stack>
                                <div ref={bottomRef} />
                            </ScrollArea>}
                    </ZoneShell>
                </Box>

                <Stack gap="sm">
                    <Divider />
                    <Group justify="flex-start" gap="xs" align="center">
                        <FileInput
                            multiple
                            value={files}
                            onChange={handleFilesChange}
                            placeholder="Прикрепить файлы к сообщению"
                            clearable
                            maw={300}
                        />
                        <UploadAttachmentsStatus attachmentsStateKind={attachmentsState.kind} onRetry={retryUploadAttachmentFiles} />
                    </Group>
                    <Group align="end" gap="sm" wrap="nowrap">
                        <Textarea
                            placeholder="Введите сообщение"
                            minRows={2}
                            style={{ flex: 1 }}
                            value={newMessage}
                            onChange={(event) => setNewMessage(event.currentTarget.value)}
                            onKeyDown={handleKeyDown}
                        />
                        <Button
                            onClick={handleSend}
                            disabled={!isSendActive}
                            leftSection={<IconSend size={16} />}
                        >
                            Отправить
                        </Button>
                    </Group>
                </Stack>
                {/* Input disabled when ChatWriteRight != currentRole or Mark=2. */}
            </Card>
        </Stack>
    );
}
