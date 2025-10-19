# ADR 0013: XML-комментарии для Swagger

**Дата:** 2025-10-17  

## Почему  
Swagger использует XML-комментарии для генерации описаний контроллеров, методов и параметров.  
Для публичных методов и основных контроллеров рекомендуется, помимо атрибутов `[ProducesResponseType]`, добавлять подробное описание через XML-комментарии, чтобы улучшить читаемость и полноту API-документации.

## Решение  
Всегда включать генерацию XML (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`),  
чтобы в окружении **Development** Swagger корректно работал при любом типе сборки (Debug/Release).  

Подавлять предупреждение `CS1591` (`<NoWarn>$(NoWarn);1591</NoWarn>`),  
чтобы XML-документация не требовалась для всех публичных методов на этапе компиляции.

## Примечание  
[Поддерживаемые теги XML-документации — Microsoft Learn](https://learn.microsoft.com/ru-ru/aspnet/core/fundamentals/openapi/openapi-comments?view=aspnetcore-10.0&utm_source=chatgpt.com#supported-xml-documentation-tags)

Изменения внесены в рамках коммита "feat #37: add endpoint for registration & authorization", файлы:
- src\backend\Api\Identity\SetupIdentity.cs
- src\backend\Api\Api.csproj
