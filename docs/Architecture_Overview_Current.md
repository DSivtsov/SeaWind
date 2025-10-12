# Architecture Overview — WorkshopCode (Current)
**Version:** v4
**Date:** 2025-10-12

## 🧾 About
 Описывает решения принятые в процесс разработки проекта.
Данные решения принимались на митинга проектной команды либо в рамках переписки в чаты проекта.
Источники
- протоколы митингов
- закрпеленные сообщения чаты проекта.
- ADR записи в директории [doc/adr](https://github.com/DSivtsov/SeaWind/tree/develop/docs/adr) репо

## 📐 Основные Решения и Технологии
- **Backend:** ASP.NET Core Web API (.NET 8)
- **Frontend:** React + Vite (Node.js LTS v22.19.0)
- **Database:**
  - PostgreSQL — основная реляционная база 
  - MongoDB — NoSQL для чатов и вложений
- **Cache:** Redis (для сессий, rate-limit, быстрых выборок) [DRAFT]
- **Message Broker:** RabbitMQ (асинхронные события) [DRAFT]
- **Auth:** ASP.NET Identity + JWT (аутентификация и авторизация) [DRAFT]
- **CI/CD:** GitHub Actions (build + тесты + миграции)
- **Containerization:** Docker + docker-compose (backend, frontend, db, broker)

## Общая Архитектура
Общая архитектура — **Clean без Domain (MVC)**

- **API** — тонкие контроллеры, валидация, DTO.
- **Application** — use-case сервисы, контракты, CQS.
- **Infrastructure** — EF Core/PostgreSQL, миграции, репозитории.
- **Domain** отсутствует в MVP.

См. раздел wiki — документы описывают использование архитектуры Clean w/o Domain в проекте WorkshopCode.
**Ссылка:** https://github.com/DSivtsov/SeaWind/wiki/Clean_Wiki_Index

### 📂 Общая структура репозитория
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
### Развёртывание с Docker Compose
В контейнере описаны сервисы **api** (backend) и **db** (PostgreSQL).
- **backend** может запускаться как в контейнере, так и локально на хосте.  
- **PostgreSQL** — всегда в контейнере.  
- Общение между сервисами в контейнере идёт по внутренним адресам и портам сервисов внутри сети Docker.  
- Доступ к API или к БД c локальной машине идёт по внешнем адресам и портам сервисов.
- В файле окружения контейнера `docker/etc` и конфигурационном файле `docker/docker-compose.yml` прописаны:
  - Все внутренние и внешние порты и адреса сервисов
  - Переменная окружения ASPNETCORE_ENVIRONMENT
  - Все конфигурационный параметры БД, как системной, так и рабочей (включая логины и  пароли пользователей)
  - ConnectionStrings__Default для подключения сервиса **api** к **db** по внутренней сети
  - Место хранения и монтажа инициализационных скриптов PostgreSQL и томов сервиса **db**
  - Место хранения и монтажа  dpkeys сервиса **api** 
  - Настройки сервисов `healthcheck` для контейнеров
 - В папке  `docker/Api.Dockerfile` хранятся инициализационные скриптов PostgreSQL
- Файл `docker/Api.Dockerfile` хранит параметры для сборки образа сервиса **api** (backend)
- Для образа сервиса **db** (PostgreSQL) используется стандартный образ postgres:16 `DockerHub`
- ConnectionStrings__Default для подключения сервиса **api** (VS/CLI dotnet) к **db** по внешней сети определяются
   - файлом `launchSettings.json`
   - файлами `appsettings.json` и `appsettings.Development.json`
- Volume `pgdata` сохраняет состояние базы данных между перезапусками.

### ⚙️ Core Decisions
- Единые порты: **5000 (http)**
- Swagger доступен: http://localhost:5000/swagger
- Swagger будет открываться автоматически
- Middleware `UseHttpsRedirection()` включен по умолчанию
- Node.js версия зафиксирована: **v22.19.0 LTS**
- Pull Request создаются только по задачам (Task) из Backlog
- CI окружение проверяет build, тесты и миграции
- SSOT: для сборки, тестов и запуска: **`dotnet CLI`** для build/test и **Docker CLI/Compose** для запуска.
- Фиксация SDK проекта - global.json

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

#### Структура проекта
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

### Интеграция с backend (Program.cs + SPA fallback)

В моде **Dev** фронтенд запускается на Vite (`5173`), а API (порт `5000`) отдельно.  
В **Prod** собранный SPA (`frontend/dist`) копируется в `backend/wwwroot/`, где бэкенд отдаёт статику и обеспечивает **SPA fallback** на `index.html`.

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

#### Почему так
- В Dev: Vite даёт HMR и быструю итерацию.
- В Prod: бэкенд обслуживает и API, и SPA на одном домене.
- Порядок в Program.cs (сначала Controllers, затем Fallback) гарантирует разделение `/api/*` и роутов SPA.

#### DEV vs PROD совместная работа фроненда и бекенда
- **DEV:**  
  - **CORS** включён только в Dev, чтобы фронтенд (SPA на `5173`) мог обращаться к API (`5000`).  
  - SPA: `http://localhost:5173`  
  - API: `http://localhost:5000/api/...`  
  - Swagger: `http://localhost:5000/swagger`
  - В директории `wwwroot` сервиса API находится заглушка, которая отдаёт статику и выполняет fallback на `index.html`.
- **PROD:**  
  - SPA и API на одном домене
  - Готовая сборка фронтенда копируется в `wwwroot`, где API отдаёт статику и выполняет fallback на `index.html`

## Change Log Удалить локальную ветку задачи 
- v4 (2025-10-12) — добавлены разделы "Развёртывание с Docker Compose" и "Data Protection", обновлены разделы "Общая Архитектура" и "Общая структура репозитория" в связи с Clean архитектурой
- v3 (2025-10-04) — добавлен раздел "Описание архитектуры фронтенда" 
- v2 (2025-09-30) — добавлен раздел "Правила добавления новых контроллеров и методов действий" (OK) 
- v1 (2025-09-18) — первоначальная версия (DS)
