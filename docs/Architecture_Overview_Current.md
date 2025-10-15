# Architecture Overview — WorkshopCode (Current)
**Version:** v5
**Date:** 2025-10-13

## 🧾 About
 Описывает решения принятые в процесс разработки проекта.
Данные решения принимались на митинга проектной команды либо в рамках переписки в чаты проекта.
Источники
- протоколы митингов
- закрепленные сообщения чаты проекта.
- ADR записи в директории [doc/adr](https://github.com/DSivtsov/SeaWind/tree/develop/docs/adr) репо

## 📑 Содержание
- [Обзор Technology Stack проекта WorkshopCode](#обзор-technology-stack-проекта-workshopcode)
- [Архитектура сервисов приложения WorkshopCode](#архитектура-сервисов-приложения-workshopcode)
- [Общая структура приложения](#общая-структура-приложения)
- [Контейнеризация и база данных](#контейнеризация-и-база-данных)
- [Правила добавления новых контроллеров и методов действий](#правила-добавления-новых-контроллеров-и-методов-действий)
- [Централизованная обработка исключений](#централизованная-обработка-исключений-exception-handling-middleware)
- [Data Protection](#data-protection)
- **Описание архитектуры фронтенда**
  - [Общая информация об архитектуре фронтенда](#общая-информация-об-архитектуре-фронтенда)
  - [Ключевые экраны](#ключевые-экраны)
  - [Стек и ограничения](#стек-и-ограничения)
  - [Файловая структура фронтенда проекта](#файловая-структура-фронтенда-проекта)
  - [Совместная работа фроненда и бекенда](#совместная-работа-фроненда-и-бекенда)
  - [Middleware и порядок в Program.cs](#middleware-и-порядок-в-programcs)
- [Change Log](#change-log)

## Обзор Technology Stack проекта WorkshopCode

### Основные решения и технологии
* **Backend:** ASP.NET Core Web API (.NET 8)
* **Frontend:** React + Vite (Node.js LTS v22.19.0)
* **Database:**
  * PostgreSQL — основная реляционная база
  * MongoDB — NoSQL для чатов и вложений
* **Cache:** Redis (сессии, rate-limit, быстрые выборки) *[draft]*
* **Message Broker:** RabbitMQ (асинхронные события) *[draft]*
* **Auth:** ASP.NET Identity + JWT (аутентификация и авторизация) *[draft]*
* **CI/CD:** GitHub Actions (build + тесты + миграции)
* **Containerization:** Docker + Docker Compose (backend, frontend, db, broker)

### Ключевые настройки и решения разработки
* Единый порт для API: **5000 (HTTP) **(**HTTPS** — через **proxy-server nginx**)
* Swagger доступен по адресу `http://localhost:5000/swagger`
* Middleware `UseHttpsRedirection()`на **API** выключен по умолчанию (**HTTPS** обеспечивается через **proxy-server nginx**)
* Версия Node.js зафиксирована
* **Pull Request** создаются под задачи (**Task**) из **EPIC Backlog** (таск-трекер **GitHub Projects**)
* **CI** проверяет `build, тесты и миграции`
* **SSOT:**
  * **Build/Test** — только через `dotnet CLI (Release)` 
  * **Run** (PROD) — только через `dotnet CLI (Release)` или `Docker Compose CLI (Release)`
   * **Централизованное управление SDK и пакетами** — см. [ADR-0012 — Centralized Build Config](./adr/0012-centralized-build-config)

## Архитектура сервисов приложения WorkshopCode
Сервисы
- WWW (**React**)
- API (**ASP.NET**)
- DB (**PostgressSQL**)

Поток данных: **WWW → API → DB** 

 Размещение и запуск сервисов
- Dev см. [docs/dev/Dev_Configurations.md](https://github.com/DSivtsov/SeaWind/blob/develop/docs/dev/Dev_Configurations.md) — конфигурации DEV-среды при разработке и тестировании

## Общая структура приложения

### Архитектура сервиса **API**
Общая архитектура — **Clean без Domain (MVC)**

- **API** — тонкие контроллеры, валидация, DTO.
- **Application** — use-case сервисы, контракты, CQS.
- **Infrastructure** — EF Core/PostgreSQL, миграции, репозитории.
- **Domain** отсутствует в MVP.

См. раздел wiki — документы описывают использование архитектуры Clean w/o Domain в проекте WorkshopCode.
**Ссылка:** https://github.com/DSivtsov/SeaWind/wiki/Clean_Wiki_Index

Файлова структура созданная для соответствия выбранной архитектуре **API**

```
SeaWind.sln
├── .github\                       # GitHub workflows и issue templates
│   ├── ISSUE_TEMPLATE\
│   └── workflows\
│
├── docker\                        # Скрипты и конфиги для контейнеров
│   ├── dpkeys\                    # Ключи Data Protection
│   └── init\                      # Инициализация БД, сиды и т.п.
│
├── docs\                          # Документация проекта (ADR, Wiki export)
│   └── adr\
│
├── src\                           # Исходный код приложения
│   ├── backend\                   # Серверная часть (Clean w/o Domain)
│   │   ├── Api\                   # Слой API (MVC-контроллеры, фильтры, DTO)
│   │   │   ├── wwwroot\           # Папка для фронтенд-артефактов (SPA build)
│   │   │   └── Api.csproj
│   │   │
│   │   ├── Application\           # Слой Application (use-case сервисы, контракты)
│   │   │   └── Application.csproj
│   │   │
│   │   ├── Infrastructure\        # Слой Infrastructure (EF Core, репозитории)
│   │   │   └── Infrastructure.csproj
│   │   │
│   │   └── Infrastructure.Postgres\ # Вариант инфраструктуры под PostgreSQL
│   │       └── Infrastructure.Postgres.csproj
│   │
│   └── frontend\                  # Фронтенд (SPA React + Vite)
│       └── web\
│           └── package.json
│
└── tests\                         # Автотесты
    ├── Api.UnitTests\             # Unit-тесты слоя API
    │   └── Api.UnitTests.csproj
    │
    └── Application.UnitTests\     # Unit-тесты слоя Application
        └── Application.UnitTests.csproj

```
### Контейнеризация и база данных

#### Контейнеры для **API** и **DB**
Для запуска части сервисов решения используются **Docker-контейнеры** под управлением **Docker Compose**.
В контейнерах определены сервисы:

* **api** — backend (может запускаться как локально, так и в контейнере);
* **db** — **PostgreSQL**, всегда в контейнере.

**Основное:**
* Взаимодействие между контейнерами идёт по внутренней сети Docker.
* Доступ с хоста — через внешние порты, указанные в `docker/docker-compose.yml`.
* Конфигурация окружения и портов задаётся в `docker/etc` и `docker/docker-compose.yml`.
* Файл `docker/Api.Dockerfile` содержит параметры сборки образа **api**.
* Для **db** используется стандартный образ `postgres:16` (DockerHub).
* **Volume** `pgdata` сохраняет состояние базы данных между перезапусками.
* Настройки `healthcheck` обеспечивают проверку готовности сервисов.

#### Подключение к **API** и **DB**

##### Параметры подключения
* Все внутренние и внешние адреса и порты сервисов задаются в `docker/docker-compose.yml`.
* `ConnectionStrings__Default` для **api → db** по **внутренней сети** указываются в `docker/etc`.
* Для **локального запуска API (VS/CLI dotnet)** `ConnectionStrings__Default` задаются:
  * в `launchSettings.json`;
  * в `appsettings.json` и `appsettings.Development.json`.
* Переменная окружения `ASPNETCORE_ENVIRONMENT` определяет активную конфигурацию.
* Конфигурационные параметры БД (имена, логины, пароли) описаны в `docker/etc`.
* Инициализационные скрипты PostgreSQL и тома монтируются в контейнере **db** при запуске.
* Папка `dpkeys` монтируется в контейнер **api** для хранения ключей.

### Правила добавления новых контроллеров и методов действий
После перехода на технологию MVC
- Один контроллер - это один отдельный файл (отдельный класс).
- Класс контроллера начинается с атрибута [ApiController] и атрибута базовой конечной точки контроллера: [Route("api/[controller]")].
В данном случае подразумевается, что путь к контроллеру будет следующий: http://localhost/api/"название_класса_контроллера".
- Класс контролллер должен наследоваться от базового класса ControllerBase.
- Методы контроллера (действия, actions) также должны начинаться как минимум с двух атрибутов - [Route] и типа запроса [HttpGet], [HttpPost] и т.д. Атрибут [Route] должен в качестве аргумента принимать название конечной точки, например: [Route("GetUser")]. В совокупности с атрибутом [Route] контроллера полный путь к методу действия будет следующим: http://loclahost/api/"название_класса_контроллера"/getuser.
- Метод действия должен возвращать типизированную задачу (Task<T>), т.к. в большинстве случаев они будут асинхронно запрашивать данные из БД.
- Все контроллеры располагаются в нашем проекте SeaWind в Backend/Controllers.

### Централизованная обработка исключений (Exception Handling Middleware)
Интерфейс **IExceptionFilter** в ASP.NET Core используется для обработки исключений, возникающих во время выполнения запроса на уровне MVC (контроллеров и действий).
**Краткий принцип работы:**
Когда в ходе выполнения контроллера или действия выбрасывается исключение, ASP.NET Core вызывает фильтры, реализующие IExceptionFilter или IAsyncExceptionFilter.
Метод OnException(ExceptionContext context) получает объект ExceptionContext, содержащий:
* Само исключение (context.Exception),
* Контекст HTTP-запроса (context.HttpContext),
* Возможность задать результат (context.Result), если вы хотите перехватить и обработать исключение.
Если в фильтре установить context.ExceptionHandled = true, то исключение считается обработанным, и дальше по конвейеру оно уже не пойдёт (глобальный обработчик ошибок не вызовется).

### Data Protection

 **Зачем использует Data Protection keys**

ASP.NET Core автоматически включает систему защиты данных (Data Protection API), даже если её явно не настраивать.  
Эта система используется для шифрования временных данных, которые фреймворк хранит «под капотом».

**Что именно использует Data Protection**

- **Cookies** — авторизационные и аутентификационные куки (например, при использовании Identity, JWT + cookies).  
  Куки шифруются и подписываются ключом.  
  Без прежнего ключа старые куки становятся нечитаемыми → пользователь вылетает из сессии.  

- **Antiforgery tokens** — защита форм от CSRF (например, `@Html.AntiForgeryToken()` в MVC).  

- **TempData и Session** — когда данные временно сохраняются между запросами, ASP.NET тоже использует Data Protection.  

- **Другие библиотеки**, использующие `IDataProtector`:  
  - IdentityServer, OpenIddict  
  - встроенные middleware для внешней авторизации (Google, Microsoft и др.)

📎 Дополнительно см. ADR-0011 «Хранение Data Protection Keys в контейнерах» — опиасние архитектурного решение о способе хранения ключей в Docker-окружениях.

## Описание архитектуры фронтенда

### Общая информация об архитектуре фронтенда

Frontend MVP представляет собой **один React SPA** (без SSR, без UI-kit, без React Query на день 1), покрывающий все MVP-сценарии.  
Основан на **React 18**, **Vite** и **React Router**.

#### Ключевые экраны
- Auth: Вход / Регистрация.
- Courses, Lectures, Journal, Exercises.
- Mentor/Admin: простые таблицы (чтение / базовые действия).
- Profile: базовая информация о пользователе.

#### Стек и ограничения
- **HTTP:** нативный `fetch` (axios не используется).
- **State:** локальное состояние + `AuthContext` (хранение token, user).
- **Формы:** нативная HTML-валидация + минимальные JS-проверки.
- **Стили:** стандартный CSS/модули, без Tailwind.

#### Файловая структура фронтенда проекта
```
frontend/
  src/
    app.jsx            # маршруты
    main.jsx           # React root
    api/
      client.js        # fetch-обёртка
      auth.js          # авторизация
    pages/             # Auth, Courses, Lectures, Journal, Exercises, Profile
    components/        # Navbar, Protected
  vite.config.js
  index.html
```

### Совместная работа фроненда и бекенда
Зависит от типа окружения 
- **DEV:**  
  - **CORS** включён только в Dev, чтобы фронтенд (SPA на `5173`) мог обращаться к API (`5000`).  
  - SPA: `http://localhost:5173`  
  - API: `http://localhost:5000/api/...`  
  - Swagger: `http://localhost:5000/swagger`
  - В директории `wwwroot` сервиса API находится заглушка, которая отдаёт статику и выполняет fallback на `index.html`.
- **PROD:**  
  - SPA и API на одном домене
  - Готовая сборка фронтенда копируется в `src/backend/Api/wwwroot`, где бэкенд отдаёт статику и обеспечивает **SPA fallback**  на `index.html`

#### Middleware и порядок в Program.cs
| Middleware                        | Dev (Vite)      | Prod (wwwroot + SPA)       | Назначение |
| --------------------------------- | --------------- | -------------------------- | ------------------------------------------------ |
| `UseForwardedHeaders()`           | ❌ (опционально) | ✅ (если есть прокси) | Поддержка X-Forwarded-* за реверс-прокси |
| `UseHttpsRedirection()`           | ❌ | ✅ | Перенаправление на HTTPS |
| `UseHsts()`                       | ❌ | ✅ | Защита от downgrade-атак |
| `UseStaticFiles()`                | ❌ | ✅ | Отдаём SPA-сборку из wwwroot |
| `UseRouting()`                    | ✅ | ✅ | Общий пайплайн API |
| `UseCors("Dev")`                 | ✅ | ❌ | CORS только в Dev (для Vite) |
| `UseAuthentication()` / `UseAuthorization()` | ⚠️ при наличии | ⚠️ при наличии | Для JWT/Auth |
| `MapControllers()`                | ✅ | ✅ | API до fallback |
| `MapFallbackToFile("index.html")` | ❌ | ✅ | SPA fallback |
**Почему так**
- В Dev: Vite даёт HMR и быструю итерацию.
- В Prod: бэкенд обслуживает и API, и SPA на одном домене.
- Порядок в Program.cs (сначала Controllers, затем Fallback) гарантирует разделение `/api/*` и роутов SPA.

## Change Log
- v5 (2025-10-13) — форматирование и уточнение формулировок (DS)
- v4 (2025-10-12) — добавлены разделы "Развёртывание с Docker Compose" и "Data Protection", обновлены разделы "Общая Архитектура" и "Общая структура репозитория" в связи с Clean архитектурой (DS)
- v3 (2025-10-04) — добавлен раздел "Описание архитектуры фронтенда" (DS)
- v2 (2025-09-30) — добавлен раздел "Правила добавления новых контроллеров и методов действий" (OK) 
- v1 (2025-09-18) — первоначальная версия (DS)