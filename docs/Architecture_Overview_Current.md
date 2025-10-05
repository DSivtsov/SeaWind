# Architecture Overview — WorkshopCode (Current)
**Version:** v3
**Date:** 2025-10-04

## Общая Архитектура

### 📐 Основные Решения и Технологии
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

### 📂 Общая структура репозитория
```
SeaWind.sln
src/
  backend/        # ASP.NET Core Web API
  frontend/       # React + Vite
tests/
  backend.tests/  # xUnit
docker-compose.yml
```

## Решения в процесс разработки проекта
Данные решения принимались на митинга проектной команды либо в рамках переписки в чаты проекта.
Источники
- протоколы митингов
- закрпеленные сообщения чаты проекта.
- ADR записи в директории [doc/adr](https://github.com/DSivtsov/SeaWind/tree/develop/docs/adr) репо

### ⚙️ Core Decisions
- Единые порты: **5000 (http)**, **5001 (https)**
- Swagger доступен: https://localhost:5001/swagger
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
<Описание по по централизованной обработке исключений> (OK)

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

В моде **Dev** фронтенд запускается на Vite (`5173`), а API (порт `5000/5001`) отдельно.  
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

#### DEV vs PROD доступ
- **DEV:**  
  - SPA: `http://localhost:5173`  
  - API: `http://localhost:5000/api/...`  
  - Swagger: `http://localhost:5000/swagger`
- **PROD:**  
  - SPA и API на одном домене, fallback на `index.html`.

## Change Log Удалить локальную ветку задачи 
- v3 (2025-10-04) — добавлен раздел "Описание архитектуры фронтенда" 
- v2 (2025-09-30) — добавлен раздел "Правила добавления новых контроллеров и методов действий" (OK) 
- v1 (2025-09-18) — первоначальная версия (DS)