# ADR 0036 — Mongo Infrastructure (SupportChat + ExerciseChat) and dev setup

## 1. Context

Нужно добавить Mongo как хранилище для чатов и вложений.

Почему Mongo здесь подходит:

- чаты и вложения естественно ложатся в документную модель
- нужны быстрые вставки и чтение сообщений
- данные разделены по двум независимым доменам:
  - SupportChat
  - ExerciseChat

Для текущей архитектуры важно:

1. Mongo работает в Docker.
2. API при старте должен ждать готовности Mongo так же, как уже ждёт Postgres.
3. В одном Mongo-контейнере используются две базы:
   - `supportchat`
   - `exercisechat`
4. Mongo-инфраструктура должна подключаться через отдельный проект `Infrastructure.Mongo`.
5. Для тестового режима нужна возможность не регистрировать реальные Mongo зависимости.

## 2. Decision

1. Добавляем отдельный проект **Infrastructure.Mongo**.
2. Поднимаем Mongo контейнером `mongo:7.0.29` с healthcheck.
3. Используем один `MongoClient` на приложение.
4. Для каждой базы вводим отдельный typed DB сервис:
   - `ISupportChatDb` / `SupportChatDb`
   - `IExerciseChatDb` / `ExerciseChatDb`
5. Mongo подключается через `InfrastructureMongoDI`.
4. Регистрация репозиториев разделяется по доменам:
   - `SupportChatRepositoriesDI`
   - `ExerciseChatRepositoriesDI`
7. Строка подключения и имена баз берутся из конфигурации (`appsettings.Development.json` + env), без хардкода.
8. В test-режиме при `WC_USE_TEST_SETTINGS=true` реальные Mongo зависимости не регистрируются.
9. `docker-compose` настраивается так, чтобы API ждал `mongo` по `service_healthy`.

## 3. Implementation notes

### 3.1. Backend: структура `Infrastructure.Mongo`

Проект `Infrastructure.Mongo` содержит:

Core DI

- `InfrastructureMongoDI.cs` — центральная регистрация Mongo инфраструктуры

Typed database access

- `ExerciseChat/ExerciseChatDb.cs`
- `SupportChat/SupportChatDb.cs`

Repository DI

- `ExerciseChatRepositoriesDI.cs`
- `SupportChatRepositoriesDI.cs`

### 3.2. DI registration

Центральная точка подключения — `InfrastructureMongoDI`.

Она делает следующее:

1. Проверяет `WC_USE_TEST_SETTINGS`.
2. Если test-режим выключен:
   - читает `Mongo:ConnectionString`
   - читает `Mongo:SupportChatDatabase`
   - читает `Mongo:ExerciseChatDatabase`
   - регистрирует singleton `MongoClient`
   - регистрирует `ISupportChatDb`
   - регистрирует `IExerciseChatDb`
3. Затем подключает доменные DI-модули:
   - `AddSupportChatRepositories()`
   - `AddExerciseChatRepositories()`

### 3.3. Typed DB classes

Доступ к базе инкапсулируется через typed DB классы.

Форма доступа:

- `ISupportChatDb` содержит `IMongoDatabase Database`
- `IExerciseChatDb` содержит `IMongoDatabase Database`

То есть репозиторий работает не с сырой регистрацией `IMongoDatabase` из DI, а через доменный контракт.

Пример использования по смыслу:

- `db.Database.GetCollection<MongoChatMessage>("messages")`

### 3.4. Repository registration

Регистрация репозиториев вынесена в отдельные DI-классы:

- `SupportChatRepositoriesDI`
- `ExerciseChatRepositoriesDI`

Сейчас в коде заложено условие для test-режима:

- в обычном режиме должны регистрироваться Mongo-репозитории
- при `WC_USE_TEST_SETTINGS=true` вместо них могут регистрироваться fake-репозитории

Это оставляет возможность изолировать тесты от реальной Mongo базы.

### 3.5. Docker: healthcheck и ожидание API

В `docker/docker-compose.yml`:

- сервис `mongo` имеет `healthcheck`
- сервис `api` имеет `depends_on: mongo: condition: service_healthy`

Это нужно, чтобы API не стартовал раньше Mongo.

## 4. Developer workstation setup (Mongo on dev machine)

Минимальная настройка для запуска Mongo локально через Docker.

### 4.1. Pull образа

```bash
docker pull mongo:7.0.29
```

### 4.2. Настройка конфигурации

Отредактировать:

- `docker/.env` (ориентир: `docker/.env.example`)
- `src/backend/Api/appsettings.Development.json` (ориентир: `appsettings.Development.json.example`)

### 4.3. Запуск Mongo

```bash
docker compose up -d mongo
```

### 4.4. Подключение из API (внутри docker network)

Параметры:

- host: `mongo`
- port: `27017`
- user/pass: `root/example`
- `authSource`: `admin`

Пример:

```text
mongodb://root:example@mongo:27017/?authSource=admin
```

### 4.5. Подключение с хоста

Параметры:

- host: `localhost`
- port: `7017`
- user/pass: `root/example`
- Authentication Database: `admin`

Пример:

```text
mongodb://root:example@localhost:7017/?authSource=admin
```
