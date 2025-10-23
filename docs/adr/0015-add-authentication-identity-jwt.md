# ADR-0015: Подключение аутентификации на базе ASP.NET Identity + JWT

## Почему

Для обеспечения безопасного доступа к API необходимо реализовать механизм аутентификации пользователей. 
Выбранное решение — **ASP.NET Identity + JWT (JSON Web Token)** — обеспечивает:

- хранение учётных данных в базе данных PostgreSQL;
- стандартные механизмы регистрации и входа без UI;
- интеграцию с DI и контроллерами ASP.NET Core;
- возможность расширения в будущем для поддержки ролей и авторизации (out of scope данного ADR).
- подходит для SPA и API с CORS.
- формирует полную инфраструктуру аутентификации пользователей для MVP, обеспечивая базу для последующей реализации авторизации и ролей.

## Решение
### Описание концепции работы решения
В разделе **wiki** [Аутентификация и авторизация (ASP.NET Identity + JWT)](https://github.com/DSivtsov/SeaWind/wiki/Reference_Index#-%D0%B0%D1%83%D1%82%D0%B5%D0%BD%D1%82%D0%B8%D1%84%D0%B8%D0%BA%D0%B0%D1%86%D0%B8%D1%8F-%D0%B8-%D0%B0%D0%B2%D1%82%D0%BE%D1%80%D0%B8%D0%B7%D0%B0%D1%86%D0%B8%D1%8F-aspnet-identity--jwt) представлено описание совместной работы ASP.NET Identity и JWT, а также объяснение работы Bearer Authentication в ASP.NET Core

### Основные параметры
* **Контекст БД:** `AppIdentityDbContext` (схема `identity` в БД Postgres).
* **Модель пользователя:** `AppUser`, унаследованная от `IdentityUser`.
* **Аутентификация:** JWT Bearer с маппингом claim’ов ASP.NET Identity для совместимости.

## Сервис Api
Основное конфигурирование  **ASP.NET Identity + JWT** осуществляется в проекте Api

### JWT-конфигурация
Секретные параметры используемые для формирования JWT задаются в `appsettings.Development.json`.
Раздел `Jwt` в конфигурации содержит параметры:

```json
"Jwt": {
  "Issuer": "WorkshopCode.API",
  "Audience": "WorkshopCode.Frontend",
  "Key": "секретный_ключ_для_подписи_JWT"
}
```

- **Issuer** — идентификатор сервера, выпускающего токены.
- **Audience** — идентификатор получателя (frontend-приложение).
- **Key** — симметричный ключ для подписи токенов.

Примечание. На production-этапе секретный ключ должен задаваться через переменные окружения (`ENV`).

### Интеграция в API
Identity и JWT регистрируются в DI и подключаются в `Program.cs` в едином виде:

```csharp
builder.Services
    .AddWorkshopIdentity()
    .AddJwtAuth(builder.Configuration)
    .AddSwaggerWithJWT();
```

---

| Метод                   | Назначение                                                                                                                                                                                |
| ----------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **AddWorkshopIdentity** | Регистрирует базовые сервисы **ASP.NET Core Identity** без UI (`UserManager`, `SignInManager` и др.); используется для управления пользователями — регистрация, вход, смена пароля и т.п. |
| **AddJwtAuth**          | Настраивает **JWT-аутентификацию** — сервер проверяет пользовательские JWT-токены; регистрирует в DI общий ключ для создания и подписи токенов.                                           |
| **AddSwaggerWithJWT**   | Подключает в **Swagger** поддержку аутентификации через JWT-токен с использованием схемы `"bearer"`.                                                                                      |

---

Middleware настроен в `PresentationDI.cs`:
ASP.NET Identity сервисы - между UseRouting() и MapControllers() и первым должен идти UseAuthentication(), а также после Routing но до MapControllers();
```csharp
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```
### AuthController
Содержит основные роут [HttpPost("login")] & [HttpPost("register")]

#### Регистрация  [HttpPost("register")]
Необходимо задать в качестве входного имени email и пароль в соответствие с требованиями.
Информация будет храниться в таблицах `AppIdentityDbContext`

#### Аутентификация [HttpPost("login")] 
После успешной аутентификации через роут [HttpPost("login")] в AuthController вызывается сервис `TokenService` (Api\Identity\TokenService.cs), для генерация  JWT-токен со следующими claim’ами.
- `sub` — идентификатор пользователя;
- `email` — e-mail пользователя.

При генерации токен **подписывается** симметричным ключом (через DI) с использованием такого же симметричного ключа (через DI), что и при конфигурации сервиса аутентификации (AddJwtAuth)

Срок жизни токена — **1 час**, необходимо дополнительно реализовывать механизм **refresh-токенов**, или будет требоваться заново пройти аутентификацию.

## Сервис Infrastructure.Postgres

### Задание для сервиса места хранения таблиц
При конфигрировании сервисов `ASP.NET Core Identity ` в Infrastructure.Postgres\AddIdentityDb.cs определяется `AppIdentityDbContext`  где будут храниться его таблицы через `AddPostgresIdentityStores(this IdentityBuilder builder)`

### Хранение данных и миграции
- Все таблицы Identity создаются в схеме `identity` (`AppIdentityDbContext`)
- `AppIdentityDbContext` наследуется от IdentityDbContext<AppUser, IdentityRole, string> и содержит стандартные таблицы `ASP.NET Core Identity `
- Миграции для `AppIdentityDbContext` размещаются в каталоге `Infrastructure.Postgres\Identity\Migrations`.
- Результаты миграций в таблице "__EFMigrationsHistory", schema: "identity"

## Использование решения
### Настройка проекта Api
1. Добавить секцию `Jwt` в конфигурационный файл `appsettings.Development.json`.
2. Для защиты эндпоинтов применять атрибут `[Authorize]`. (без указание роли требуется только пройти аутентификацию и получить JWT-токен)

### Настройка фронтенда
После прохождения аутентификации сервис Api пришлет JWT-токен его необходимо сохранить в хранилище браузера
и отправлять при необходимости доступа к эндпоинтам закрытым атрибутом `[Authorize]`
Токен вставляется в заголовок `Authorization: Bearer <token>`.

### Использование через Swagger Integration
После прохождения аутентификации сервис Api и получения JWT-токен, его необходимо вставить в UI Swagger `Authorize`, т.к. используется схема "bearer".
То необходимо только вставлять ТОЛЬКО token (без "Bearer "), "Bearer " - вставлять в поле не надо

## Out of Scope
- Авторизация по ролям и политикам (`[Authorize(Roles=...)]`).
- Создание и управление ролями (`IdentityRole`).
- Подтверждение e-mail и восстановление пароля.
- Механизм refresh-токенов.
- Механизм logout (аннулирование токена) — планируется как часть авторизации.
