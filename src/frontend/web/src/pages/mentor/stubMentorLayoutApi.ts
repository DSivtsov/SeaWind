import type { StudentExerciseDto } from "@/pages/mentor/mentorLayoutApi";
import { demoFunction, type DemoOpt } from "@/shared/api/stubApiRequest";


const demo_getStudentExerciseDto: DemoOpt = { delay: 1000, forceError: null };    // forceError: error | forceThrow "DEMO_FORCED_ERROR" | undefined

export async function stub_getStudentExerciseDto(email: string, isOnlyOnCheck: boolean, token: string,
    signal: AbortSignal | undefined) {
    void token;
    void signal;
    void email;
    void isOnlyOnCheck;

    await demoFunction(demo_getStudentExerciseDto);

    if (isOnlyOnCheck) {
        return studentExercises.filter(se => se.status === "OnMentor");
    }

    return studentExercises;
}
const exercises = {
    ex1: {
        Id: "cb47b48f-af74-522f-9428-00b63ecd5dff",
        Title: "Функции и базовые вычисления",
        ShortDescription: "Набор задач на реализацию методов с параметрами и числовыми вычислениями."
    },
    ex2: {
        Id: "30b41a62-5557-536a-8b76-072a2cfe69fa",
        Title: "Строки и ввод-вывод",
        ShortDescription: "Практика работы со строками: форматирование, поиск, замены, парсинг простых значений."
    },
    ex3: {
        Id: "e7891cd6-4093-5840-8391-0dd9ba9c36e2",
        Title: "Условные конструкции",
        ShortDescription: "Задачи на if/else и логические операторы: ветвление, проверки диапазонов, простые правила."
    },
    ex4: {
        Id: "d73cf883-a206-5a0f-a68a-6274c026cd8c",
        Title: "Циклы и накопление результата",
        ShortDescription: "Практика for/while: суммы, произведения, счётчики, поиск минимума/максимума."
    },
    ex5: {
        Id: "d37086ca-7d69-525c-98d9-915dd7342c98",
        Title: "Массивы: проход и поиск",
        ShortDescription: "Базовые операции с массивами: заполнение, линейный поиск, подсчёт, простая статистика."
    },
    ex6: {
        Id: "9e0bc811-3759-55c4-b2e8-9b45c00007a4",
        Title: "Мини-проект: консольный калькулятор",
        ShortDescription: "Собрать меню команд + функции вычислений; обработка неверного ввода (минимально)."
    }
};

const studentExercises: StudentExerciseDto[] = [
    {
        studentExerciseId: "leo.student@example.com_ex5",
        studentId: "leo.student@example.com",
        exerciseId: exercises.ex5.Id,
        email: "leo.student@example.com",
        exerciseTitle: exercises.ex5.Title,
        exerciseShortDescription: exercises.ex5.ShortDescription,
        status: "OnMentor",
        requestedCheckAt: "2026-03-13T11:05:00Z",
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "judy.student@example.com_ex1",
        studentId: "judy.student@example.com",
        exerciseId: exercises.ex1.Id,
        email: "judy.student@example.com",
        exerciseTitle: exercises.ex1.Title,
        exerciseShortDescription: exercises.ex1.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "bob.student@example.com_ex5",
        studentId: "bob.student@example.com",
        exerciseId: exercises.ex5.Id,
        email: "bob.student@example.com",
        exerciseTitle: exercises.ex5.Title,
        exerciseShortDescription: exercises.ex5.ShortDescription,
        status: "OnMentor",
        requestedCheckAt: "2026-03-13T11:40:00Z",
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "grace.student@example.com_ex3",
        studentId: "grace.student@example.com",
        exerciseId: exercises.ex3.Id,
        email: "grace.student@example.com",
        exerciseTitle: exercises.ex3.Title,
        exerciseShortDescription: exercises.ex3.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "mia.student@example.com_ex5",
        studentId: "mia.student@example.com",
        exerciseId: exercises.ex5.Id,
        email: "mia.student@example.com",
        exerciseTitle: exercises.ex5.Title,
        exerciseShortDescription: exercises.ex5.ShortDescription,
        status: "OnMentor",
        requestedCheckAt: "2026-03-12T17:10:00Z",
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "carol.student@example.com_ex6",
        studentId: "carol.student@example.com",
        exerciseId: exercises.ex6.Id,
        email: "carol.student@example.com",
        exerciseTitle: exercises.ex6.Title,
        exerciseShortDescription: exercises.ex6.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "carol.student@example.com_ex4",
        studentId: "carol.student@example.com",
        exerciseId: exercises.ex4.Id,
        email: "carol.student@example.com",
        exerciseTitle: exercises.ex4.Title,
        exerciseShortDescription: exercises.ex4.ShortDescription,
        status: "OnMentor",
        requestedCheckAt: "2026-03-13T09:25:00Z",
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "mia.student@example.com_ex2",
        studentId: "mia.student@example.com",
        exerciseId: exercises.ex2.Id,
        email: "mia.student@example.com",
        exerciseTitle: exercises.ex2.Title,
        exerciseShortDescription: exercises.ex2.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "judy.student@example.com_ex2",
        studentId: "judy.student@example.com",
        exerciseId: exercises.ex2.Id,
        email: "judy.student@example.com",
        exerciseTitle: exercises.ex2.Title,
        exerciseShortDescription: exercises.ex2.ShortDescription,
        status: "OnMentor",
        requestedCheckAt: "2026-03-12T21:15:00Z",
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "leo.student@example.com_ex2",
        studentId: "leo.student@example.com",
        exerciseId: exercises.ex2.Id,
        email: "leo.student@example.com",
        exerciseTitle: exercises.ex2.Title,
        exerciseShortDescription: exercises.ex2.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "grace.student@example.com_ex5",
        studentId: "grace.student@example.com",
        exerciseId: exercises.ex5.Id,
        email: "grace.student@example.com",
        exerciseTitle: exercises.ex5.Title,
        exerciseShortDescription: exercises.ex5.ShortDescription,
        status: "OnMentor",
        requestedCheckAt: "2026-03-13T10:50:00Z",
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "bob.student@example.com_ex4",
        studentId: "bob.student@example.com",
        exerciseId: exercises.ex4.Id,
        email: "bob.student@example.com",
        exerciseTitle: exercises.ex4.Title,
        exerciseShortDescription: exercises.ex4.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "judy.student@example.com_ex3",
        studentId: "judy.student@example.com",
        exerciseId: exercises.ex3.Id,
        email: "judy.student@example.com",
        exerciseTitle: exercises.ex3.Title,
        exerciseShortDescription: exercises.ex3.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "leo.student@example.com_ex6",
        studentId: "leo.student@example.com",
        exerciseId: exercises.ex6.Id,
        email: "leo.student@example.com",
        exerciseTitle: exercises.ex6.Title,
        exerciseShortDescription: exercises.ex6.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    },
    {
        studentExerciseId: "grace.student@example.com_ex1",
        studentId: "grace.student@example.com",
        exerciseId: exercises.ex1.Id,
        email: "grace.student@example.com",
        exerciseTitle: exercises.ex1.Title,
        exerciseShortDescription: exercises.ex1.ShortDescription,
        status: "OnStudent",
        requestedCheckAt: null,
        checkedAt: null,
        mark: null
    }
];
