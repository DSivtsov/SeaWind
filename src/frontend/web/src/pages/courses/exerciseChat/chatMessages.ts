import type { ChatAttachmentDto, ChatMessageDto, ChatMessageUi } from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import type { Role } from "@/shared/auth/meApi";
import type { UiZoneState } from "@/shared/components/ZoneShell";

export type CreateOptimisticMessageParams = {
    author: { userId: string; role: Role }
    text: string | null
    attachments: ChatAttachmentDto[]
}

export function getClientMessages(
    chatServerMessages: ChatMessageDto[]
): ChatMessageUi[] {
    return chatServerMessages.map((m) => {
        if (!m.messageId) {
            throw new Error("Server message must contain messageId");
        }

        return {
            ...m,
            clientGuid: m.messageId,
            statusUpload: "loaded",
        };
    });
}

export function createOptimisticMessage(
    params: CreateOptimisticMessageParams
): ChatMessageUi {
    const clientGuid = crypto.randomUUID();

    const newMsg: ChatMessageUi = {
        ...params,
        messageId: undefined,
        createdAt: new Date().toISOString(),

        clientGuid,
        statusUpload: "loading",
    };

    return newMsg;
}

export function markMessageAsLoaded(
    message: ChatMessageUi,
    serverMessageId: string
): ChatMessageUi {
    return {
        ...message,
        messageId: serverMessageId,
        statusUpload: "loaded",
    };
}

export function markMessageAsError(
    message: ChatMessageUi
): ChatMessageUi {
    return {
        ...message,
        statusUpload: "error",
    };
}

export function markMessageAsLoading(
    message: ChatMessageUi
): ChatMessageUi {
    return {
        ...message,
        statusUpload: "loading",
    };
}

export function findMessageByClientGuid(
    state: UiZoneState<ChatMessageUi[]>,
    clientGuid: string
): ChatMessageUi | null {
    if (state.kind !== "ready") return null;
    return state.data.find((m) => m.clientGuid === clientGuid) ?? null;
}
