## Guards rules (MVP)

Ниже — артефакт “правила + поведение + required data”, как ты описал.

### Required Data для guard’ов

`GET /api/users/me` возвращает минимум:

* `isAuthenticated: boolean`
* `role: "Guest" | "FreeStudent" | "Student" | "Mentor" | "Admin"`
* `userId: string | null`

Этого достаточно, чтобы решить “пускать/не пускать”.

---

# Матрица доступа по route patterns
## Public (без авторизации) (GUEST):
* `/` (Landing)
* `/courses`

## Auth only (любой авторизованный пользователь):
все остально только с автопризацией
FreeStudent/Student/Mentor/Admin
* `/courses/:courseId/lectures`
* `/courses/:courseId/exercises`
* `/profile/*` (ProfileLayout)

## Student/Mentor/Admin:
* `/courses/:courseId/workshops`

## Student/Mentor:
* `/courses/:courseId/exercises/:exerciseId/chat` (CourseExerciseChat)

Mentor only:

* `/mentor/*`

Admin only:

* `/admin/*`

> Non-routing modals (AvatarMenu, SupportChatModal, Auth modals) **не являются routes**, guard’ами по URL не защищаются.

---

### Поведение при запрете

Общее правило (MVP-упрощение): **редирект на `/courses`**.

1. Not authenticated → redirect `/courses`
!isAuthenticated → navigate("/courses", { replace: true })

2. Wrong role → 403
isAuthenticated && !roleAllowed → navigate("/403", { replace: true }) (или ErrorPane(code=403) если нет отдельного URL)

MVP-правило для 403
- Делай отдельный routing screen: /403 (Forbidden).
- UI минимум: “403 Forbidden” + кнопка Back to courses → /courses.

---

### Что реализуем в коде (структура)

## AuthProvider/store

* `meLoading: boolean`
* `isAuthenticated: boolean`
* `role: Role` (лучше хранить `"Guest"` даже когда не автhed, чтобы не плодить `null`)
* `userId: string | null`
* `meError: boolean` (или `meStatus: "loading" | "ready" | "error"`)
  Потому что без этого непонятно, что делать, если `/api/users/me` упал: **в MVP трактуем как guest** и снимаем `meLoading`.

## RouteGuard

1. `meLoading === true` → показываем **layout-level loading**
2. `meLoading === false`:

   * матчим текущий path по patterns → получаем `required`
   * если `required === "public"` → ok
   * если `required !== "public"` и `!isAuthenticated` → redirect `/courses`
   * если `isAuthenticated` но `role` не входит в allowed → redirect `/403`
   * иначе render children

## “request login modal”
МVP вариант: редирект на `/courses`, а там кнопка Login.
* defaults: `auth-only`
* exceptions: список public + список role-guards

## **MVP**:
1. **Порядок match’а**
   Явный порядок в конфиге, **сверху вниз**. Первый совпавший — применяется.

2. **Fallback для неизвестных routes**
   Считать **auth-only**.
   Гость → redirect `/courses`.

3. **`meError`**
   Считать пользователя **Guest**.
   `meLoading = false`, без ретраев, без модалов.

4. **Роли**
   Фиксированный enum:
   `Guest | FreeStudent | Student | Mentor | Admin`.

5. **Служебные экраны**
   Есть `/403`.
   `/404` — **не требуется для MVP**.

