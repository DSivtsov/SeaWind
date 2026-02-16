import type { ExerciseContentBlockDto, ChatMessageDto, ChatThreadDto, ChatAttachmentDto }
    from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { Exercises } from "@/pages/courses/layoutTabs/stubCourseLayoutTabsApi";
import { httpError } from "@/shared/api/apiError";
import { demoFunction, generateSafeName, generateUUIDNoDash, type DemoOpt } from "@/shared/api/stubApiRequest";

const demoExerciseData: DemoOpt = { delay: 1000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function stub_getExerciseData(token: string, signal: AbortSignal | undefined, exerciseId: string) {
    void token;
    void signal;

    await demoFunction(demoExerciseData);

    if (exerciseId !== "60659bf0-0b5a-11f1-b4ac-0800200c9a66") {
        throw httpError("http", "NotFoundError", 404);
    }
    return Exercises[0];
}

const demoExerciseContentBlock: DemoOpt = { delay: 1000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function stub_getExerciseContentBlock(token: string, signal: AbortSignal | undefined, exerciseId: string) {
    void token;
    void signal;
    //const urlGetCourseById = `/api/courses/${encodeURIComponent(courseId)}`;
    //return apiRequest<CourseExerciseChatDto>(urlGetCourseById, { method: "GET", parse: "json", signal }, token);
    await demoFunction(demoExerciseContentBlock);

    if (exerciseId !== "60659bf0-0b5a-11f1-b4ac-0800200c9a66") {
        throw httpError("http", "NotFoundError", 404);
    }
    return demoContentBlock;
}

const demoContentBlock: ExerciseContentBlockDto = {
    "details": "В этом упражнении необходимо реализовать указанные функций.\r\n \
            Каждая функция должна выполнять базовые числовые вычисления и возвращать корректный результат.\r\n \
            \"throw new NotImplementedException();\" - временная заглушка которую надо заменить правильным кодом \
            Каждая функция должна выполнять базовые числовые вычисления и возвращать корректный результат.\r\n \
            \"throw new NotImplementedException();\" - временная заглушка которую надо заменить правильным кодом \
            В этом упражнении необходимо реализовать указанные функций.\r\n \
            Каждая функция должна выполнять базовые числовые вычисления и возвращать корректный результат.\r\n \
            \"throw new NotImplementedException();\" - временная заглушка которую надо заменить правильным кодом \
            Каждая функция должна выполнять базовые числовые вычисления и возвращать корректный результат.\r\n \
            \"throw new NotImplementedException();\" - временная заглушка которую надо заменить правильным кодом \
            В этом упражнении необходимо реализовать указанные функций.\r\n \
            Каждая функция должна выполнять базовые числовые вычисления и возвращать корректный результат.\r\n \
            \"throw new NotImplementedException();\" - временная заглушка которую надо заменить правильным кодом \
            Каждая функция должна выполнять базовые числовые вычисления и возвращать корректный результат.\r\n \
            \"throw new NotImplementedException();\" - временная заглушка которую надо заменить правильным кодом",

    "blocks": [
        {
            "kind": "code",
            "language": "csharp",
            "contentUrl": "/contentExercises/func-basic/example.cs",
        },
        {
            "kind": "picture",
            "contentUrl": "/contentExercises/func-basic/example.png",
        }
    ]
};


const demoPostAttachmentMessage: DemoOpt = { delay: 3000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function stub_postAttachmentMessage(token: string, signal: AbortSignal | undefined,
    exerciseId: string, file: File): Promise<ChatAttachmentDto> {
    // временно не используются
    void token;
    void signal;

    await demoFunction(demoPostAttachmentMessage);
    const safeExerciseId = generateSafeName(exerciseId);
    const attachmentId = generateUUIDNoDash();
    const safeFileName = generateSafeName(file.name);

    return {
        attachmentId: attachmentId,
        fileName: file.name,
        url: `/chatsExercise/${safeExerciseId}/${attachmentId}__${safeFileName}`,
    };
}


const demoPostExerciseChatMessages: DemoOpt = { delay: 4000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function stub_postExerciseChatMessage(token: string, signal: AbortSignal | undefined,
    exerciseId: string, newChatMessage: ChatMessageDto): Promise<string> {
    // временно не используются
    void token;
    void signal;
    void exerciseId;
    void newChatMessage;

    await demoFunction(demoPostExerciseChatMessages);
    const messageId = generateUUIDNoDash();
    return messageId;
}


const demoGetExerciseChatMessages: DemoOpt = { delay: 1000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function stub_getExerciseChatMessages(token: string, signal: AbortSignal | undefined, exerciseId: string, studentId: string) {
    void token;
    void signal;

    await demoFunction(demoGetExerciseChatMessages);

    if (exerciseId !== "60659bf0-0b5a-11f1-b4ac-0800200c9a66" ||
        (studentId !== "3b795346-78d9-5464-87a7-f18e37343066" &&        //nick.mentor@example.com
            studentId !== "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7"))          //carol.student@example.com
        throw httpError("http", "NotFoundError", 404);
    return demoChatMessages;
}
const demoChatMessages: ChatMessageDto[] =
    [
        {
            "messageId": "msg_0001",
            "author": { "userId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7", "role": "Student" },
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "attachmentId": "att_0001",
                    "fileName": "Solution.cs",
                    "url": "/chatsExercise/csharp-basic/60659bf0-0b5a-11f1-b4ac-0800200c9a66/f48222bd2bfd412a__Solution.cs"
                }
            ],
        },
        {
            "messageId": "msg_0002",
            "author": { "userId": "3b795346-78d9-5464-87a7-f18e37343066", "role": "Mentor" },
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
        {
            "messageId": "msg_0003",
            "author": { "userId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7", "role": "Student" },
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "attachmentId": "att_0001",
                    "fileName": "Solution.cs",
                    "url": "/chatsExercise/csharp-basic/60659bf0-0b5a-11f1-b4ac-0800200c9a66/f48222bd2bfd412a__Solution.cs"
                }
            ],
        },
        {
            "messageId": "msg_0004",
            "author": { "userId": "3b795346-78d9-5464-87a7-f18e37343066", "role": "Mentor" },
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
        {
            "messageId": "msg_0005",
            "author": { "userId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7", "role": "Student" },
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "attachmentId": "att_0001",
                    "fileName": "Solution.cs",
                    "url": "/chatsExercise/csharp-basic/60659bf0-0b5a-11f1-b4ac-0800200c9a66/f48222bd2bfd412a__Solution.cs"
                }
            ],
        },
        {
            "messageId": "msg_0006",
            "author": { "userId": "3b795346-78d9-5464-87a7-f18e37343066", "role": "Mentor" },
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
        {
            "messageId": "msg_0007",
            "author": { "userId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7", "role": "Student" },
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "attachmentId": "att_0001",
                    "fileName": "Solution.cs",
                    "url": "/chatsExercise/csharp-basic/60659bf0-0b5a-11f1-b4ac-0800200c9a66/f48222bd2bfd412a__Solution.cs"
                }
            ],
        },
        {
            "messageId": "msg_0008",
            "author": { "userId": "3b795346-78d9-5464-87a7-f18e37343066", "role": "Mentor" },
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
    ];


const demoExerciseChatThread: DemoOpt = { delay: 1000, forceError: null };    // forceError = "DEMO_FORCED_ERROR"

export async function stub_getExerciseChatThread(token: string, signal: AbortSignal | undefined, courseId: string, exerciseId: string, userId: string) {
    void token;
    void signal;

    await demoFunction(demoExerciseChatThread);

    if (courseId !== "csharp-basic" ||
        exerciseId !== "60659bf0-0b5a-11f1-b4ac-0800200c9a66" ||
        userId !== "3b795346-78d9-5464-87a7-f18e37343066") {
        throw httpError("http", "NotFoundError", 404);
    }
    return demoChatThread;
}

const demoChatThread: ChatThreadDto =
{
    "threadId": "thr_c1_e1_s_alice",
    "scope": {
        "courseId": "csharp-basic",
        "exerciseId": "60659bf0-0b5a-11f1-b4ac-0800200c9a66",
        "studentId": "3b795346-78d9-5464-87a7-f18e37343066"
    },
    "lastMessageAt": "2026-02-19T19:40:20.000Z",
    "lastMessagePreview": "Ок, поправь naming и убери var в примере.",
    "counters": {
        "total": 2,
        "unreadByStudent": 1,
        "unreadByMentor": 0
    }
};


