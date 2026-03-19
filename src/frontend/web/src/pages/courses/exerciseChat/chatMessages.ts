import type {
    Attachment, MessageDto, Message, ExerciseChatDto,
    MessageUploadResponse
} from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import type { ChatRole } from "@/pages/courses/exerciseChat/ExerciseChatShell";
import type { UiZoneState } from "@/shared/components/ZoneShell";

export type CreateOptimisticMessageParams = {
    threadId: string
    authorId: string
    authorRole: ChatRole
    text: string | null
    attachments: Attachment[]
}

export function getMaxServerSeq(chatServerMessages: MessageDto[]): number {
    return chatServerMessages.reduce((max, m) => Math.max(max, m.seq), 0);
}

export function toClientMessages(chatServerMessages: MessageDto[]): Message[] {
    return chatServerMessages.map((dto) => {
        return {
            ...dto,
            statusUpload: "synced",
        };
    });
}

export function createOptimisticMessage(
    params: CreateOptimisticMessageParams, nextClientSeqRef: number): Message {
    const newMsg: Message = {
        ...params,
        id: `client-${params.threadId}-${nextClientSeqRef}`,
        createdAt: new Date().toISOString(),
        seq: nextClientSeqRef,
        statusUpload: "uploading",
    };
    return newMsg;
}

export function updatedLoadedMessage(message: Message, res: MessageUploadResponse): Message {
    return {
        ...message,
        id: res.id,
        seq: res.serverSeq,
        createdAt: res.createdAt,
        statusUpload: "synced",
    };
}

export function markMessageAsError(
    message: Message
): Message {
    return {
        ...message,
        statusUpload: "error",
    };
}

export function markMessageAsLoading(
    message: Message
): Message {
    return {
        ...message,
        statusUpload: "uploading",
    };
}

export function findMessageBySelectedSeq(
    state: UiZoneState<Message[]>,
    selectedSeq: number
): Message | null {
    if (state.kind !== "ready") return null;
    return state.data.find((m) => m.seq === selectedSeq) ?? null;
}

export type ChatPerms = {
    canWrite: boolean;
    canEdit: (msg: Message) => boolean;
    canDelete: (msg: Message) => boolean;
};

export function getChatPerms({ exercise, threadLocks }: ExerciseChatDto, role: ChatRole): ChatPerms {
    const IsFinished = exercise.mark === 2;
    const canWrite = !IsFinished &&
        (role === "Student" && exercise.status === "OnStudent") || (role === "Mentor" && exercise.status === "OnMentor");

    const canEdit = (msg: Message): boolean => canWrite && (msg.statusUpload === "synced" && msg.seq > threadLocks.lockSeq);
    const canDelete = canEdit;

    return { canWrite, canEdit, canDelete };
}

const INFO_TEXT: Record<ChatRole, { write: string; read: string }> = {
    Student: {
        write: "Опубликуйте решение или задайте вопрос",
        read: "Работа на проверке — писать может Mentor",
    },
    Mentor: {
        write: "Проведите ревью или задайте вопрос",
        read: "Ожидается сообщение студента — писать может Student",
    },
};

export function getChatTitleInfo(canWrite: boolean, userRole: ChatRole): string {
    const roleTexts = INFO_TEXT[userRole];
    return canWrite ? roleTexts.write : roleTexts.read;
}
