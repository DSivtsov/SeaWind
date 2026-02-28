import type { ChatAttachmentDto, ChatMessageUi } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { ActionIcon, Box, Button, Card, Group, Stack, Text, Tooltip } from "@mantine/core";
import { IconCancel, IconPaperclip, IconRepeat } from "@tabler/icons-react";

const bubbleBgOther = "dark.6";
const bubbleBgMine = "blue.8";

const widthChat = 500;

export type MessageChatExerciseProps = {
    msg: ChatMessageUi
    currentUserId: string | null
    onRetrySend: () => void
    onCancelSend: () => void
}

function showMessage(msgText: string | null) {
    if (!msgText) return null;

    return <Text size="sm">{msgText}</Text>;
}

function showAttachment(attachments: ChatAttachmentDto[]) {
    if (attachments.length === 0) return null;

    return (
        <Stack gap={4}>
            {attachments.map((a) => (
                <Text
                    key={a.attachmentId}
                    size="xs"
                    c="dimmed"
                    component="a"
                    href={a.url}
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    <IconPaperclip size={15} stroke={2} /> {a.fileName}
                </Text>
            ))}
        </Stack>
    );
}

export function MessageChatExercise({ msg, currentUserId, onRetrySend, onCancelSend }: MessageChatExerciseProps) {
    const isMine = msg.author.userId === currentUserId;
    const justify = isMine ? "flex-end" : "flex-start";
    const bubbleBg = isMine ? bubbleBgMine : bubbleBgOther;
    const showError = msg.statusUpload === "error";
    const showLoading = msg.statusUpload === "loading";


    return (
        <>
            <Stack gap={2} pb="xs">
                <Group justify={justify}>
                    <Card key={msg.clientGuid} withBorder radius="md" bg={bubbleBg} maw={widthChat * 0.8}>
                        <Stack gap={4}>
                            <Group justify="space-between">
                                <Text fw={600} size="sm">{msg.author.role}</Text>
                                <Text size="xs" c="dimmed">{msg.createdAt}</Text>
                            </Group>
                            {showMessage(msg.text)}
                            {showAttachment(msg.attachments)}
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

