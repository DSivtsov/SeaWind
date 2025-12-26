# ADR-0025: Refactoring Course Identifier (UUID → Natural String PK)

**About**
Данный ADR фиксирует решение об отказе от UUID в сущности `Course`
в пользу natural string primary key (`string(32)`), а также описывает
подход к миграции на новую структуру данных в рамках MVP
(через сброс схемы и автоматическое применение миграций с сидированием).

### Context

Во фронтенде используются маршруты вида:

*   `/courses/:courseId`
*   `/courses/:courseId/lectures`
*   `/courses/:courseId/exercises`

Этот идентификатор:

*   постоянно присутствует в URL
*   фигурирует в логах, багрепортах, демо и обучающих материалах
*   используется командой в повседневной коммуникации

Изначально в `Course` использовался UUID как primary key.

### Problem

Использование UUID в качестве идентификатора курса приводит к практическим проблемам:

*   URL становятся нечитаемыми и неудобными
    (`/courses/8c5d0c9d-8baf-4b72-9c4f-3e8b3d7f2a41`)
*   сложно передавать и воспринимать такие ссылки (голосом, глазами, в переписке)
*   в логах и багрепортах UUID не несёт полезной информации
*   UUID ухудшает восприятие UI и демо-данных

UUID остаётся нормальным техническим решением как внутренний ключ БД,
но в данном домене он **просачивается наружу** и даёт сложность без реальной пользы.

### Decision

Принято решение **полностью отказаться от UUID** и использовать один идентификатор:

*   `Course.Id` — **natural primary key**
*   тип: `string(32)`
*   формат: строго `lower`
*   допустимые символы: `a-z0-9-`

Этот идентификатор используется:

*   как PK в БД
*   в API
*   в маршрутах фронтенда
*   в ссылках, логах и документации

Примеры:

*   `/courses/csharp-basics`
*   `/courses/net8-webapi`

Почему `string(32)`

*   достаточный запас уникальности
*   разумная длина URL
*   не создаёт искусственных ограничений (8–16 символов)
*   формат легко валидируется регулярным выражением

### Migration Strategy (MVP)

Чтобы **не усложнять миграции** при смене структуры `Course`
(UUID → natural string PK), в **MVP** принято более простое и прозрачное решение:

*   сбросить текущую схему `main`
*   пересоздать её одной “чистой” миграцией
*   при запуске сервера:
    *   EF автоматически применит миграции
    *   выполнится автоматическое сидирование демо-данных

Это снижает риск ошибок в сложных ALTER-миграциях и ускоряет работу команды.

#### Dev / Demo workflow

Удаление схемы `main` (только `MainDbContext`) внутри контейнера Postgres:

```powershell
docker ps
docker exec -i <CONTAINER_ID> `
  psql -U wc_app -d workshopcode -c "DROP SCHEMA main CASCADE;"
```

Пример:

```powershell
PS H:\_ASP.NET\SeaWind> docker ps
CONTAINER ID   IMAGE         COMMAND                  CREATED      STATUS                    PORTS                                           NAMES
6852aa22e326   postgres:16   "docker-entrypoint.s…"   2 days ago   Up 38 minutes (healthy)   0.0.0.0:55432->5432/tcp, [::]:55432->5432/tcp   workshopcode-db-1

PS H:\_ASP.NET\SeaWind> docker exec -i 6852aa22e326 `
  psql -U wc_app -d workshopcode -c "DROP SCHEMA main CASCADE;"
NOTICE:  drop cascades to 2 other objects
DETAIL:  drop cascades to table main."__EFMigrationsHistory"
drop cascades to table main."Courses"
DROP SCHEMA
```

**Примечание:**
Это удаляет **все данные** в схеме `main`, включая `__EFMigrationsHistory`.
Подходит только для **dev / demo окружений**, где данные полностью восстанавливаются сидированием.

Consequences
------------

Плюсы:

*   проще модель данных
*   понятные маршруты и ссылки
*   меньше миграционной сложности в MVP
*   быстрее разработка и поддержка

Минусы / ограничения:

*   подход применим для текущего домена и MVP
*   для production-scale сценариев может потребоваться пересмотр стратегии идентификаторов
