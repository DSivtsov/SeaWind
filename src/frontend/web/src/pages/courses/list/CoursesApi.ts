import { apiGet } from "@/shared/api/httpWrappers";
import { getToken } from "@/shared/auth/auth";

export type CourseDto = {
  id: string;
  title: string;
  code: string;
  description: string;
};

export function getCourses(signal?: AbortSignal): Promise<CourseDto[]> {
  return apiGet<CourseDto[]>("/api/courses", signal, getToken());
}
