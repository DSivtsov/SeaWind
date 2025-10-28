# Использование CustomExceptionFilter

**Date:** 2025-10-28  
**Status:** ✅ Актуализировано по [ADR-0016-unified-exception-handling.md](../adr/ADR-0016-unified-exception-handling.md)

Как правильно разрабатывать и описывать методы контроллеров (слой API) с учётом использования `CustomExceptionFilter` и единого формата ошибок **ProblemDetails (RFC 7807)**.

---

## Коротко
Бросаем исключения → их перехватывает `CustomExceptionFilter` → фильтр формирует единый JSON в формате **ProblemDetails** и соответствующий HTTP-статус → Swagger документируется через `[ProducesResponseType]` (или глобальный фильтр).

---

## 1. Выдача ошибок из метода (НЕ возвращаем `BadRequest/Conflict` вручную)

* Валидационные и бизнес-ошибки оформляем через собственные исключения:
  * `throw new BadRequestException("Некорректные данные");`
  * `throw new ConflictException("Такой объект уже существует");`
  * `throw new NotFoundException("Элемент не найден");` и т.д.

* Фильтр сам преобразует исключения в `ProblemDetails`, используя коды из `ExceptionToStatusCodeMap.cs`:
  * `BadRequestException` → 400
  * `ConflictException` → 409
  * `NotFoundException` → 404
  * прочие → 500

* Пример формирования ответа фильтром:

```csharp
var problem = _problemDetailsFactory.CreateProblemDetails(
    context.HttpContext,
    statusCode: StatusCodes.Status400BadRequest,
    title: "Bad Request",
    detail: ex.Message,
    instance: context.HttpContext.Request.Path);
```

---

## 2. Документирование ответов для Swagger

Swagger не анализирует код фильтра, поэтому нужно явно указать возможные коды ответов и их тип:

### Вариант A — аннотации на методе

```csharp
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
```

> Даже если метод возвращает `IResult` или `ActionResult`, для ошибок указываем `ProblemDetails`, потому что именно его возвращает фильтр.

---

## 3. Что это даёт

- Контроллер **только бросает исключение**, без ручного формирования ответов.  
- `CustomExceptionFilter` **сам определяет статус и тело ответа** в формате RFC 7807.  
- Swagger показывает корректные схемы ошибок (`ProblemDetails`, `ValidationProblemDetails`).  
- Тесты и фронтенд получают **предсказуемую структуру JSON** независимо от источника ошибки.  

---

📘 **См. также:**  
- [ADR-0016-unified-exception-handling.md](../adr/0016-unified-exception-handling.md) — описание архитектурного решения.  
- [ExceptionHandling.md](https://github.com/DSivtsov/SeaWind/wiki/Exception_handling) — общее руководство и структура конвейера обработки ошибок.

