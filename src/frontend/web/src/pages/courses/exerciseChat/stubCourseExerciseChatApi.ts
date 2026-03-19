import type { ExerciseContentBlockDto, MessageDto, Attachment, ExerciseDto, ExerciseChatDto, MessageUploadResponse, MessageUploadRequest }
    from "@/pages/courses/exerciseChat/courseExerciseChatApi";
import { Exercises } from "@/pages/courses/layoutTabs/stubCourseLayoutTabsApi";
import { httpError } from "@/shared/api/apiError";
import { demoFunction, generateUUIDNoDash, type DemoOpt } from "@/shared/api/stubApiRequest";

// import {type ApiError}  from "@/shared/api/apiError";
// const error: ApiError = httpError("http", "msg", 404);
const demoExerciseData: DemoOpt = { delay: 1000, };    // forceError: error | forceThrow "DEMO_FORCED_ERROR" | undefined

export async function stub_getExerciseData(token: string, signal: AbortSignal | undefined, exerciseId: string): Promise<ExerciseDto> {
    void token;
    void signal;

    await demoFunction(demoExerciseData);

    if (exerciseId !== "cb47b48f-af74-522f-9428-00b63ecd5dff") {
        throw httpError("http", "NotFoundError", 404);
    }

    const exercise: ExerciseDto = Exercises[0];
    return exercise;
}

const demoExerciseContentBlock: DemoOpt = { delay: 1000, forceError: null };    // forceError: error | forceThrow "DEMO_FORCED_ERROR" | undefined

export async function stub_getExerciseContentBlock(token: string, signal: AbortSignal | undefined, exerciseId: string) {
    void token;
    void signal;
    //const urlGetCourseById = `/api/courses/${encodeURIComponent(courseId)}`;
    //return apiRequest<CourseExerciseChatDto>(urlGetCourseById, { method: "GET", parse: "json", signal }, token);
    await demoFunction(demoExerciseContentBlock);

    if (exerciseId !== "cb47b48f-af74-522f-9428-00b63ecd5dff") {
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
            "kind": "Code",
            "typeContent": "CSharp",
            "urlFile": "/contentExercises/csharp-basic/func-basic_example.cs",
        },
        {
            "kind": "Picture",
            "urlFile": "/contentExercises/csharp-basic/func-basic_example.png",
        }
    ]
};


const demoPostAttachmentMessage: DemoOpt = { delay: 3000, forceError: null };    // forceError: error | forceThrow "DEMO_FORCED_ERROR" | undefined

export async function stub_postAttachmentMessage(token: string, signal: AbortSignal | undefined,
    threadId: string, uploadfiles: File[]): Promise<Attachment[]> {
    // временно не используются
    void token;
    void signal;
    void threadId;

    await demoFunction(demoPostAttachmentMessage);

    const attachments: Attachment[] = uploadfiles.map(file => {
        return {
            id: generateUUIDNoDash(),
            fileNameOriginal: file.name
        };
    });

    return attachments;
}


const demoPostExerciseChatMessages: DemoOpt = { delay: 4000, forceError: null };    // forceError: error | forceThrow "DEMO_FORCED_ERROR" | undefined

export async function stub_postExerciseChatMessage(token: string, signal: AbortSignal | undefined,
    threadId: string, body: MessageUploadRequest): Promise<MessageUploadResponse> {
    // временно не используются
    void token;
    void signal;
    void threadId;
    void body;

    await demoFunction(demoPostExerciseChatMessages);

    const idServer = generateUUIDNoDash();
    const seqServer = body.clientSeq;   // случай совпадения clientSeq и seq на сервере
    //const seqServer = body.clientSeq + 2; // случай seqServer не совпадает с  clientSeq

    return {
        clientSeq: body.clientSeq,
        id: idServer,
        serverSeq: seqServer,
        createdAt: new Date().toISOString(),
    };
}


const demoGetExerciseChatMessages: DemoOpt = { delay: 1000, forceError: null };    // forceError: error | forceThrow "DEMO_FORCED_ERROR" | undefined

