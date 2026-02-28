import type { AttachmentsState } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { Loader, Group, Text, Tooltip, ActionIcon } from "@mantine/core";
import { IconRepeat, IconSquareCheck } from "@tabler/icons-react";

type AttachmentsStateKind = AttachmentsState["kind"];

export type UploadAttachmentsStatusProps = {
    attachmentsStateKind: AttachmentsStateKind
    onRetry: () => void
}

export function UploadAttachmentsStatus({ attachmentsStateKind, onRetry }: UploadAttachmentsStatusProps) {
    const contentMap = {
        error:
            <>
                <Tooltip label="Ошибка загрузки. Повторить">
                    <ActionIcon
                        size="sm"
                        variant="light"
                        color="red"
                        onClick={onRetry}
                    >
                        <IconRepeat size={14} />
                    </ActionIcon>
                </Tooltip>
            </>,

        loading:
            <>
                <Loader color="blue" size="sm" />
                <Text size="sm" c="blue">Отправляется…</Text>
            </>,

        loaded:
            <>
                <IconSquareCheck color="green" />
            </>,
        empty: null
    } satisfies Record<typeof attachmentsStateKind, React.ReactNode>;

    return (
        <Group miw={140} gap={4} justify="space-between">
            {contentMap[attachmentsStateKind]}
        </Group>
    );
}
