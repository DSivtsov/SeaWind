# WorkshopCode · Чат упражнения — единая логика workflow, данных и message flow

Этот документ обновляет предыдущую combined-версию.

Здесь уже используется актуальная модель:
- `status`: `OnStudent | OnMentor`
- `mark`: `null | 0 | 1 | 2`
- один общий `lockSeq`

Документ нужен как единая опора для:
- workflow упражнения
- доступа к dashboard
- хранения данных в Postgres и Mongo
- optimistic UI в чате
- server ACK / refresh / merge

## 1. Общая идея

Это не обычный мессенджер.

Чат здесь — часть рабочего экрана упражнения.
Он живёт вокруг одного `StudentExercise` и подчиняется workflow этого упражнения.

Postgres хранит состояние процесса:
- кто сейчас должен действовать
- какая оценка выставлена
- кто из менторов ведёт проверку
- как попасть к нужному thread

Mongo хранит содержимое чата:
- thread
- сообщения
- вложения
- `seq`
- `lockSeq`

## 2. Экран и маршруты

Основной экран:

`/exercises/:exerciseId/dashboard`

Экран состоит из трёх логических частей:
- `ExerciseInfo`
- `ExerciseChat`
- `ExerciseChatStatus`

Основные входы:
- Student открывает dashboard из `CourseExercisesPage`
- Mentor открывает dashboard из `MentorExerciseChatInbox`

Маршрут inbox для ментора:

`/mentor/exercises-chats-inbox`

## 3. Access contract

### Student

Route:
- `/exercises/:exerciseId/dashboard`

Источник `studentId`:
- JWT

### Mentor

Route:
- `/exercises/:exerciseId/dashboard?studentId=UUID`

Источник `studentId`:
- query

Если `studentId` отсутствует или невалиден:
- backend возвращает `400 Bad Request`
- frontend показывает предметное сообщение
- после этого делает возврат назад, а если это невозможно — редирект в mentor inbox

## 4. Основные сущности

## 4.1 StudentExercise

Главная workflow-сущность в Postgres.

Минимальный состав:
- `studentExerciseId`
- `studentId`
- `exerciseId`
- `courseId`
- `status`: `OnStudent | OnMentor`
- `mark`: `null | 0 | 1 | 2`
- `requestedCheckAt`
- `checkedAt`
- `threadId`
- `assignedMentorId`

### Смысл полей

`status`:
- `OnStudent` — сейчас ход студента
- `OnMentor` — сейчас ход ментора

`mark`:
- `null` — итоговой оценки ещё нет
- `0` — не зачтено
- `1` — зачтено
- `2` — зачтено отлично

`threadId`:
- ссылка на thread в Mongo
- клиент не должен присылать `threadId` как источник истины

`assignedMentorId`:
- текущий ментор, который ведёт это упражнение
- нужен для ownership и conflict-логики

## 4.2 Mongo thread

Минимальный состав:
- `_id = threadId`
- `studentExerciseId`
- `lastMessageSeq`
- `lockSeq`
- `createdAt`

### Смысл полей

`lastMessageSeq`:
- атомарный счётчик server-side seq
- из него выдаётся следующий `ServerSeq`

`lockSeq`:
- общий глобальный seq в рамках thread
- фиксируется при передаче хода другой стороне

## 4.3 Mongo message

Минимальный состав:
- `_id`
- `threadId`
- `seq`
- `authorRole`: `Student | Mentor`
- `authorId`
- `text`
- `attachmentIds[]`
- `createdAt`
- `editedAt?`
- `isDeleted?`

## 4.4 Mongo attachment

Минимальный состав:
- `_id`
- `threadId`
- `messageId?`
- `contentUrl`
- `createdAt`
- `fileNameOriginal`
- `size`
- `contentType`

## 5. Ownership и mentor conflict

Student dashboard открывается всегда для самого студента.

