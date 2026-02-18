# ADR 0036 — Mongo Infrastructure (SupportChat + ExerciseChat) and dev setup

### 1\. Context

Нужно добавить Mongo как хранилище для чатов и вложений (по проектной логике: отдельные домены данных, удобный документный формат, быстрые вставки/чтения сообщений). При этом:

1.  Mongo работает в Docker.
2.  API при старте должен **ждать** готовности Mongo так же, как уже ждёт Postgres.
3.  В одном Mongo-контейнере используются **две базы**:
    *   `supportchat`
    *   `exercisechat`

### 2\. Decision

1.  Добавляем отдельный проект **`Infrastructure.Mongo`**.
2.  Поднимаем Mongo контейнером `mongo:7.0.29` с healthcheck.
3.  В API подключаем Mongo через DI (общая точка регистрации + две “ветки” подключения):
    *   `AddSupportChatMongo.cs`
    *   `AddExerciseChatMongo.cs`
4.  Строки подключения берём **из конфигурации** (`appsettings.Development.json` + env), без хардкода.
5.  `docker-compose` настраивается так, чтобы **API ждал** `mongo` по `service_healthy`.

### 3\. Options considered

1.  **Одна Mongo база для всего чата**
    *   проще по настройке
    *   хуже по изоляции и управлению (разные коллекции/индексы/TTL-политики в одной БД)
2.  **Две базы в одном контейнере** ← выбрано
    *   нормальная логическая изоляция
    *   один контейнер, проще dev-операции
3.  **Два контейнера Mongo**
    *   сильнее изоляция
    *   больше конфигурации и поддержки, не нужно сейчас

### 4\. Consequences

1.  На старте API не будет падать из-за “Mongo ещё не поднялся”.
2.  Поддержка двух доменов данных в Mongo становится явной (SupportChat / ExerciseChat).
3.  Для локальной разработки есть простой повторяемый сценарий запуска Mongo.

### 5\. Implementation notes

### 5.1. Backend: состав и точки подключения

*   Новый проект:
    *   `src/backend/Infrastructure.Mongo/Infrastructure.Mongo.csproj`
    *   `src/backend/Infrastructure.Mongo/MongoInfrastructureDI.cs`
*   Регистрация конкретных баз:
    *   `src/backend/Infrastructure.Mongo/AddSupportChatMongo.cs`
    *   `src/backend/Infrastructure.Mongo/AddExerciseChatMongo.cs`
*   Интеграция в API:
    *   `src/backend/Api/Api.csproj` — reference на `Infrastructure.Mongo`
    *   `src/backend/Api/Program.cs` — вызовы DI регистрации Mongo

### 5.2. Docker: healthcheck и ожидание API

*   В `docker/docker-compose.yml`:
    *   сервис `mongo` имеет `healthcheck`
    *   сервис `api` имеет `depends_on: mongo: condition: service_healthy`

### 6\. Developer workstation setup (Mongo on dev machine)

Ниже — минимальная настройка, чтобы разработчик мог запускать Mongo локально через Docker и подключаться к нему как из API, так и с хоста.

### 6.1. Pull образа Mongo

```bash
docker pull mongo:7.0.29
```

### 6.2. Настроить конфиги

Отредактировать:

*   `docker/.env` (ориентир: `docker/.env.example`)
*   `src/backend/Api/appsettings.Development.json` (ориентир: `appsettings.Development.json.example`)

### 6.3. Запуск только Mongo

```bash
docker compose up -d mongo
```

### 6.4. Как подключаться из API (внутри docker network)

Внутри сети Docker (`internal`):

*   host: `mongo`
*   port: `27017`
*   user/pass: `root/example`
*   `authSource`: `admin`

Пример (по смыслу):

```text
mongodb://root:example@mongo:27017/?authSource=admin
```

### 6.5. Как подключаться с хоста (Windows)

С хоста (проброшенный порт):

*   host: `localhost`
*   port: `7017`
*   user/pass: `root/example`
*   Authentication Database: `admin`

Пример:

```text
mongodb://root:example@localhost:7017/?authSource=admin
```

### 7. Architectural notes
- Использование одного Mongo-контейнера с двумя логически изолированными базами (supportchat, exercisechat) является допустимой и целесообразной конфигурацией для текущей архитектуры.
Такое разделение обеспечивает изоляцию доменов данных без усложнения инфраструктуры.
- Параметр container_name допускается к использованию в среде разработки.
Следует учитывать, что его применение ограничивает возможности горизонтального масштабирования и может потребовать пересмотра конфигурации при переходе к production-окружению или оркестрации.
