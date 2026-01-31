import { Button, Stack } from "@mantine/core";

/** One generic component example */
export type SelectProps<T> = {
    items: readonly T[];
    value: T | null;
    getKey: (item: T) => string;
    render: (item: T) => React.ReactNode;
    onSelect: (item: T) => void;
};

export function Select<T>(props: SelectProps<T>) {
    const { items, value, getKey, render, onSelect } = props;
    const selectedKey = value ? getKey(value) : null;

    return (
        <Stack gap={6}>
            {items.map((it) => {
                const key = getKey(it);
                const isSelected = selectedKey === key;

                return (
                    <Button key={key} variant={isSelected ? "outline" : "subtle"} radius="md" fullWidth
                        onClick={(e) => { e.preventDefault(); onSelect(it); }}
                    >
                        {render(it)}
                    </Button>
                );
            })}
        </Stack >
    );
}
