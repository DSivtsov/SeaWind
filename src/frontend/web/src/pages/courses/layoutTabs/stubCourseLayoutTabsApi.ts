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
        id: "60659bf0-0b5a-11f1-b4ac-0800200c9a66",
        orderNo: 1,
        title: "Функции и базовые вычисления",
        shortDescription:
            "Набор задач на реализацию Function с параметрами и числовыми вычислениями.",
    },
    {
        id: "60659bf1-0b5a-11f1-b4ac-0800200c9a66",
        orderNo: 2,
        title: "Условные конструкции",
        shortDescription:
            "Практика использования if/else и логических операторов для принятия решений.",
    },
    {
        id: "60659bf2-0b5a-11f1-b4ac-0800200c9a66",
        orderNo: 3,
        title: "Циклы и повторяющиеся вычисления",
        shortDescription:
            "Работа с for и while для реализации повторяющихся алгоритмов.",
    },
    {
        id: "60659bf3-0b5a-11f1-b4ac-0800200c9a66",
        orderNo: 4,
        title: "Работа со строками",
        shortDescription:
            "Обработка строк, поиск символов и преобразование текста.",
    },
];

