const sleep = (ms: number) => new Promise<void>((r) => setTimeout(r, ms));

export type DemoOpt = { delay?: number, forceError: string | null };

export async function demoFunction({ delay, forceError }: DemoOpt): Promise<void> {
    if (delay !== undefined) await sleep(delay);
    if (forceError) throw new Error(forceError);
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
