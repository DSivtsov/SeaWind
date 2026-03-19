import { downloadAttachmentApi, type Attachment, type Message } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { useAuthContext } from "@/shared/auth/authContext";
import { ActionIcon, Box, Button, Card, Group, Stack, Text, Tooltip, UnstyledButton } from "@mantine/core";
import { IconCancel, IconPaperclip, IconRepeat } from "@tabler/icons-react";

const bubbleBgOther = "dark.6";
const bubbleBgMine = "blue.8";

const widthChat = 500;

export type MessageExerciseChatProps = {
    msg: Message
    currentUserId: string | null
    onRetrySend: () => void
    onCancelSend: () => void
}

async function downloadAttachment(attachment: Attachment, token: string): Promise<void> {
    try {
        const blob = await downloadAttachmentApi(attachment.id, token);

        const objectUrl = window.URL.createObjectURL(blob);
        const link = document.createElement("a");
        link.href = objectUrl;
        link.download = attachment.fileNameOriginal;
        document.body.appendChild(link);
        link.click();
        link.remove();

        setTimeout(() => {
            window.URL.revokeObjectURL(objectUrl);
        }, 1000);

    } catch (error) {
        console.log(error);
    }
}

function ShowMessage(msgText: string | null) {
    if (!msgText) return null;

    return <Text size="sm">{msgText}</Text>;
}

function ShowAttachment(attachments: Attachment[], token: string | null) {
    if (attachments.length === 0 || token === null) return null;

    return (
        <Stack gap={4}>
            {attachments.map((a) => (
                <UnstyledButton
                    key={a.id}
                    onClick={() => downloadAttachment(a, token)}
                >
                    <Tooltip label="Загрузить файл">
                        <Text size="xs" c="dimmed">
                            <IconPaperclip size={15} stroke={2} /> {a.fileNameOriginal}
                        </Text>
                    </Tooltip>
                </UnstyledButton>
            ))
            }
        </Stack >
    );
}

function diffDaysFromNow(date: Date) {
    const d = new Date(date);
    const now = new Date();

    d.setHours(0, 0, 0, 0);
    now.setHours(0, 0, 0, 0);

    // 0 - сегодня, 1 - вчера, 2 - позавчера и т.д.
    return Math.floor((now.getTime() - d.getTime()) / 86400000);
}

function formatTime24(date: Date) {
    return date.toLocaleTimeString([], {
        hour: "2-digit",
        minute: "2-digit",
        hour12: false
    });
}

function getDate(date: Date) {
    return date.toLocaleDateString([], {
        day: "2-digit",
        month: "2-digit"
    });
}

function formatChatDateAndTime(iso: string) {
    const d = new Date(iso);

    const daysDiff = diffDaysFromNow(d);

    switch (daysDiff) {
        case 0:
            return "Сегодня, " + formatTime24(d);
        case 1:
            return "Вчера, " + formatTime24(d);
        default:
            return `${getDate(d)}, ` + formatTime24(d);
    }
}

export function MessageExerciseChat({ msg, currentUserId, onRetrySend, onCancelSend }: MessageExerciseChatProps) {
    const authCtx = useAuthContext();
    const token = authCtx.state.token;
    const isMine = msg.authorId === currentUserId;
    const justify = isMine ? "flex-end" : "flex-start";
    const bubbleBg = isMine ? bubbleBgMine : bubbleBgOther;
    const showError = msg.statusUpload === "error";
    const showLoading = msg.statusUpload === "uploading";
    const formatedDateAndTime = formatChatDateAndTime(msg.createdAt);

    return (
        <>
            <Stack gap={2} pb="xs">
                <Group justify={justify}>
                    <Card key={msg.seq} withBorder radius="md" bg={bubbleBg} maw={widthChat * 0.8}>
                        <Stack gap={4}>
                            <Group justify="space-between">
                                <Text fw={600} size="sm">{msg.authorRole}</Text>
                                <Text size="xs" c="dimmed">{formatedDateAndTime}</Text>
                            </Group>
                            {ShowMessage(msg.text)}
                            {ShowAttachment(msg.attachments, token)}
                        </Stack>
                    </Card>
                </Group>
                <Box pr="xs">
                    {showLoading && (
                        <Group justify="flex-end">

                            <Text size="xs" c="dimmed" ta="right" pr="xs">
                                Отправляется…
                            </Text>
                            <Tooltip label="Отменить отправку">
                                <ActionIcon variant="subtle" color="red" size="xs" radius="xl" onClick={onCancelSend}>
                                    <IconCancel />
                                </ActionIcon>
                            </Tooltip>
                        </Group>
                    )}
                    {showError && (
                        <Group gap={6} justify="flex-end">
                            <Text size="xs" c="red.5">
                                Не отправлено
                            </Text>
                            <Button size="xs" variant="light" leftSection={<IconRepeat size={12} />} onClick={onRetrySend}>
                                Повторить
                            </Button>
                            <Tooltip label="Отменить отправку">
                                <ActionIcon variant="subtle" color="red" size="xs" radius="xl" onClick={onCancelSend}>
                                    <IconCancel />
                                </ActionIcon>
                            </Tooltip>
                        </Group>
                    )}
                </Box>
            </Stack>
        </>
    );
}

