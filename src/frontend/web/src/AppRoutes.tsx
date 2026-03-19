//import React from "react";
import { Navigate, Route, Routes } from "react-router-dom";

// Demo pages (keep)
import TestManTine from '@/pages/demo/TestManTine';
import TestCourseListPage from "@/pages/demo/TestCourseListPage";

import { PagePlaceholder } from "@/shared/components/PagePlaceholder";
import { TestCoursesLayout } from "@/pages/demo/TestCoursesLayout";
import { CoursesLayout } from "@/pages/courses/list/CoursesLayout";
import { CoursesLayoutStub } from "@/pages/courses/list-stub/CoursesLayout.stub";
import TestPage from "@/pages/demo/TestPage";

import { LandingPage } from "@/pages/landing/LandingPage";
import { Test } from "@/pages/demo/Test";
import { TestMePage } from "@/pages/auth/TestMePage";

import { RouteGuard } from "@/shared/components/RouteGuard";
import { ForbiddenPage } from "@/pages/guard/ForbiddenPage";
import { CourseLecturesPage } from "@/pages/courses/layoutTabs/CourseLecturesPage";
import { CourseLayoutTabs } from "@/pages/courses/layoutTabs/CourseLayoutTabs";

import { AdminLayout } from "@/pages/admin/AdminLayout";
import { AdminManageUserRoles } from "@/pages/admin/AdminManageUserRoles";
import { CourseExercisesPage } from "@/pages/courses/layoutTabs/CourseExercisesPage";
import { ExerciseDashboard } from "@/pages/courses/exerciseChat/ExerciseDashboard";
import { MentorLayout } from "@/pages/mentor/MentorLayout";
import { MentorExerciseChatInbox } from "@/pages/mentor/MentorExerciseChatInbox";

export function AppRoutes() {
  return (
    <Routes>
      {/* Begin section demo routes */}
      <Route path="/testmantine" element={<TestManTine />} />

      <Route path="/test" element={<TestCoursesLayout />}>
        <Route index element={<Navigate to="courses" replace />} />
        <Route path="courses" element={<TestCourseListPage />} />
      </Route>

      <Route path="/test-test" element={<Test />} />
      <Route path="/test-page" element={<TestPage />} />

      <Route path="/courses-stub" element={<CoursesLayoutStub />} />

      <Route path="/test-me" element={<TestMePage />} />

      {/* End section demo routes */}

      {/* Public */}
      <Route path="/" element={<LandingPage />} />
      <Route path="/courses" element={<CoursesLayout />} />

      {/* Public system pages */}
      {/* Guard */}
      <Route path="/403" element={<ForbiddenPage />} />

      {/* Protected: everything that is not public */}
      <Route element={<RouteGuard />}>
        {/* Course area (tabs layout in MVP) */}

        <Route path="/courses/:courseId" element={<CourseLayoutTabs />}>
          <Route index element={<Navigate to="lectures" replace />} />
          <Route path="lectures" element={<CourseLecturesPage />} />
          <Route path="exercises" element={<CourseExercisesPage />} />
          <Route path="workshops" element={<PagePlaceholder title="CourseWorkshopSessions (Tab)" />} />
        </Route>

        {/* Student&Mentor: full-screen chat (outside tabs, no direct navigation) */}
        <Route path="/exercises/:exerciseId/dashboard" element={<ExerciseDashboard />} />

        {/* Mentor area */}
        <Route path="/mentor" element={<MentorLayout />}>
          <Route index element={<Navigate to="exercises-chats-inbox" replace />} />
          <Route path="exercises-chats-inbox" element={<MentorExerciseChatInbox />} />
          <Route path="workshop-manage" element={<PagePlaceholder title="Mentor · ManageWorkshop" />} />
          <Route path="hours-spent" element={<PagePlaceholder title="Mentor · EditHoursSpent" />} />
        </Route>

        {/* Mentor: full-screen (outside tabs, no direct navigation) */}
        <Route path="/mentor/courses/:courseId" element={<PagePlaceholder title="CourseLayout (Tabs wrapper)" />}>
          <Route path="workshops/:sessionId" element={<PagePlaceholder title="Course Workshop Session (Full-screen)" />} />
        </Route>


        {/* Admin area */}
        <Route path="/admin" element={<AdminLayout />}>
          <Route index element={<Navigate to="users-roles" replace />} />
          <Route path="hours-added" element={<PagePlaceholder title="Admin · EditHoursAdded" />} />
          <Route path="support-inbox" element={<PagePlaceholder title="Admin · SupportInbox" />} />
          <Route path="users-roles" element={<AdminManageUserRoles />} />
        </Route>

      </Route>

      {/* Fallback */}
      <Route path="*" element={<Navigate to="/courses" replace />} />

    </Routes>
  );
}
