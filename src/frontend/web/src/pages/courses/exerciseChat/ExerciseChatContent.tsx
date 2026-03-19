import {
    createOptimisticMessage, findMessageBySelectedSeq, toClientMessages, markMessageAsError, updatedLoadedMessage,
    markMessageAsLoading, type ChatPerms, type CreateOptimisticMessageParams,
    getMaxServerSeq
} from "@/pages/courses/exerciseChat/chatMessages";
import {
    type Message, type MessageDto, getExerciseChatMessages, type Attachment, postMessageAttachments,
    type AttachmentsState, postExerciseChatMessage,
    type MessageUploadRequest,
    type MessageUploadResponse,
    getMissedExerciseChatMessages
} from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import type { DashboardChatInput } from "@/pages/courses/exerciseChat/ExerciseChatShell";
import { MessageExerciseChat } from "@/pages/courses/exerciseChat/MessageExerciseChat";
import { UploadAttachmentsStatus } from "@/pages/courses/exerciseChat/UploadAttachmentsStatus";
import { httpError, isAbort, isApiError, type ApiError } from "@/shared/api/apiError";
import { useAuthContext } from "@/shared/auth/authContext";
import { ZoneShell, ContentSkeleton, ContentError, type UiZoneState } from "@/shared/components/ZoneShell";
import { Box, Button, Divider, FileInput, Group, ScrollArea, Stack, Textarea, Tooltip } from "@mantine/core";
import { IconSend } from "@tabler/icons-react";
import { useState, useCallback, useEffect, useRef } from "react";

type ExerciseChatContentProps = {
    dashboardChat: DashboardChatInput
    threadId: string
    chatPerms: ChatPerms
    clientSeq: number;
    onClientSeqChange: (value: number) => void;
};

