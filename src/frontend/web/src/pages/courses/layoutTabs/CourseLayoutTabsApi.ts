import type { CourseDto } from "@/pages/courses/list/CoursesApi";
import { demoFunction, type DemoOpt } from "@/shared/api/stubApiRequest";

export type CourseLectureDto = {
    id: string;
    order: number;
    title: string;
    description?: string | null;
    videoUrl?: string | null;
};

const demoGetCourse: DemoOpt = { delay: 1000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"
const demoGetCourseLectures: DemoOpt = { delay: 1500, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function getCourse(courseId: string, token: string, signal?: AbortSignal): Promise<CourseDto> {
    void courseId;
    void token;
    void signal;

    await demoFunction(demoGetCourse);

    return demoCourse;
}

export async function getCourseLectures(courseId: string, token: string, signal?: AbortSignal): Promise<CourseLectureDto[]> {
    void courseId;
    void token;
    void signal;

    await demoFunction(demoGetCourseLectures);

    //return [];
    return demoLectures;
}


const demoCourse: CourseDto = {
    id: "demo101",
    title: "Demo Course",
    description: "Демо-курс для разработки и отладки интерфейса.",
};

const demoLectures: CourseLectureDto[] = [
    {
        id: "lecture-1",
        order: 1,
        title: "Введение в курс",
        description: "Обзор целей курса, структуры и формата обучения.",
        videoUrl: "https://youtu.be/KcYM3pVJkf8?si=kWnsr_SF-la6usPN",
    },
    {
        id: "lecture-5",
        order: 5,
        title: "Практический разбор",
        description: "Разбор примеров и типичных ошибок на практике.",
        videoUrl: "https://example.com/video/practice",
    },
    {
        id: "lecture-6",
        order: 6,
        title: "Итоги и дальнейшие шаги",
        description: "Подведение итогов курса и рекомендации по дальнейшему изучению.",
        videoUrl: "https://example.com/video/summary",
    },
    {
        id: "lecture-2",
        order: 2,
        title: "Базовые понятия",
        description: "Ключевые термины и базовые концепции, необходимые для дальнейших лекций.",
        videoUrl: "https://example.com/video/basics",
    },
    {
        id: "lecture-3",
        order: 3,
        title: "Инструменты и окружение",
        description: "Настройка рабочего окружения и обзор используемых инструментов.",
        videoUrl: "https://example.com/video/tools",
    },
    {
        id: "lecture-4",
        order: 4,
        title: "Архитектурный обзор",
        description: "Общее представление архитектуры проекта и основных компонентов.",
        videoUrl: "https://example.com/video/architecture",
    },
];
