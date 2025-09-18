# Architecture Overview — WorkshopCode (SeaWind)

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

## 🖼️ Architecture Diagram (ASCII) [DRAFT]
```
 [Frontend: React+Vite]
            │  (REST/SignalR)
            ▼
 [Backend: ASP.NET Core Web API]
   │        │         │
   │        │         │
   ▼        ▼         ▼
PostgreSQL MongoDB   Redis
   │
   ▼
RabbitMQ (Message Broker)
```