//import React from "react";
import { Navigate, Route, Routes } from "react-router-dom";

// Demo pages (keep)
import ManTineTest from '@/pages/ManTineTest';
import DemoPage from "@/pages/DemoPage";

/**
 * MVP routing (real structure), with a single reusable placeholder.
 * Auth screens are modals (non-routing), so no /login here.
 *
 * Replace <PagePlaceholder /> with real pages as you implement them.
 */
import { PagePlaceholder } from "@/shared/PagePlaceholder";

export function AppRoutes() {
  return (
    <Routes>
      {/* Keep demo routes */}
      <Route path="/mantinetest" element={<ManTineTest />} />
      <Route path="/test" element={<DemoPage />} />

      {/* Landing */}
      <Route path="/" element={<PagePlaceholder title="Landing" />} />

      {/* Main Page Public */}
      <Route path="/courses" element={<PagePlaceholder title="CourseList" />} />

      {/* Course area (tabs layout in MVP) */}
      <Route path="/courses/:courseId" element={<PagePlaceholder title="CourseLayout (Tabs wrapper)" />}>
        <Route index element={<Navigate to="lectures" replace />} />
        <Route path="lectures" element={<PagePlaceholder title="CourseLectures (Tab)" />} />
        <Route path="exercises" element={<PagePlaceholder title="CourseExercises (Tab)" />} />
        <Route path="workshop-sessions" element={<PagePlaceholder title="CourseWorkshopSessions (Tab)" />} />
      </Route>

      {/* Student&Mentor: full-screen chat (outside tabs, no direct navigation) */}
      <Route
        path="/courses/:courseId/exercises/:exerciseId/chat"
        element={<PagePlaceholder title="Student&Mentor · CourseExerciseChat (Full-screen)" />}
      />

      {/* Mentor area */}
      <Route path="/mentor" element={<PagePlaceholder title="MentorLayout (wrapper)" />}>
        <Route index element={<Navigate to="exercises/inbox" replace />} />
        <Route path="exercises/inbox" element={<PagePlaceholder title="Mentor · CourseExerciseChat Inbox" />} />
        <Route path="workshop-manage" element={<PagePlaceholder title="Mentor · ManageWorkshop" />} />
        <Route path="hours-spent" element={<PagePlaceholder title="Mentor · EditHoursSpent" />} />
      </Route>

      {/* Mentor: full-screen (outside tabs, no direct navigation) */}
      <Route path="/mentor/courses/:courseId" element={<PagePlaceholder title="CourseLayout (Tabs wrapper)" />}>
        <Route path="workshops/:sessionId" element={<PagePlaceholder title="Course Workshop Session (Full-screen)" />} />
      </Route>


      {/* Admin area */}
      <Route path="/admin" element={<PagePlaceholder title="AdminLayout (wrapper)" />}>
        <Route index element={<Navigate to="hours-added" replace />} />
        <Route path="hours-added" element={<PagePlaceholder title="Admin · EditHoursAdded" />} />
        <Route path="support/inbox" element={<PagePlaceholder title="Admin · SupportInbox" />} />
        <Route path="users-roles" element={<PagePlaceholder title="Admin · ManageUsers" />} />
      </Route>

      {/* Fallback */}
      <Route path="*" element={<Navigate to="/courses" replace />} />
    </Routes>
  );
}
