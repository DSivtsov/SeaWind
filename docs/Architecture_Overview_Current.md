# Architecture Overview — WorkshopCode (Current)

## 📐 Project Architecture (MVP)
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

## 📂 Repository Structure
```
SeaWind.sln
src/
  backend/        # ASP.NET Core Web API
  frontend/       # React + Vite
tests/
  backend.tests/  # xUnit
docker-compose.yml
```

## Правила добавления новых контроллеров и методов действий
- Один контроллер - это один отдельный файл (отдельный класс).
- Класс контроллера начинается с атрибута [ApiController] и атрибута базовой конечной точки контроллера: [Route("api/[controller]")].
В данном случае подразумевается, что путь к контроллеру будет следующий: http://localhost/api/"название_класса_контроллера".
- Класс контролллер должен наследоваться от базового класса ControllerBase.
- Методы контроллера (действия, actions) также должны начинаться как минимум с двух атрибутов - [Route] и типа запроса [HttpGet], [HttpPost] и т.д. Атрибут [Route] должен в качестве аргумента принимать название конечной точки, например: [Route("GetUser")]. В совокупности с атрибутом [Route] контроллера полный путь к методу действия будет следующим: http://loclahost/api/"название_класса_контроллера"/getuser.
- Метод действия должен возвращать типизированную задачу (Task<T>), т.к. в большинстве случаев они будут асинхронно запрашивать данные из БД.
- Все контроллеры располагаются в нашем проекте SeaWind в Backend/Controllers.

## ⚙️ Core Decisions
- Единые порты: **5000 (http)**, **5001 (https)**
- Swagger доступен: https://localhost:5001/swagger
- Swagger будет открываться автоматически
- Middleware `UseHttpsRedirection()` включен по умолчанию
- Node.js версия зафиксирована: **v22.19.0 LTS**
- Pull Request создаются только по задачам (Task) из Backlog
- CI окружение проверяет build, тесты и миграции
- SSOT: для сборки, тестов и запуска: **`dotnet CLI`** для build/test и **Docker CLI/Compose** для запуска.
- Фиксация SDK проекта - global.json