Для Mentor действует ownership-модель:
- первый ментор, который успешно забрал упражнение в работу, становится `assignedMentorId`
- читать и вести thread может только этот ментор
- другой ментор получает conflict-сценарий

Если второй ментор пытается открыть dashboard:
- frontend показывает сообщение
- делает `navigate(-1)`, если есть куда возвращаться
- иначе делает redirect в `/mentor/exercises-chats-inbox`

## 6. Переходы workflow

## 6.1 Student → Mentor

Действие:
- студент отправляет работу на проверку

Переход:
- `OnStudent -> OnMentor`

Что обновляется:
- `status = OnMentor`
- `requestedCheckAt = now`
- `lockSeq = current last seq in thread`

Смысл:
- всё до `lockSeq` считается уже зафиксированной student-фазой
- дальше рабочий ход переходит к ментору

## 6.2 Mentor → Student

Действие:
- ментор завершает текущую проверку и возвращает работу студенту

Переход:
- `OnMentor -> OnStudent`

Что обновляется:
- `status = OnStudent`
- `mark = 0 | 1 | 2`
- `checkedAt = now`
- `lockSeq = current last seq in thread`

Смысл:
- всё до `lockSeq` считается уже зафиксированной mentor-фазой
- дальше рабочий ход снова у студента

Отдельного статуса `Accepted` больше нет.
Если работа зачтена, это выражается через `mark = 1 | 2`.

## 7. Как читать `lockSeq`

`seq` в thread всегда общий.
`lockSeq` тоже общий.

Практически модель читается так:
- `seq <= lockSeq` — уже зафиксированная предыдущая фаза
- `seq > lockSeq` — текущая активная фаза

Кто обновляет `lockSeq`:
- та сторона, которая передаёт ход другой стороне

## 8. Права писать в чат

## 8.1 При `OnStudent`

- Student: write allowed
- Mentor: read only

## 8.2 При `OnMentor`

- Mentor: write allowed
- Student: read only

Отдельного read-only статуса `Accepted` больше нет.
Ограничение на запись определяется текущим `status` и серверными правилами.

## 9. Правила edit / delete

Проверка делается на сервере.

Сообщение нельзя edit/delete, если:
- сообщение не принадлежит текущему автору
- текущая роль и статус не разрешают действие
- `seq <= lockSeq`

То есть базовое правило одно:
- всё, что уже попало в зафиксированную фазу, больше не редактируется и не удаляется

## 10. Что хранить в Postgres, а что не надо

Postgres хранит:
- workflow
- ownership
- итоговую оценку
- привязку к thread
- очередь проверки

Не надо хранить в Postgres как обязательную часть этой модели:
- `lastMessagePreview`
- `unreadCount`
- message cache для UI

Это можно добавлять отдельно, если когда-то появится отдельный сценарий для inbox-preview.

## 11. Message lifecycle

Ниже уже не workflow-часть, а message flow между client и server.

## 11.1 Load chat

1. Клиент запрашивает thread.
2. Сервер возвращает сообщения.
3. Клиент строит локальный список и вычисляет `lastSeq`.

`lastSeq` здесь — это последний подтверждённый server seq, который уже есть у клиента.

## 11.2 Client creates optimistic message

При отправке сообщения клиент сразу создаёт optimistic message в UI.

У него есть:
- временный client-side id
- client-side correlation key (`clientSeq` или аналог)
- временный status `sending`
- пока нет финального `MessageId`
- пока нет финального `ServerSeq`

Название client-side поля в коде не принципиально.
Это может быть:
- `clientSeq`
- `tmpSeq`
- другой локальный ключ

Важно только, что он нужен для confirm-update уже существующего optimistic message.

## 11.3 Server validates and saves message

Server flow:
1. валидирует автора
2. валидирует вложения
3. атомарно получает следующий `ServerSeq` через `lastMessageSeq`
4. сохраняет сообщение в Mongo
5. возвращает ACK

