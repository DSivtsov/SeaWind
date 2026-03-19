import type { CourseExerciseDto } from "@/pages/courses/layoutTabs/courseLayoutTabsApi";
import { demoFunction, type DemoOpt } from "@/shared/api/stubApiRequest";

const demoAllExercisesByCourseIdOrdered: DemoOpt = { delay: 1000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function stub_getAllExercisesByCourseIdOrdered(courseId: string, token: string, signal?: AbortSignal): Promise<CourseExerciseDto[]> {
    // временно не используются
    void token;
    void signal;

    await demoFunction(demoAllExercisesByCourseIdOrdered);

    if (courseId !== "csharp-basic") {
        return [];
    }
    return Exercises;
}

export const Exercises: CourseExerciseDto[] = [
    {
        id: "cb47b48f-af74-522f-9428-00b63ecd5dff",
        orderNo: 1,
        title: "Функции и базовые вычисления",
        shortDescription:
            "Набор задач на реализацию Function с параметрами и числовыми вычислениями.",
        mark: null,
    },
    {
        id: "cb47b48f-af74-522f-9428-00b63ecd5df0",
        orderNo: 2,
        title: "Условные конструкции",
        shortDescription:
            "Практика использования if/else и логических операторов для принятия решений.",
        mark: null,
    },
    {
        id: "cb47b48f-af74-522f-9428-00b63ecd5df1",
        orderNo: 3,
        title: "Циклы и повторяющиеся вычисления",
        shortDescription:
            "Работа с for и while для реализации повторяющихся алгоритмов.",
        mark: null,
    },
    {
        id: "cb47b48f-af74-522f-9428-00b63ecd5df2",
        orderNo: 4,
        title: "Работа со строками",
        shortDescription:
            "Обработка строк, поиск символов и преобразование текста.",
        mark: null,

    },
];

