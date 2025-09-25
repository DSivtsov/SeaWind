# Дополнительная информация к ER-диаграмме (MVP)
**Version:** v1
**Date:** 2025-09-24  
**Status:** In Review  


Документ дополняет основную диаграмму **WorkshopCode_ER_MVP**.  
Все ссылки на пользователей в MVP идут через **IDENTITY_USER** (виртуальная таблица — Identity-managed tables).

---

### 1. **UserData**
- ❌ Не используется в MVP.  
- Все бизнес-сущности ссылаются напрямую на `IDENTITY_USER.Id`.  
- UserData может появиться позже (NotMVP), когда нужно будет вынести расширенные профили (аватар, соцсети, язык, timezone).  

---

### 2. **Course**
- Хранит описание курса.  
- Особенности:  
  * `Id` — PK.  
  * `Code` — UK (slug для фронтенда и линков).  
  * `CreatedAt`, `UpdatedAt` — аудит, сортировка, кеш-инвалидация.  

---

### 3. **Lecture**
- Хранить описание лекции в курсе.  
- Особенности:  
  * UK(`CourseId`, `OrderNo`) — уникальность порядка внутри курса.  
  * `Status` — check(`Draft`/`Published`):  
    - Draft → можно создать лекцию без `VideoUrl`;  
    - Published → требует `VideoUrl`.  
  * `OrderNo` — гарантированный порядок отображения (drag-and-drop, пагинация).  

---

### 4. **Exercise**
- Описание задания в рамках курса.  
- Особенности:  
  * UK(`CourseId`, `OrderNo`) — уникальность порядка внутри курса.  
  * `OrderNo` — детерминированная сортировка и безопасные перестановки.  

---

### 5. **StudentCourseExercise**  _(объединение Journal + StudentExercise)_
- Строка описывает выполнение **конкретного задания** студентом **в рамках конкретного курса**.  
- Особенности:  
  * `Id` — PK (surrogate key, `uuid`).  
  * FK(`UserId`)   → `IDENTITY_USER.Id`, **ON DELETE RESTRICT**.  
  * FK(`CourseId`) → `Course.Id`, **ON DELETE CASCADE**.  
  * FK(`ExerciseId`) → `Exercise(CourseId, Id)` — **составная ссылка** вместе с `CourseId` (гарантирует принадлежность задания курсу).  
  * **UK(`UserId`, `CourseId`, `ExerciseId`)** — уникальность выполнения задания студентом в курсе.  
  * `Status` — check(`NotDone`/`Done`).  
  * `ChatStorageKind` — check(`mongo`/`s3`/`fs`/`url`), **default = 'mongo'**.  

---

### 6. **MentorWork**
- Фиксация работы ментора (воркшоп или проверка задания).  
- Особенности:  
  * `Id` — PK.  
  * `MentorId` — FK → `IDENTITY_USER.Id`, **ON DELETE RESTRICT**.  
  * `WorkType` — ENUM: `Workshop` \| `ExerciseReview`.  
  * `StudentCourseExerciseId` — FK → `StudentCourseExercise.Id`, **NULL когда `WorkType='Workshop'`**; **ON DELETE RESTRICT**.  
  * `WorkshopSessionId` — FK → `WorkshopSessions.Id`, **NULL когда `WorkType='ExerciseReview'`**; **ON DELETE RESTRICT**.  
  * `StartedAt` / `EndedAt` — контроль целостности: `EndedAt IS NULL OR EndedAt > StartedAt`.  
  * `Hours` — может вычисляться как `(EndedAt-StartedAt)` (материализуется или считается в отчётах).  
  * `WorkedAt`, `CreatedAt` — аудит.  

---

### 7. **StudentTopUp**
- Пополнение баланса студентом (ручной ввод админом).  
- Особенности:  
  * `Id` — PK.  
  * `StudentId` — FK → `IDENTITY_USER.Id`.  
  * `AdminId` — FK → `IDENTITY_USER.Id`.  
  * `HoursAdded > 0` (CHECK).  

---

### 8. **StudentBalance**
- Вычисляемое представление (VIEW).  
- Поля: `StudentId`, `HoursAdded`, `HoursSpent`, `HoursRemaining`.  
- Источник данных: `MentorWork` и `StudentTopUp`.  

---

### 9. **AspNetIdentity**
- Абстрактный контейнер.  
- Базовая таблица: **IDENTITY_USER** (Identity).  
- Все доменные сущности используют FK → `IDENTITY_USER.Id` (`Guid/uuid`).  
- MVP-вариант: `AppUser : IdentityUser<Guid>` (расширяемый класс).  

---

### 10. **WorkshopSessions**
- Сущность для проводимых воркшопов.  
- MVP: минимальное описание (`Id`, `Title`, `Description`, `DateTime`).  

## Change Log  
- v1 (2025-09-24) —  версия для WorkshopCode_ER_MVP v2 