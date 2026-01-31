import { PageShell } from "@/shared/components/PageShell";
import { CourseCard, type Course } from "@/pages/courses/list/CourseCard";
import type { UiState } from "@/shared/types/UiState";
import { ScrollArea, SimpleGrid } from "@mantine/core";
import { useState, useEffect } from "react";

const DEMO_COURSES: Course[] = [
    { id: "csharp-base-1", title: "C# Base", description: "Basics of C# for game/web development." },
    { id: "unity-base-1", title: "Unity Base", description: "Unity fundamentals: scenes, prefabs, components." },
    { id: "aspnet-mvp-1", title: "ASP.NET MVP", description: "Build Workshoptitle MVP backend with .NET 8." },
    { id: "csharp-base-2", title: "C# Base", description: "Basics of C# for game/web development." },
    { id: "unity-base-2", title: "Unity Base", description: "Unity fundamentals: scenes, prefabs, components." },
    { id: "aspnet-mvp-2", title: "ASP.NET MVP", description: "Build Workshoptitle MVP backend with .NET 8." },
    { id: "csharp-base-3", title: "C# Base", description: "Basics of C# for game/web development." },
    { id: "unity-base-3", title: "Unity Base", description: "Unity fundamentals: scenes, prefabs, components." },
    { id: "aspnet-mvp-3", title: "ASP.NET MVP", description: "Build Workshoptitle MVP backend with .NET 8." },
    { id: "csharp-base-4", title: "C# Base", description: "Basics of C# for game/web development." },
    { id: "unity-base-4", title: "Unity Base", description: "Unity fundamentals: scenes, prefabs, components." },
    { id: "aspnet-mvp-4", title: "ASP.NET MVP", description: "Build Workshoptitle MVP backend with .NET 8." },
];

export function CoursesListPageStub() {
    // union type for UI state
    const [state, setState] = useState<UiState>("loading");

    // typed state for data
    const [courses, setCourses] = useState<Course[]>([]);

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

            setCourses(DEMO_COURSES);

            const isEmpty = DEMO_COURSES.length === 0;
            //const isEmpty = true;
            if (isEmpty) {
                setState("empty");
            } else {
                setState("ready");
            }
        }, 550);

        return () => window.clearTimeout(id);
    }, []); // deps are correct: runs once like "componentDidMount"

    const onRetry = () => {
        // mouse event typing example
        setState("loading");
        // simplest retry: re-run the same loading logic
        setCourses(DEMO_COURSES);
        setState(DEMO_COURSES.length === 0 ? "empty" : "ready");
    };

    return (
        <PageShell state={state} errorText="Failed to load courses." onRetry={onRetry}>
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
