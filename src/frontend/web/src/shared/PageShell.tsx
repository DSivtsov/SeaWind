import type { UiState } from "@/shared/UiState";
import { Stack, Button, Text } from '@mantine/core';
import { IconReload } from '@tabler/icons-react';

type PageShellProps = {
    state: UiState;
    errorText?: string;
    onRetry?: () => void;
    children?: React.ReactNode;
};

const renderContent = (state: UiState, errorText?: string, onRetry?: () => void, children?: React.ReactNode) => {
    switch (state) {
        case "loading":
            return <Text>Loading…</Text>;
        case "empty":
            return <Text>Nothing here yet.</Text>;
        case "error":
            return (
                <Stack gap="xs">
                    <Text c="red.7">
                        {errorText ?? "Something went wrong."}
                    </Text>
                    {onRetry ? (
                        <Button variant="filled" radius="xl" color="gray" rightSection={<IconReload size={14} />} onClick={onRetry}>
                            Retry
                        </Button>
                    ) : null}
                </Stack>
            );
        case "default":
            return children;
        default: {
            // Exhaustiveness guard: if UiState changes, TS will error here.
            const _never: never = state;
            return _never;
        }
    }
};

export function PageShell(props: PageShellProps) {
    const { state, errorText, onRetry, children } = props;
    return (
        renderContent(state, errorText, onRetry, children)
    );
}
