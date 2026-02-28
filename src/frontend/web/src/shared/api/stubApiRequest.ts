import type { ApiError } from "@/shared/api/apiError";

const sleep = (ms: number) => new Promise<void>((r) => setTimeout(r, ms));

//const error: ApiError = httpError("http", "msg", 404);
export type DemoOpt =
    | { delay?: number; forceError?: null }
    | { delay?: number; forceError: ApiError }
    | { delay?: number; forceThrow?: string }; // намеренно "не ApiError"

export async function demoFunction(opt: DemoOpt): Promise<void> {
    if (opt.delay !== undefined) await sleep(opt.delay);

    if ("forceError" in opt && opt.forceError) throw opt.forceError;
    if ("forceThrow" in opt) throw new Error(opt.forceThrow);
}

export function generateUUIDNoDash(): string {
    return crypto.randomUUID().replace(/-/g, "");
}
/* Для сегмента URL/папки достаточно правила:
привести к lower-case
заменить пробелы на -
разрешить только [a-z0-9_-]
всё остальное → -
схлопнуть повторяющиеся -
обрезать длину (например 60)
если пусто → "x"
*/

export function generateSafeName(name: string): string {
    const raw = name.trim().toLowerCase();

    const replaced = raw
        .replace(/\s+/g, "-")
        .replace(/[^a-z0-9_-]+/g, "-")
        .replace(/-+/g, "-")
        .replace(/^-+|-+$/g, "");

    const clipped = replaced.slice(0, 60);

    return clipped.length > 0 ? clipped : "x";
}

//const demoGetCourse: DemoOpt = { delay: 1000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"
//const demoGetCourseLectures: DemoOpt = { delay: 1500, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

/* export async function getCourseById(courseId: string, token: string, signal?: AbortSignal): Promise<CourseDto> {
    await demoFunction(demoGetCourse);

}
 */
/* export async function getAllLecturesByCourseIdOrdered(courseId: string, token: string, signal?: AbortSignal): Promise<CourseLectureDto[]> {
    await demoFunction(demoGetCourseLectures);

} */


/* const demoCourse: CourseDto = {
    id: "demo101",
    title: "Demo Course",
    description: "Демо-курс для разработки и отладки интерфейса.",
}; */

/* const demoLectures: CourseLectureDto[] = [
    {
        id: "lecture-1",
        orderNo: 1,
        title: "Введение в курс",
        description: "Обзор целей курса, структуры и формата обучения.",
        videoUrl: "https://youtu.be/KcYM3pVJkf8?si=kWnsr_SF-la6usPN",
    },
    {
        id: "lecture-5",
        orderNo: 5,
        title: "Практический разбор",
        description: "Разбор примеров и типичных ошибок на практике.",
        videoUrl: "https://example.com/video/practice",
    },
    {
        id: "lecture-6",
        orderNo: 6,
        title: "Итоги и дальнейшие шаги",
        description: "Подведение итогов курса и рекомендации по дальнейшему изучению.",
        videoUrl: "https://example.com/video/summary",
    },
    {
        id: "lecture-2",
        orderNo: 2,
        title: "Базовые понятия",
        description: "Ключевые термины и базовые концепции, необходимые для дальнейших лекций.",
        videoUrl: "https://example.com/video/basics",
    },
    {
        id: "lecture-3",
        orderNo: 3,
        title: "Инструменты и окружение",
        description: "Настройка рабочего окружения и обзор используемых инструментов.",
        videoUrl: "https://example.com/video/tools",
    },
    {
        id: "lecture-4",
        orderNo: 4,
        title: "Архитектурный обзор",
        description: "Общее представление архитектуры проекта и основных компонентов.",
        videoUrl: "https://example.com/video/architecture",
    },
]; */
