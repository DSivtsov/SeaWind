# WorkshopCode — SiteMap (MVP)

## Таблица экранов и маршрутов

| SCREEN | URL | Layout | Type | Entry |
| --- | --- | --- | --- | --- |
| Landing | / | Full-screen | Routing | Direct / External |
| Course List | /courses | CoursesLayout | Routing | Landing |
| Course Lectures Tab | /courses/:courseId/lectures | CourseLayoutTabs | Routing | Course List |
| Course Exercises Tab | /courses/:courseId/exercises | CourseLayoutTabs | Routing | Course List |
| Course Workshop Tab | /courses/:courseId/workshop-sessions | CourseLayoutTabs | Routing | Course List |
| Student&Mentor · CourseExerciseChat | /courses/:courseId/exercises/:exerciseId/chat | CourseExerciseChat (Full-screen) | Routing | Course Exercises Tab, MentorExerciseChat |
| AvatarMenu | - | Dropdown Menu | Modal | Any screen (Header avatar click) |
| Auth Modal (Registartion) | - | Overlay (Auth Modal) | Modal | Course List |
| Auth Modal (Login) | - | Overlay (Auth Modal) | Modal | Course List |
| Auth Modal (Forget Password) | - | Overlay (Auth Modal) | Modal | Login modal |
| Mentor · CourseExerciseChat Inbox | /mentor/exercises/inbox | MentorLayout | Routing | Course List, MentorManageWorkshop, MentorTimeEntries |
| MentorManageWorkshop | /mentor/workshop-manage | MentorLayout | Routing | MentorExerciseChat, MentorTimeEntries |
| Mentor · EditHoursSpent | /mentor/hours-spent | MentorLayout | Routing | MentorExerciseChat, MentorManageWorkshop |
| Mentor · Course Workshop Session | /mentor/courses/:courseId/workshops/:sessionId | CourseLayoutTabs | Routing | MentorManageWorkshop |
| Mentor Select Student Course | - | Overlay | Modal | MentorManageWorkshop |
| Admin · EditHoursAdded | /admin/hours-added | AdminLayout | Routing | Course List, AdminSupportChat, AdminManageUsers |
| Admin · SupportChat | /admin/support/inbox | AdminLayout | Routing | AdminManageUsers, AdminManageTime |
| Admin · EditUserRoles | /admin/users-roles | AdminLayout | Routing | AdminSupportChat, AdminManageTime |
| ProfileChangeContact | /profile/contact | ProfileLayout | Routing | Course List, ProfileChangePassword |
| ProfileChangePassword |  /profile/password | ProfileLayout | Routing | ProfileChangeContact |
| Support Chat Modal | - | Overlay | Modal | Any screen (Footer support click) || * | - | all layouts | Global navigation | All Courses (Header) → /courses |

## Правила использования

- Этот файл является **единственным источником истины** по структуре экранов.
- Все Layout, Guards и Routing обязаны соответствовать этому документу.

