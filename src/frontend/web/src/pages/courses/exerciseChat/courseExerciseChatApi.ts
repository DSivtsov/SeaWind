import {
    stub_getExerciseChatMessages, stub_getExerciseChatThread, stub_getExerciseContentBlock, stub_getExerciseData,
    stub_postAttachmentMessage,
    stub_postExerciseChatMessage
} from "@/pages/courses/exerciseChat/stubCourseExerciseChatApi";
import { type CourseExerciseDto } from "@/pages/courses/layoutTabs/courseLayoutTabsApi";
import type { ApiError } from "@/shared/api/apiError";
import type { Role } from "@/shared/auth/meApi";

export type ExerciseContentBlockDto = {
    details: string

    blocks: Array<
        { kind: "picture"; contentUrl: string }
        | { kind: "code"; contentUrl: string; language: "csharp" | "json" | "text" }
    >
}

export type ChatThreadDto = {
    threadId: string

    scope: {
        courseId: string
        exerciseId: string
        studentId: string
    }

    lastMessageAt: string | null
    lastMessagePreview: string | null

    counters: {
        total: number
        unreadByStudent: number
        unreadByMentor: number
    }
}

export type ChatMessageDto = {
    messageId?: string

    author: {
        userId: string
        role: Role
    }

    createdAt: string
    text: string | null

    attachments: ChatAttachmentDto[]
}

export type ChatAttachmentDto = {
    attachmentId: string
    fileName: string
    url: string
}

export type AttachmentsState =
    | { kind: "loading" }
    | { kind: "error"; error: ApiError }
    | { kind: "empty"; attachments: ChatAttachmentDto[] }
    | { kind: "loaded"; attachments: ChatAttachmentDto[] };

export type UploadStatus = "loading" | "loaded" | "error";

export type ChatMessageUi = ChatMessageDto & {
    clientGuid: string
    statusUpload: UploadStatus
}

export async function getExerciseData(exerciseId: string, token: string, signal?: AbortSignal): Promise<CourseExerciseDto> {

    return await stub_getExerciseData(token, signal, exerciseId);
}

export async function getExerciseContentBlock(exerciseId: string, token: string,
    signal?: AbortSignal): Promise<ExerciseContentBlockDto> {

    return await stub_getExerciseContentBlock(token, signal, exerciseId);
}

export async function loadCode(codeUrl: string, signal: AbortSignal) {
    const res = await fetch(codeUrl, { signal: signal });

    if (!res.ok) throw new Error("Failed to load file");

    return await res.text();
}

export async function getExerciseChatMessages(exerciseId: string, studentId: string, token: string,
    signal?: AbortSignal): Promise<ChatMessageDto[]> {

    return await stub_getExerciseChatMessages(token, signal, exerciseId, studentId);
}

export async function postAttachmentMessage(exerciseId: string, file: File,
    token: string, signal?: AbortSignal): Promise<ChatAttachmentDto> {
    // POST /api/exerciseChat/{courseId}/{exerciseId}/attachments
    /*
        const uploadUrl = `/api/exerciseChat/${courseId}/${exerciseId}/attachments`;
        const form = new FormData();
        form.append("file", file);
        await fetch(uploadUrl, { method: "POST", body: form, signal });
         */

    // возвращает contentUrl, по которому фронт потом скачает:
    // "/chatsExercise/{courseId}/{exerciseId}/{attachmentId}__{safeName}"
    return await stub_postAttachmentMessage(token, signal, exerciseId, file);
}


export async function postExerciseChatMessage(exerciseId: string, newChatMessage: ChatMessageDto,
    token: string, signal?: AbortSignal): Promise<string> {
    /*
POST POST /api/exerciseChat/{courseId}/{exerciseId}/messages
body: { newChatMessage }
→ возвращает “каноническое” сообщение (с server ids)
*/

    return await stub_postExerciseChatMessage(token, signal, exerciseId, newChatMessage);
}

export async function getExerciseChatThread(courseId: string, exerciseId: string, userId: string, token: string,
    signal?: AbortSignal): Promise<ChatThreadDto> {

    return await stub_getExerciseChatThread(token, signal, courseId, exerciseId, userId);
}

