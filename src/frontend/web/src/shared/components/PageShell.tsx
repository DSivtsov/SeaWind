import type { UiState } from "@/shared/types/UiState";
import { Stack, Button, Text, Title } from '@mantine/core';
import { IconReload } from '@tabler/icons-react';

type PageShellProps = {
    title?: string;
    state: UiState;
    loadingView?: React.ReactNode;
    emptyView?: React.ReactNode;
    errorText?: string;
    onRetry?: () => void;
    children?: React.ReactNode;
};

export function PageShell(props: PageShellProps) {
    const { title, state, loadingView, emptyView, errorText, onRetry, children } = props;
    return (
        <Stack gap="sm">
            {title && <Title order={2}>{title}</Title>}
            {renderContent(state, loadingView, emptyView, errorText, onRetry, children)}
        </Stack>
    );
}

const renderContent = (state: UiState, loadingView?: React.ReactNode, emptyView?: React.ReactNode, errorText?: string,
    onRetry?: () => void, children?: React.ReactNode) => {
    switch (state) {
        case "loading":
            return loadingView ?? defaultLoadingView();
        case "empty":
            return emptyView ?? defaultEmptyView();
        case "error":
            return errorView(errorText, onRetry);
        case "ready":
            return children;
        default: {
            // Exhaustiveness guard: if UiState changes, TS will error here.
            const _never: never = state;
            return _never;
        }
    }
};

function errorView(errorText: string | undefined, onRetry: (() => void) | undefined) {
    return <Stack gap="xs">
        <Text c="red.7">
            {errorText ?? "Что-то пошло не так."}
        </Text>
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

