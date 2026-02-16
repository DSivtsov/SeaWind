import { Alert, Button, Skeleton, Stack, Text } from "@mantine/core";
import { IconAlertCircle } from "@tabler/icons-react";
import type { ReactNode } from "react";

export type UiZoneState<T> =
    { kind: "empty" }
    | { kind: "loading" }
    | { kind: "error"; message: string }
    | { kind: "ready"; data: T }

type ZoneShellProps<T> = {
    state: UiZoneState<T>
    emptyView?: ReactNode
    loadingView: ReactNode
    error: (message: string) => ReactNode
    children: (data: T) => ReactNode
}

export function ZoneShell<T>({ state, emptyView, loadingView, error, children }: ZoneShellProps<T>) {
    if (state.kind === "empty") return emptyView ?? <ContentEmpty />;
    if (state.kind === "loading") return loadingView;
    if (state.kind === "error") return error(state.message);
    return children(state.data);
}

type ContentEmptyProps = {
    message?: string
}

export function ContentEmpty({ message }: ContentEmptyProps) {
    return (
        <Text>{message ?? "Данных нет"}</Text>);
}

type ContentErrorProps = {
    smallSize?: boolean
    title?: string
    message?: string
    onRetry?: () => void
}

export function ContentError({ smallSize = false, title, message, onRetry }: ContentErrorProps) {
    return (
        <Stack gap={smallSize ? "xs" : "md"}>
            <Alert
                icon={<IconAlertCircle size={16} />}
                title={title ?? "Ошибка"}
                color="red"
                variant="light"
            >
                <Text size={smallSize ? "xs" : "sm"}>
                    {message ?? "Не удалось загрузить данные."}
                </Text>
            </Alert>

            {onRetry && (
                <Button variant="light" onClick={onRetry}>
                    Повторить
                </Button>
            )}
        </Stack>
    );
}

type ContentSkeletonProps = {
    size?: "lg" | "md" | "xs"
}

export function ContentSkeleton({ size = "lg" }: ContentSkeletonProps) {
    const config = {
        lg: {
            title: 28,
            text: 16,
            block: 120,
            gap: "sm" as const,
        },
        md: {
            title: 20,
            text: 14,
            block: 80,
            gap: "xs" as const,
        },
        xs: {
            title: 10,
            text: 12,
            block: 20,
            gap: "xs" as const,
        },
    }[size];

    return (
        <Stack gap={config.gap}>
            <Skeleton height={config.title} width="40%" radius="sm" />
            {size !== "xs" &&
                <>
                    <Skeleton height={config.text} radius="sm" />
                    <Skeleton height={config.text} radius="sm" />
                    <Skeleton height={config.text} width="80%" radius="sm" />
                </>
            }
            <Skeleton height={config.block} radius="sm" />
        </Stack>
    );
}
