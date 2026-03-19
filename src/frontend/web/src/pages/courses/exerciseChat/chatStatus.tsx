import type { StudentExerciseMark } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import type { ChatRole, } from "@/pages/courses/exerciseChat/ExerciseChatShell";
import type { JSX } from "react";

export const tooltipTextStudent = (
    <>Вы можете отправить улучшенную версию чтобы получить более высокую оценку.<br />
        Если вы уже отправили на проверку, дождитесь результата, после этого сможете отправить новую версию.</>
);

export const tooltipTextMentor = (
    <>Напишите отзыв и оцените работу студента перед отправкой вашего ответа.</>
);

export function checkEnableSendOnCheck(role: ChatRole, threadLocks: { lockSeq: number },
    clientSeq: number): boolean {
    switch (role) {
        case "Student":
            return clientSeq > threadLocks.lockSeq;
        case "Mentor":
            return clientSeq > threadLocks.lockSeq;
    }
}

export function getMarkInfo(mark: StudentExerciseMark | null): { markText: string; buttonText: string | JSX.Element; } {
    switch (mark) {
        case 0:
            return {
                markText: "Не зачтено",
                buttonText: <>Запросить проверку<br /> исправленной версии</>
            };
        case 1:
            return {
                markText: "Зачтено",
                buttonText: <>Запросить проверку<br /> улучшенной версии</>
            };
        case 2:
            return {
                markText: "Отлично",
                buttonText: <>Упражнение полностью <br /> выполнено</>
            };
        default:
            return {
                markText: "Нет оценки",
                buttonText: "Запросить проверку"
            };
    }
}
