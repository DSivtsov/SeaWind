import { getCourses, type CourseDto } from "@/pages/courses/list/CoursesApi";
import { PageShell } from "@/shared/PageShell";
import { CourseCard } from "@/pages/courses/list/CourseCard";
import type { UiState } from "@/shared/UiState";
import { ScrollArea, SimpleGrid } from "@mantine/core";
import { useState, useEffect, useRef } from "react";
import { loadPageData } from "@/shared/api/loadPageData";

export function CoursesListPage() {
    // union type for UI state
    const [uiState, setUiState] = useState<UiState>("loading");
    const [courses, setCourses] = useState<CourseDto[]>([]);
    const [errorText, setErrorText] = useState<string>("");

    const controllerRef = useRef<AbortController | null>(null);

    const load = (signal?: AbortSignal) => {
        controllerRef.current?.abort();

        const abortController = new AbortController();
        controllerRef.current = abortController;

        setUiState("loading");

        void loadPageData(
            () => getCourses(signal),
            (data) => {
                setCourses(data);
                setUiState(data.length === 0 ? "empty" : "default");
            },
            (msg) => {
                setErrorText(msg);
                setUiState("error");
            }
        );
    };

    useEffect(() => {
        load();
        return () => controllerRef.current?.abort();
    }, []);

    //Retry cycle of load()
    const onRetry = load;

    return (
        <PageShell state={uiState} errorText={errorText} onRetry={onRetry}>
            <ScrollArea h="100%">
                <SimpleGrid cols={2}>
                    {courses.map((item) => (
                        <CourseCard key={item.id} course={item} />
                    ))}
                </SimpleGrid>
            </ScrollArea>
        </PageShell>
    );
}