export async function stub_getExerciseChatMessages(token: string, signal: AbortSignal | undefined, threadId: string) {
    void token;
    void signal;

    await demoFunction(demoGetExerciseChatMessages);

    if (threadId !== "69b03bf20279d4f213043b05")
        return [];
    //(studentId !== "3b795346-78d9-5464-87a7-f18e37343066" &&        //nick.mentor@example.com
    // studentId !== "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7"))          //carol.student@example.com
    //throw httpError("http", "NotFoundError", 404);

    return demoChatMessages;
}
const demoChatMessages: MessageDto[] =
    [
        {
            "id": "msg_0001",
            seq: 1,
            "authorId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7",
            "authorRole": "Student",
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "id": "69acfcf9cf201d619996bd81",
                    "fileNameOriginal": "Solution.cs",
                    //"downloadUrl": "cb47b48f-af74-522f-9428-00b63ecd5dff/f48222bd2bfd412a__Solution.cs",
                }
            ],
        },
        {
            "id": "msg_0002",
            seq: 2,
            "authorId": "3b795346-78d9-5464-87a7-f18e37343066",
            "authorRole": "Mentor",
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
        {
            "id": "msg_0003",
            seq: 3,
            "authorId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7",
            "authorRole": "Student",
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "id": "69acfcf9cf201d619996bd81",
                    "fileNameOriginal": "Solution.cs",
                    //"downloadUrl": "cb47b48f-af74-522f-9428-00b63ecd5dff/f48222bd2bfd412a__Solution.cs",
                }
            ],
        },
        {
            "id": "msg_0004",
            seq: 4,
            "authorId": "3b795346-78d9-5464-87a7-f18e37343066",
            "authorRole": "Mentor",
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
        {
            "id": "msg_0005",
            seq: 5,
            "authorId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7",
            "authorRole": "Student",
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "id": "69acfcf9cf201d619996bd81",
                    "fileNameOriginal": "Solution.cs",
                    //"downloadUrl": "cb47b48f-af74-522f-9428-00b63ecd5dff/f48222bd2bfd412a__Solution.cs",
                }
            ],
        },
        {
            "id": "msg_0006",
            seq: 6,
            "authorId": "3b795346-78d9-5464-87a7-f18e37343066",
            "authorRole": "Mentor",
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
        {
            "id": "msg_0007",
            seq: 7,
            "authorId": "8cb9fe4e-5dc7-5be5-8cd4-0038cd267ab7",
            "authorRole": "Student",
            "createdAt": "2026-02-19T19:38:05.000Z",
            "text": "Я сделал решение. Посмотри, пожалуйста.",
            "attachments": [
                {
                    "id": "69acfcf9cf201d619996bd81",
                    "fileNameOriginal": "Solution.cs",
                    //"downloadUrl": "cb47b48f-af74-522f-9428-00b63ecd5dff/f48222bd2bfd412a__Solution.cs",
                }
            ],
        },
        {
            "id": "msg_0008",
            seq: 8,
            "authorId": "3b795346-78d9-5464-87a7-f18e37343066",
            "authorRole": "Mentor",
            "createdAt": "2026-02-19T19:40:20.000Z",
            "text": "Ок, поправь naming и убери var в примере. После этого можно отправлять на проверку.",
            "attachments": [],
        },
    ];

const demoExerciseChatStatus: DemoOpt = { delay: 1000, forceError: null };    // forceError: error | forceThrow: "DEMO_FORCED_ERROR" | undefined

export async function stub_getExerciseChatData(token: string, signal: AbortSignal | undefined, exerciseId: string) {
    void token;
    void signal;

    await demoFunction(demoExerciseChatStatus);

    if (exerciseId !== "cb47b48f-af74-522f-9428-00b63ecd5dff") {
        throw httpError("http", "NotFoundError", 404);
    }
    return ExerciseChatDto;
}

const ExerciseChatDto: ExerciseChatDto =
{
    exercise:
    {
        "status": "OnStudent",
        "mark": null,
        "threadId": "69a9d551501cae1989b429b1",
    },
    threadLocks:
    {
        lockSeq: 2
    }
};
