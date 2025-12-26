import { useEffect, useMemo, useState } from "react";
import type { UiState } from "@/shared/UiState";
import { TestPageShell } from "@/pages/demo/shared/TestPageShell";
import { Select } from "@/shared/ui/Select";
import { useDebouncedValue } from "@/shared/hooks/useDebouncedValue";
import { Box, Button, Stack, Text, TextInput } from "@mantine/core";

/** Demo: uses all points in one place */
type Course = {
    id: string;
    title: string;
};

const ALL_COURSES: Course[] = [
    { id: "csharp-base", title: "C# Base" },
    { id: "unity-base", title: "Unity Base" },
    { id: "aspnet-mvp", title: "ASP.NET MVP" },
];

function TestManTine() {
    // union type for UI state
    const [state, setState] = useState<UiState>("loading");

    // typed state for data
    const [courses, setCourses] = useState<Course[]>([]);
    const [selected, setSelected] = useState<Course | null>(null);

    // events (ChangeEvent)
    const [query, setQuery] = useState("");
    const debouncedQuery = useDebouncedValue(query, 250);

    // useEffect deps + cleanup (simulated fetch)
    useEffect(() => {
        setState("loading");

        const id = window.setTimeout(() => {
            // simulate "error" sometimes just to show the pattern
            const shouldFail = false;

            if (shouldFail) {
                setState("error");
                return;
            }

            setCourses(ALL_COURSES);

            const isEmpty = ALL_COURSES.length === 0;
            //const isEmpty = true;
            if (isEmpty) {
                setState("empty");
            } else {
                setState("default");
            }
        }, 350);

        return () => window.clearTimeout(id);
    }, []); // deps are correct: runs once like "componentDidMount"

    const filtered = useMemo(() => {
        const q = debouncedQuery.trim().toLowerCase();
        if (q.length === 0) return courses;
        return courses.filter((c) => c.title.toLowerCase().includes(q));
    }, [courses, debouncedQuery]);

    const onRetry = () => {
        // mouse event typing example
        setState("loading");
        // simplest retry: re-run the same loading logic
        setCourses(ALL_COURSES);
        setState(ALL_COURSES.length === 0 ? "empty" : "default");
    };
    const isNoResults = state === "default" && courses.length > 0 && filtered.length === 0;

    return (
        <TestPageShell title="React + TypeScript — practical minimum" state={state} errorText="Failed to load courses." onRetry={onRetry}>
            <Stack gap="sm" w={520} >
                <TextInput
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    label="Фильтр"
                    description="Ограничение списка курсов"
                    placeholder="Введите значение для фильтрации…"
                />
                {/*
                Для отладки debouncedQuery только
                <Text size="xs" c="dimmed">
                    Debounced query:{" "}
                    <Text component="span" fw={500} c="inherit">
                        {debouncedQuery || "—"}
                    </Text>
                </Text>
                 */}
                {isNoResults ? <Text size="xs" c="dimmed">No matches</Text> : null}
                <Select<Course>
                    items={filtered}
                    value={selected}
                    getKey={(c) => c.id}
                    render={(c) => (
                        <Stack gap={1}>
                            <Text component="span" size="sm" fw={600} lh={1.1}>{c.title}</Text>
                            <Text component="span" size="xs" c="dimmed" lh={1.1}>{c.id}</Text>
                        </Stack>
                    )}
                    onSelect={(c) => setSelected(c)}
                />
                <Box pt={6}>
                    <Button
                        type="button"
                        variant="outline"
                        onClick={() => setSelected(null)}
                        disabled={selected == null}
                    >
                        Clear selection
                    </Button>
                </Box>
                <Stack gap={2}>
                    <Text size="md" fw={300} >Selected:</Text>
                    <Text>{selected ? `${selected.title} (${selected.id})` : "—"}</Text>
                </Stack>
            </Stack>
        </TestPageShell>
    );
}

export default TestManTine;
