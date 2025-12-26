import { apiRequest } from "@/shared/api/apiRequests";

export type CourseDto = {
  id: string;
  title: string;
  description: string;
};

export function getCourses(signal?: AbortSignal): Promise<CourseDto[]> {
  return apiRequest<CourseDto[]>("/api/courses", { method: "GET", signal });
}
