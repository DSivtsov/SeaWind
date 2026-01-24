# Frontend MVP — Реализованные страницы

## Назначение документа

Документ содержит сводный перечень страниц фронтенда, реализованных в рамках **Frontend MVP** проекта WorkshopCode.

Используется как справочный материал:
- для комментариев к PR;
- для быстрого обзора текущего покрытия экранов;
- для синхронизации frontend и backend команд.

Документ **не описывает UX и реализацию**, а фиксирует только факт наличия страницы и её назначение.

---

## Список страниц (Frontend MVP)

| URL | Component | Назначение | Примечания |
|-----|----------|------------|------------|
| `/` | `LandingPage.tsx` | Публичная landing page проекта | Использует Public Theme v0 |
| `/courses` | `CoursesLayout.tsx` | Страница списка курсов | Данные через `/api/courses` |
| `/courses/:courseId` | `CourseLayoutTabs.tsx` | Контейнер курса с вкладками | Общая инфраструктура вкладок курса |
| `/courses/:courseId/lectures` | `CourseLecturesPage.tsx` | Просмотр списка лекций курса | Данные через `/api/courses/{courseId}/lectures` |

### Модальные окна (доступны со страницы `/courses`)

| Component        | Назначение                 | Примечания                                                   |
| ---------------- | -------------------------- | ------------------------------------------------------------ |
| `AvatarMenu.tsx` | Меню пользователя в Header | Non-routing dropdown, использует `auth.me`, без auth-решений |
| `LoginModal.tsx` | Окно входа пользователя | Добровольное открытие, без forced-login |
| `RegistrationModal.tsx` | Окно регистрации пользователя | После регистрации — переход к логину |

---

## Тестовые страницы (Dev / MVP)

Тестовые страницы используются для проверки архитектурных решений и не являются частью пользовательского сценария.

| URL | Component | Назначение |
|-----|----------|------------|
| `http://localhost:5173/test-me` | `TestMePage.tsx` | Тестовая страница для проверки обработки `401 / 403` (Authorize API) |
| `http://localhost:5173/test-page` | `TestPage.tsx` | Пример React-страницы: App → Page → PageShell → Content (в одном файле) |
| `http://localhost:5173/testmantine` | `TestManTine.tsx` | Версия TestPage с Mantine-компонентами и UI-настройками (уровни вынесены в модули) |
| `http://localhost:5173/test/courses` | `TestCourses`, `LayoutTestCourseListPage` | Тестовая страница списка курсов внутри test-layout (Layout + Outlet) |
| `http://localhost:5173/courses-stub` | `CoursesLayoutStub.tsx` | Страница списка курсов с API-заглушкой |
| `http://localhost:5173/courses` | `CoursesLayout.tsx` | Страница списка курсов с реальным Backend API |

---

## Примечания

- Все страницы относятся к **Frontend MVP**.
- Доступность некоторых страниц зависит от состояния backend API.
- Тестовые страницы могут быть удалены или скрыты после стабилизации MVP.

---


