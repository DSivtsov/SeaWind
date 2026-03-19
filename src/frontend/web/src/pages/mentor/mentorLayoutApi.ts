import { apiRequest } from "@/shared/api/apiRequests";

type StudentExerciseStatus = "OnStudent" | "OnMentor";

export type StudentExerciseDto = {
    studentExerciseId: string
    studentId: string
    exerciseId: string
    email: string
    exerciseTitle: string
    exerciseShortDescription: string
    status: StudentExerciseStatus
    mark: number | null
    requestedCheckAt: string | null
    checkedAt: string | null
};

export function buildUrl(params: { email: string; isOnlyOnCheck: boolean }): string {
    const sp = new URLSearchParams();

    if (params.email != null && params.email.trim()) sp.set("email", params.email.trim());

    if (params.isOnlyOnCheck) sp.set("onlyOnCheck", "true");

    const qs = sp.toString();
    return qs ? `/api/mentor/exercise-chats/inbox?${qs}` : "/api/mentor/exercise-chats/inbox";
}

export async function getMentorExerciseChatInbox(email: string, isOnlyOnCheck: boolean, token: string,
    signal?: AbortSignal): Promise<StudentExerciseDto[]> {

    const url = buildUrl({ email, isOnlyOnCheck });

    return apiRequest<StudentExerciseDto[]>(url, { method: "GET", parse: "json", signal }, token);
    //return stub_getStudentExerciseDto(email, isOnlyOnCheck, token, signal);
}

