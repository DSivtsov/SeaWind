# ADR 0038: Chat write locking by threadId

## Почему или Контекст

В чате есть два типа операций записи:

- изменение статуса задания (StudentExercise.Status)
- отправка сообщений (Message + Thread.Seq)

Они выполняются независимо, но логически связаны.

Без синхронизации возникают проблемы:

- сообщение может быть записано уже после смены статуса, но “как будто до”
- статус может зафиксироваться, не учитывая только что записанное сообщение
- рассинхронизация между Status и Seq

Проблема не в одной таблице/коллекции, а между двумя источниками состояния:
- Postgres (Status)
- Mongo (Messages + Seq)

Нужно гарантировать, что эти операции не выполняются параллельно.

---

## Решение

Все write-операции сериализуются через единый lock по threadId.

Как это устроено:

- вводится lock-хранилище в Mongo
- ResourceId = threadId и хранится как _id (уникальность гарантируется Mongo)
- перед любой write-операцией берётся lock

Какие операции идут под lock:

- изменение StudentExercise.Status
- запись message
- изменение Thread.Seq

Поведение:

- если lock получен → операция выполняется
- если lock занят → возвращается 409 Conflict
- после выполнения lock всегда освобождается в finally

Проверка записи сообщения:

- определяется только текущим Status
- lockSeq не участвует в обычной записи
- lockSeq используется только для фиксации момента смены статуса

---

## Пример использования
Типичный flow на backend:
1. AcquireLock(threadId) — попытаться взять lock по `threadId`
2. Если lock не получен — вернуть `409 Conflict`
3. Выполнить write-операцию после получения lock (insert message / change status)
4. ReleaseLock(threadId) — всегда освободить lock в `finally`

```csharp
var locked = await lockService.AcquireLockAsync(threadId, lockOwnerId, ct);
if (!locked)
    throw new ConflictException("Чат сейчас занят другой write-операцией.");

try
{
    await performWriteOperationAsync(..., ct);
}
finally
{
    await lockService.ReleaseLockAsync(threadId, lockOwnerId, CancellationToken.None);
}
```

## Следствие

- Исключаются race conditions между Status и Message
- Упрощается модель (нет сложных проверок через seq)
- Все операции записи становятся линейными внутри threadId
- Нет серверных retry — управление конфликтами на клиенте
- Требование: операции внутри lock должны быть короткими (≈10–20 ms)
- Release lock обязан вызываться в finally
- CancellationToken не должен отменять Release

---

## Out of Scope

- Долгие операции внутри lock
- Retry / очереди на сервере
- Атомарность изменений между несколькими хранилищами (нет distributed transaction / rollback)