На сервере три сущности имеют разный смысл:
- `MessageId` — identity
- `ServerSeq` — порядок в thread
- `lastMessageSeq` — atomic counter

## 11.4 Server ACK

Ответ сервера должен вернуть достаточно данных, чтобы клиент подтвердил optimistic message.

Минимально:
- correlation key (`clientSeq` или аналог)
- `messageId`
- `serverSeq`
- `createdAt`

## 11.5 Confirm-update on client

Клиент получает ACK и:
1. находит optimistic message по correlation key
2. заменяет временные поля на серверные
3. переводит сообщение из `sending` в `sent`
4. обновляет `lastSeq`

## 12. Refresh / merge flow

Один ACK не гарантирует, что у клиента уже есть все промежуточные сообщения.

Если между отправкой и ACK появились сообщения из:
- другой вкладки
- другого устройства
- другого клиента

клиент должен подтянуть пропущенные записи.

## 12.1 Refresh after `lastSeq`

Клиент запрашивает сообщения:
- `Seq > lastSeq`

Сервер возвращает новые server-side записи.

## 12.2 Merge rules

Клиент объединяет серверные сообщения с локальным списком так:
1. если серверное сообщение соответствует уже существующему optimistic message — optimistic message заменяется
2. если такого сообщения локально ещё нет — оно добавляется
3. после этого итоговый список заново собирается по `ServerSeq`
4. `lastSeq` обновляется до максимального подтверждённого `Seq`

Это важно.
Здесь не надо делать простой append в конец.
Правильная модель — merge + rebuild ordered list by `Seq`.

## 13. Почему message flow и workflow не надо смешивать

Есть две разные зоны логики.

### Чат отвечает за:
- загрузку сообщений
- optimistic message
- ACK confirm-update
- refresh / merge
- локальное отображение списка

### Экран статуса отвечает за:
- `Send to Check`
- `Return to Student`
- mark
- ownership
- conflict handling
- enable/disable action-кнопок

Чат может наружу отдавать нужные данные, но не должен сам решать workflow-кнопки.

## 14. Inbox и dashboard flow

## 14.1 Mentor inbox

`MentorExerciseChatInbox` делает запрос:
- `/api/mentor/exercise-chats/inbox`

В ответ приходит список упражнений с разными `studentId`.

Из inbox ментор открывает:
- `/exercises/:exerciseId/dashboard?studentId=UUID`

## 14.2 Student flow

Student открывает dashboard без query-параметра.
`studentId` берётся из JWT.

## 15. Финальные инварианты

Ниже короткий список того, что должно оставаться истинным.

1. У одного thread один общий глобальный `seq`.
2. У thread один общий `lockSeq`.
3. `lockSeq` обновляется при передаче хода другой стороне.
4. `seq <= lockSeq` относится к уже зафиксированной предыдущей фазе.
5. `status` имеет только два значения: `OnStudent | OnMentor`.
6. `mark` хранит результат проверки и не требует отдельного статуса `Accepted`.
7. `MessageId`, `ServerSeq` и client correlation key — это разные сущности с разным смыслом.
8. Порядок сообщений определяется только server-side seq.
9. Клиент после refresh делает merge и пересборку списка по `Seq`.
10. `threadId` не должен быть клиентским источником истины для доступа к чужому чату.
11. Читать и вести thread может только текущий `assignedMentorId`.
12. Dashboard — это рабочий экран упражнения, а не просто отдельный chat widget.

## 16. Краткий итог

### Postgres

Хранит:
- workflow
- ownership
- оценку
- времена переходов
- ссылку на thread

### Mongo

Хранит:
- thread
- сообщения
- вложения
- `lastMessageSeq`
- `lockSeq`
- `seq`

### Client

Отвечает за:
- optimistic UI
- confirm-update after ACK
- refresh / merge
- итоговый ordered list by `ServerSeq`
