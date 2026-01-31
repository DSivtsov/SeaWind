import type { ApiError } from "@/shared/api/apiError";
import type { UiState } from "@/shared/types/UiState";
import { Stack, Button, Text, Title } from '@mantine/core';
import { IconReload } from '@tabler/icons-react';

type PageShellProps = {
    title?: string;
    state: UiState;
    loadingView?: React.ReactNode;
    emptyView?: React.ReactNode;
    errorText: string;
    error?: ApiError
    onRetry?: () => void;
    children?: React.ReactNode;
};

export function PageShell(props: PageShellProps) {
    return (
        <Stack gap="sm">
            {props.title && <Title order={2}>{props.title}</Title>}
            {renderContent(props)}
        </Stack>
    );
}

const renderContent = (props: PageShellProps) => {
    const { state, loadingView, emptyView, errorText, error, onRetry, children } = props;
    switch (state) {
        case "loading":
            return loadingView ?? defaultLoadingView();
        case "empty":
            return emptyView ?? defaultEmptyView();
        case "error":
            return errorView(errorText, error, onRetry);
        case "ready":
            return children;
        default: {
            // Exhaustiveness guard: if UiState changes, TS will error here.
            const _never: never = state;
            return _never;
        }
    }
};

function errorView(errorText: string | undefined, error: ApiError | undefined, onRetry: (() => void) | undefined) {
    //console.log(`error?.correlationId[${error?.correlationId}]`);
    return <Stack gap="xs">
        <Text c="red.7">{errorText ?? "Что-то пошло не так."}</Text>

        {error && (
            <>
                {error.status !== undefined && (
                    <Text size="xs" c="dimmed">Code: {error.status}</Text>
                )}
                {error.correlationId && (
                    <Text size="xs" c="dimmed">Trace: {error.correlationId}</Text>
                )}
            </>
        )}

        {onRetry ? (
            <Button variant="filled" radius="xl" color="gray" rightSection={<IconReload size={14} />} onClick={onRetry}>
                Retry
            </Button>
        ) : null}
    </Stack>;
}

function defaultEmptyView(): React.ReactNode {
    return <Text>Здесь пока ничего нет.</Text>;
}

function defaultLoadingView(): React.ReactNode {
    return <Text>Загрузка…</Text>;
}

