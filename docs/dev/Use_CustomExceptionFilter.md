# Использование CustomExceptionFilter

**Date:** 2025-10-17  
**Status:** ℹ️ Reference

Как правильно разрабатывать и описывать методы контролеров (слой Api)  с учетом использования  `CustomExceptionFilter`

## Коротко
кидаем исключения → их перехватывает `CustomExceptionFilter` → он формирует единый JSON (`ResponseDtoBase`) и статус (в т.ч. 409) → Swagger документируем через `[ProducesResponseType]` (или глобальный фильтр).

## Подробно

### 1. Выдача ошибок из метода (НЕ возвращаем `BadRequest/Conflict` вручную)
------------------------------------------------------------------------

*   Валидационные/прочие ошибки → бросаем свои исключения:
    *   `throw new BadRequestException("...");`
    *   `throw new ConflictException("...");` // для дубликатов (409)
    *   `throw new NotFoundException("...");` и т.д.
*   Логику статусов держим в фильтре (`switch` по типам исключений):
    *   `BadRequestException` → 400
    *   `ConflictException` → 409
    *   `NotFoundException` → 404
    *   default → 500
*   Фильтр сериализует **единый контракт**:
    ```csharp
    new ResponseDtoBase { ErrorMessage = context.Exception.Message }
    ```

    **Примечание**.
    - Если не хватает уже созданных создаем дополнительные по аналогии в `src\backend\Api\Exceptions\` и подключаем к  `CustomExceptionFilter` (метод `OnException`)
    - Пример использования см. внутри методов `AuthController` и `TestersController`


### 2. Описание ответов для Swagger (документация)
----------------------------------------------

Swagger не «видит» код фильтра, поэтому ему нужно явно сказать, какие ответы возможны и какой у них **schema**.

### Вариант A — атрибуты на методе (просто)

```csharp
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ResponseDtoBase), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ResponseDtoBase), StatusCodes.Status409Conflict)]
```

> Даже если метод возвращает `IResult`, для ошибок указываем `ResponseDtoBase`, потому что **его** отдаёт фильтр.


### Что это дает
*   Разделяем ответственность: **контроллер** бросает исключение, **фильтр** решает статус/тело ответа.
*   Единый формат ошибок (`ResponseDtoBase`) упрощает фронт и тесты.
*   Swagger требует явной схемы для ошибок, потому что он не анализирует код фильтра.