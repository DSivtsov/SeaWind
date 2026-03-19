import type { CourseDto } from "@/pages/courses/list/CoursesApi";
import { apiRequest } from "@/shared/api/apiRequests";

export type CourseLectureDto = {
    id: string;
    orderNo: number;
    title: string;
    videoUrl?: string | null;
    description?: string | null;
};

export type CourseExerciseDto = {
    id: string;
    orderNo: number;
    title: string;
    shortDescription?: string;
    mark: number | null;
};

export async function getCourseById(courseId: string, token: string, signal?: AbortSignal): Promise<CourseDto> {
    const urlGetCourseById = `/api/courses/${encodeURIComponent(courseId)}`;

    return apiRequest<CourseDto>(urlGetCourseById, { method: "GET", parse: "json", signal }, token);
}

export async function getAllLecturesByCourseIdOrdered(courseId: string, token: string, signal?: AbortSignal): Promise<CourseLectureDto[]> {
    const urlGetAllLecturesByCourseId = `/api/courses/${encodeURIComponent(courseId)}/lectures`;

    return apiRequest<CourseLectureDto[]>(urlGetAllLecturesByCourseId, { method: "GET", parse: "json", signal }, token);
}

export async function getAllExercisesByCourseIdOrdered(courseId: string, token: string, signal?: AbortSignal): Promise<CourseExerciseDto[]> {
    const urlGetAllExercisesByCourseId = `/api/courses/${encodeURIComponent(courseId)}/exercises`;

    return apiRequest<CourseExerciseDto[]>(urlGetAllExercisesByCourseId, { method: "GET", parse: "json", signal }, token);
}



