import { getCourses, type CourseDto } from "@/pages/courses/list/CoursesApi";
import { PageShell } from "@/shared/PageShell";
import { CourseCard } from "@/pages/courses/list/CourseCard";
import type { UiState } from "@/shared/UiState";
import { Box, SimpleGrid } from "@mantine/core";
import { useState, useEffect, useRef } from "react";
import { isAbort, type ApiError } from "@/shared/api/apiRequests";

export function CoursesListPage() {
    // union type for UI state
    const [uiState, setUiState] = useState<UiState>("loading");
    const [courses, setCourses] = useState<CourseDto[]>([]);
    const [errorText, setErrorText] = useState<string>("");

    const controllerRef = useRef<AbortController | null>(null);

    const load = async () => {
        controllerRef.current?.abort();

        const abortController = new AbortController();
        controllerRef.current = abortController;

        setUiState("loading");
        try {
            const data = await getCourses(abortController.signal);

            setCourses(data);
            setUiState(data.length === 0 ? "empty" : "ready");
        } catch (e: unknown) {
            if (isAbort(e)) return;

            const msg = `Проблема с сервером. Попробуйте позже. Error [${((e as ApiError).message ?? "Request failed")}]`;
            console.log(msg);
            setErrorText(msg);
            setUiState("error");
        }
    };

    useEffect(() => {
        load();
        return () => controllerRef.current?.abort();
    }, []);

    //Retry cycle of load()
    const onRetry = load;

    return (
        <PageShell state={uiState} errorText={errorText} onRetry={onRetry}>
            <Box p="md" >
                <SimpleGrid cols={2}>
                    {courses.map((item) => (
                        <CourseCard key={item.id} course={item} />
                    ))}
                </SimpleGrid>
            </Box>
        </PageShell>
    );
}