export function ExerciseChatContent({ dashboardChat, threadId, chatPerms, clientSeq, onClientSeqChange }: ExerciseChatContentProps) {
    const authCtx = useAuthContext();
    const token = authCtx.state.token;
    const userId = authCtx.me.kind === "ready" ? authCtx.me.user.userId : null;

    const controllerRef = useRef<AbortController | null>(null);
    const controllerUploadAttachmentsRef = useRef<AbortController | null>(null);
    const controllerSendMsgRef = useRef<AbortController | null>(null);
    const [newMessage, setNewMessage] = useState('');
    const [exerciseChatState, setExerciseChatState] = useState<UiZoneState<Message[]>>({ kind: "empty" });
    const bottomRef = useRef<HTMLDivElement | null>(null);

    const [files, setFiles] = useState<File[]>([]);
    const [attachmentsState, setAttachmentsState] = useState<AttachmentsState>({ kind: "empty", attachments: [] });

    const missedAbortRef = useRef<AbortController | null>(null);
    const missedLoadInFlightRef = useRef(false);

    const isAttachmentReadyToSend = attachmentsState.kind === "empty" || attachmentsState.kind === "loaded";

    const isExerciseChatReady = exerciseChatState.kind === "ready";
    const isSendReady = newMessage.trim().length > 0 && isExerciseChatReady && isAttachmentReadyToSend;
    const msgCount = isExerciseChatReady ? exerciseChatState.data.length : 0;

    const [isMessageSending, setIsMessageSending] = useState<boolean>(false);

    const loadExerciseChat = useCallback(async () => {
        controllerRef.current?.abort();

        const abortController = new AbortController();
        controllerRef.current = abortController;

        if (!threadId || !token) {
            setExerciseChatState({ kind: "error", message: "Missing data or token" });
            return;
        }

        try {
            setExerciseChatState({ kind: "loading" });
            const chatServerMessages: MessageDto[] = await getExerciseChatMessages(threadId, token, abortController.signal);

            onClientSeqChange(getMaxServerSeq(chatServerMessages));
            const clientMessages: Message[] = toClientMessages(chatServerMessages);

            //не используем состояние "empty"
            /* if (chatServerMessages.length === 0) setExerciseChatState({ kind: "empty" }); */
            setExerciseChatState({ kind: "ready", data: clientMessages });

        } catch (e) {
            if (abortController.signal.aborted) return;
            setExerciseChatState({ kind: "error", message: (e as ApiError).message });
        }
    }, [threadId, token, onClientSeqChange]);

    const onRetryExerciseChat = loadExerciseChat;

    useEffect(() => {
        loadExerciseChat();
        return () => controllerRef.current?.abort();
    }, [loadExerciseChat]);

    useEffect(() => {
        if (isExerciseChatReady)
            bottomRef.current?.scrollIntoView({ behavior: "smooth" });

    }, [isExerciseChatReady, msgCount]);

    function getAbortSignal_SendMessage(): AbortSignal {
        controllerSendMsgRef.current?.abort();

        const c = new AbortController();
        controllerSendMsgRef.current = c;
        return c.signal;
    }

    function cancelSendMessage(): void {
        controllerSendMsgRef.current?.abort();
        controllerSendMsgRef.current = null;
        setIsMessageSending(false);
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
            handleSendMessage();
        }
    }

    function handleSendMessage() {
        const textNewMsg = newMessage.trim();
        if (!textNewMsg || !userId || !isExerciseChatReady || !dashboardChat || !isAttachmentReadyToSend) return;

        if (isMessageSending) return;
        setIsMessageSending(true);

        const paramsNewChatMessage: CreateOptimisticMessageParams = {
            threadId: threadId,
            authorId: userId,
            authorRole: dashboardChat.role,
            text: textNewMsg,
            attachments: [...attachmentsState.attachments]
        };

        const nextSeq = clientSeq + 1;
        onClientSeqChange(nextSeq);
        const newChatMessage = createOptimisticMessage(paramsNewChatMessage, nextSeq);

        // сброс всех данных сообщения и вложений, которые отправляются на сервер, в UI,
        //  так как мы уже создали оптимистичное сообщение с этими данными
        setAttachmentsState({ kind: "empty", attachments: [] });
        setFiles([]);
        setNewMessage("");

        // оптимистично добавляем сообщение в UI, данное сообщение помечено как "отправляется"
        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            return { kind: "ready", data: [...prev.data, newChatMessage] };
        });

        // начинаем процесс отправки сообщения на сервер
        const signal = getAbortSignal_SendMessage();
        pushToServer(newChatMessage, signal);
    }

    function handleCancel(selectedSeq: number) {
        cancelSendMessage();

        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            const filtered = prev.data.filter(m => m.seq !== selectedSeq);

            return { ...prev, data: filtered };
        });
    }

    async function pushToServer(chatMessage: Message, signal: AbortSignal) {
        if (!threadId || !token) return;

        const tmpSeq = chatMessage.seq;

        const uploadingRequest: MessageUploadRequest = {
            clientSeq: tmpSeq,
            text: chatMessage.text,
            attachmentIds: chatMessage.attachments.map((a) => a.id)
        };

        let updatedMessage: Message;
        let response: MessageUploadResponse | null = null;
        try {
            response = await postExerciseChatMessage(threadId, uploadingRequest, token, signal);
            updatedMessage = updatedLoadedMessage(chatMessage, response);
            setIsMessageSending(false);
        } catch (e) {
            if (isAbort(e)) return;

            updatedMessage = markMessageAsError(chatMessage);
        }

        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            const updated = prev.data.map((msg) => msg.seq !== tmpSeq ? msg : updatedMessage);

            return { ...prev, data: updated };
        });

        if (response !== null && tmpSeq !== response.serverSeq) {
            // На сервере есть сообщения загруженные до нашего оптимистичного
            void loadMissedMessages(tmpSeq, response.serverSeq);
        }
    }

    function onRetrySendMessage(selectedSeq: number) {
        console.log(`onRetrySendMessage[${selectedSeq}]`);

        setExerciseChatState((prev) => {
            if (prev.kind !== "ready") return prev;

            const updated = prev.data.map((msg) => msg.seq !== selectedSeq ? msg : markMessageAsLoading(msg));

            return { ...prev, data: updated };
        });

        const msg = findMessageBySelectedSeq(exerciseChatState, selectedSeq);
        if (!msg) return;

        const signal = getAbortSignal_SendMessage();

        pushToServer(msg, signal);
    };

    const uploadAttachments = useCallback(async (uploadfiles: File[]): Promise<void> => {
        controllerUploadAttachmentsRef.current?.abort();

        const abortController = new AbortController();
        controllerUploadAttachmentsRef.current = abortController;
        const signal: AbortSignal = abortController.signal;

        if (uploadfiles.length === 0) {
            setAttachmentsState({ kind: "empty", attachments: [] });
            return;
        }

        if (!threadId || !token)
            throw httpError("abort", "[postAttachmentMessage]: Missing data");

        setAttachmentsState({ kind: "loading" });
        try {
            const attachmentsLoaded: Attachment[] = await postMessageAttachments(threadId, uploadfiles, token, signal);

            if (signal.aborted) {
                setAttachmentsState({ kind: "empty", attachments: [] });
                setFiles([]);
                return;
            }

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

    }, [threadId, token]);

    const retryUploadAttachmentFiles = () => uploadAttachments(files);

    useEffect(() => {
        uploadAttachments(files);

        return () => controllerUploadAttachmentsRef.current?.abort();
    }, [files, uploadAttachments]);

    const handleFilesChange = (selectedFiles: File[] | null) => {
        if (!selectedFiles || selectedFiles.length === 0) {
            setFiles([]);
            return;
        }

        setFiles(selectedFiles);
    };

    async function loadMissedMessages(clientSeq: number, serverSeq: number) {
        missedAbortRef.current?.abort();
        missedAbortRef.current = new AbortController();

        if (missedLoadInFlightRef.current || !threadId || !token)
            return;
        missedLoadInFlightRef.current = true;

        try {

            const chatServerMessages: MessageDto[] = await getMissedExerciseChatMessages(threadId, clientSeq, serverSeq,
                token, missedAbortRef.current.signal);

            const missedMessages: Message[] = toClientMessages(chatServerMessages);

            setExerciseChatState((prev) => {
                if (prev.kind !== "ready") return prev;

                const updated = [...prev.data, ...missedMessages].sort((a, b) => a.seq - b.seq);

                return { ...prev, data: updated };
            });

        } catch (e) {
            if (missedAbortRef.current?.signal.aborted) return;

            setExerciseChatState({ kind: "error", message: (e as ApiError).message });
        }
        finally {
            missedLoadInFlightRef.current = false;
        }
    }

    useEffect(() => {
        return () => {
            missedAbortRef.current?.abort();
        };
    }, []);

    const placeHolderMessageText = dashboardChat.role === "Student" ?
        "Опишите ваш вопрос или отправляемое решение"
        : "Ответьте на вопрос или дайте комментарий к проверяемому решению";    //Mentor

    return (
        <>
            <Box flex={1} mih={0}>
                <ZoneShell state={exerciseChatState}
                    loadingView={<ContentSkeleton size="lg" />}
                    error={(msg) => <ContentError smallSize={true} message={msg} onRetry={onRetryExerciseChat}
                        title="Ошибка. Не смог загрузить чат"
                    />}
                >
                    {(data: Message[]) =>
                        <ScrollArea h="100%" type="auto">
                            <Stack gap="sm" p="sm">
                                {data.map((msg) => <MessageExerciseChat key={msg.seq} msg={msg} currentUserId={userId}
                                    onRetrySend={() => onRetrySendMessage((msg.seq))}
                                    onCancelSend={() => handleCancel(msg.seq)} />)}
                            </Stack>
                            <div ref={bottomRef} />
                        </ScrollArea>}
                </ZoneShell>
            </Box>

            {chatPerms.canWrite && <Stack gap="sm">
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
                        placeholder={placeHolderMessageText}
                        rows={5}
                        style={{ flex: 1 }}
                        value={newMessage}
                        onChange={(event) => setNewMessage(event.currentTarget.value)}
                        onKeyDown={handleKeyDown}
                    />
                    <Tooltip
                        disabled={isSendReady}
                        label="Дождитесь загрузки вложений и введите текст сообщения"
                        openDelay={300}
                    >
                        <span>
                            <Button
                                onClick={handleSendMessage}
                                disabled={!isSendReady || isMessageSending}
                                leftSection={<IconSend size={16} />}
                            >
                                Отправить
                            </Button>
                        </span>
                    </Tooltip>
                </Group>
            </Stack>}
        </>
    );
}
