import type { ChatRole, DashboardChatInput } from "@/pages/courses/exerciseChat/ExerciseChatShell";
import type { ApiError } from "@/shared/api/apiError";
import { apiRequest } from "@/shared/api/apiRequests";

export type ExerciseDto = {
    orderNo: number;
    title: string;
};

export type ExerciseContentBlockDto = {
    details: string

    blocks: Array<
        { kind: "Picture"; urlFile: string }
        | { kind: "Code"; urlFile: string; typeContent: "CSharp" | "Json" | "Text" }
    >
}

export type StudentExerciseStatus = "OnStudent" | "OnMentor"

export type StudentExerciseMark = 0 | 1 | 2

export type ExerciseChatDto = {
    exercise:
    {
        status: StudentExerciseStatus
        mark: StudentExerciseMark | null
        threadId: string;
    },
    threadLocks:
    {
        lockSeq: number
    }
}

export type MessageDto = {
    id: string
    seq: number
    authorId: string
    authorRole: ChatRole
    text: string | null
    createdAt: string
    attachments: Attachment[]
}

export type MessageUploadRequest = {
    clientSeq: number
    text: string | null
    attachmentIds: string[]
}

export type MessageUploadResponse = {
    clientSeq: number
    id: string
    serverSeq: number
    createdAt: string
}

export type UploadStatus = "uploading" | "synced" | "error";

export type Message = MessageDto & {
    statusUpload: UploadStatus
}

export type Attachment = {
    id: string  //id появляется после загрузки Attachment на сервер, до этого момента AttachmentDto нет, а есть только File
    fileNameOriginal: string
}

export type ChangeStatusThreadResponse = {
    serverSeq: number
}

export type AttachmentsState =
    | { kind: "loading" }
    | { kind: "error"; error: ApiError }
    | { kind: "empty"; attachments: Attachment[] }
    | { kind: "loaded"; attachments: Attachment[] };

export async function getExerciseData(exerciseId: string, token: string, signal?: AbortSignal): Promise<ExerciseDto> {
    const urlGetExerciseDataById = `/api/exercises/${exerciseId}`;

    return apiRequest<ExerciseDto>(urlGetExerciseDataById, { method: "GET", parse: "json", signal }, token);
    //return await stub_getExerciseData(token, signal, exerciseId);
}

export async function getExerciseContentBlock(exerciseId: string, token: string,
    signal?: AbortSignal): Promise<ExerciseContentBlockDto> {
    const urlGetExerciseContentBlockById = `/api/exercises/${exerciseId}/content`;

    return apiRequest<ExerciseContentBlockDto>(urlGetExerciseContentBlockById, { method: "GET", parse: "json", signal }, token);
    //return await stub_getExerciseContentBlock(token, signal, exerciseId);
}

export async function loadCode(codeUrl: string, signal: AbortSignal) {
    const res = await fetch(codeUrl, { signal: signal });

    if (!res.ok) throw new Error("Failed to load file");

    return await res.text();
}

export async function getExerciseChatMessages(threadId: string, token: string, signal?: AbortSignal): Promise<MessageDto[]> {
    const urlGetExerciseChatMessages = `/api/threads/${threadId}/messages`;

    return apiRequest<MessageDto[]>(urlGetExerciseChatMessages, { method: "GET", parse: "json", signal }, token);

    //return await stub_getExerciseChatMessages(token, signal, threadId);
}

export async function getMissedExerciseChatMessages(threadId: string, beginSeq: number, tillSeq: number,
    token: string, signal?: AbortSignal): Promise<MessageDto[]> {
    const urlGetExerciseChatMessages = `/api/threads/${threadId}/messages?BeginSeq=${beginSeq}&TillSeq=${tillSeq}`;

    return apiRequest<MessageDto[]>(urlGetExerciseChatMessages, { method: "GET", parse: "json", signal }, token);
    //return await stub_????
}

export async function postMessageAttachments(threadId: string, uploadfiles: File[],
    token: string, signal?: AbortSignal): Promise<Attachment[]> {
    const urlPostMessageAttachments = `/api/threads/${threadId}/attachments`;

    const form = new FormData();
    uploadfiles.forEach(file => form.append("Files", file));

    return apiRequest<Attachment[]>(urlPostMessageAttachments, { method: "POST", parse: "json", body: form, signal }, token);
    //return await stub_postAttachmentMessage(token, signal, threadId, uploadfiles);
}

export async function postExerciseChatMessage(threadId: string, body: MessageUploadRequest, token: string,
    signal?: AbortSignal): Promise<MessageUploadResponse> {
    const urlPostExerciseChatMessage = `/api/threads/${threadId}/messages`;

    return apiRequest<MessageUploadResponse>(urlPostExerciseChatMessage, { method: "POST", parse: "json", body, signal }, token);
    //return await stub_postExerciseChatMessage(token, signal, threadId, body);
}

export async function getExerciseChatData(exerciseId: string, dashboardChat: DashboardChatInput, token: string,
    signal?: AbortSignal): Promise<ExerciseChatDto> {

    let urlGetExerciseChatData = `/api/exercises/${exerciseId}/chat-thread`;

    if (dashboardChat.role === "Mentor" && dashboardChat.studentId) {
        const params = new URLSearchParams({
            studentId: dashboardChat.studentId
        });

        urlGetExerciseChatData += `?${params.toString()}`;
    }

    return apiRequest<ExerciseChatDto>(urlGetExerciseChatData, { method: "GET", parse: "json", signal }, token);
    //return await stub_getExerciseChatData....
}

export async function downloadAttachmentApi(attachmentId: string, token: string): Promise<Blob> {
    const downloadUrl = `/api/attachments/${attachmentId}/download`;

    return apiRequest<Blob>(downloadUrl, { method: "GET", parse: "blob" }, token);
}

export async function putChangeStatusThreadAsync(threadId: string, newStatus: StudentExerciseStatus, mark: number | null, token: string,
    signal: AbortSignal): Promise<ChangeStatusThreadResponse> {
    const urlPutChangeStatusThread = `/api/threads/${threadId}/status`;

    const body = {
        newStatus: newStatus,
        mark: mark
    };

    return apiRequest<ChangeStatusThreadResponse>(urlPutChangeStatusThread, { method: "PUT", parse: "json", body, signal }, token);
}
