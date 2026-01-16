# ADR-0031: Guard & Routes (Auth + Role-based access)

## Context
В приложении требуется единый, предсказуемый механизм контроля доступа по URL.
Ранее логика доступа могла быть размазана между UI, роутами и страницами, что усложняло сопровождение и повышало риск ошибок.

Необходимо зафиксировать простую и детерминированную модель:
- какие маршруты считаются public;
- какие маршруты требуют аутентификации;
- где и как принимается решение «пускать / не пускать»;
- как учитываются роли пользователя.

## Decision

### Общий принцип
Guard отвечает только за одно: **всё, что не public, не должно быть доступно без корректного доступа**.

Guard является **единственной точкой принятия решения** о доступе по URL.
UI может скрывать элементы навигации, но доступ по прямому адресу определяется только guard.

### Структура маршрутов

**Public routes (без guard):**
- `/` (Landing)
- `/courses` (Courses list)

**Demo / Test routes (без guard):**
- `/test*`, `/courses-stub` и прочие тестовые страницы

**Protected routes (под guard):**
- `/courses/:courseId/*`
- `/courses/:courseId/exercises/:exerciseId/chat`
- `/mentor/*`
- `/admin/*`
- `/profile/*`

**Fallback:**
- `*` маршрут расположен в конце и редиректит на `/courses`, не обходя guard.

### Guest
Guest не является ролью.
Guest определяется логически как состояние `!auth.isAuthenticated`.

### Источник истины
Guard использует только данные из `AuthProvider`:
- `auth.isAuthenticated`
- `auth.me` (включая `role`), загружаемый через `GET /api/users/me`

### Правило для `/api/users/me`
- Эндпоинт защищён только `[Authorize]`
- Запрещены role-based ограничения (`[Authorize(Roles=...)]`, policy)
- Следствия:
  - `401` — только при невалидном или отсутствующем токене
  - `403` — никогда не возвращается из `/api/users/me`
  - `me.kind === "error"` на фронте означает реальную серверную/сетевую проблему

### Матрица доступа (role-check в guard)
Решение принимается по `location.pathname` (первое совпадение сверху вниз):
- `/admin/*` → `Admin`
- `/mentor/*` → `Mentor`
- `/courses/:courseId/exercises/:exerciseId/chat` → `Student | Mentor`
- `/courses/:courseId/workshop-sessions/*` → `Student | Mentor | Admin`
- `/courses/:courseId/(lectures|exercises)/*` → auth-only
- `/profile/*` → auth-only
- неизвестные guarded URL → auth-only

### Поведение Guard
1. `!auth.isAuthenticated` → redirect `/courses`
2. `auth.me.kind === "loading"` → блокирующий placeholder `Loading`
3. `auth.me.kind === "ready"`:
   - если роль не удовлетворяет requirement → redirect `/403`
   - иначе → render `<Outlet />`
4. Любые прочие / нештатные состояния (`error`, защитный `empty`) →
   - единый fallback: `PagePlaceholder("Error...")`
   - без redirect, чтобы избежать циклов

## Consequences
- Логика доступа централизована и легко проверяема
- Исключены скрытые переходы и расхождения между UI и URL-доступом
- Role-based UX становится предсказуемым
- Guard остаётся простым, без дублирующих проверок и состояний
