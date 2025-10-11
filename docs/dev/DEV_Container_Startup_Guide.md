# DEV Container Startup Guide
**Version:** v1  
**Date:** 2025-10-11  
**Status:** Approved  

## 📖 About this document
**DEV Container Startup Guide** — это пошаговая инструкция по развёртыванию и первому запуску локального окружения WorkshopCode в контейнерах Docker.  

Документ описывает полный процесс от подготовки секретов до запуска backend и применения миграций базы данных.  
Он предназначен для разработчиков, которые хотят быстро поднять рабочую DEV-среду без ручной настройки PostgreSQL или API вне контейнеров.

Основные шаги:
- настройка локальных секретов (`.env`, `appsettings.Development.json`);
- сборка и запуск контейнеров Docker (`api`, `db`);
- применение миграций EF Core;
- тестирование API через Swagger;
- безопасная остановка и выгрузка контейнеров.


## 1. Задание секретов
Перед запуском проекта произведи локальную настройку секретов для окружения **DEV**.  
Необходимо создать и настроить локальные файлы внутри директории решения `<локальная_копия_репозитория_SeaWind>`.  
Настройки в этих файлах должны быть между собой согласованы и не конфликтовать с другими локальными приложениями (порты и т.п.).

```bash
\docker\.env                   # смотри пример —  .env.example
\src\backend\Api\appsettings.Development.json   # смотри пример —  appsettings.Development.json.example
```

> 💡 Примечание: можно просто скопировать и переименовать их.

---

## 2. Docker — сборка контейнера и запуск только БД (отдельная консоль)

Перед запуском убедись, что **Docker Desktop** запущен.

```bash
# Проверка доступа к CLI Docker Compose (должен ставиться вместе Docker Desktop)
cd "корень репо"
docker compose version

# Переход в каталог docker
cd docker

# Сборка контейнеров
docker compose build

# Запуск только базы данных
docker compose up db                # Просмотрите вывод, чтобы не было ошибок.
```

---

## 3. Подготовка к миграции и запуск backend (отдельная консоль)

Следуй стандартным шагам сборки приложения (`3.1 Сборка проекта` из wiki [WorkshopCode_ProjectRun](https://github.com/DSivtsov/SeaWind/wiki/WorkshopCode_ProjectRun)).

```bash
# Установка инструмента миграций
cd "корень репо"
dotnet tool install --global dotnet-ef

dotnet ef -v      # Проверка наличия и доступности

# Переход в каталог Infrastructure
cd src/backend/Infrastructure.Postgres

# Проверка списка миграций
 dotnet ef migrations list -p . -v
```

> В сообщениях должно быть:
> `ConnectionStrings=[Host=localhost;Port=55432 ...]`  
> В конце: `20251008043903_Init_Testers (Pending)`

```bash
# Применение миграций
 dotnet ef database update -p . -v
```

> В сообщениях не должно быть ошибок, и внутри должно быть видны сообщения:
> `CREATE TABLE testers ... Executed DbCommand`  
> В конце: `Done`

---

## 4. Использование БД и запуск backend

После успешного применения миграций можно использовать БД для разработки или тестирования API.

### 🔹 Запуск backend через dotnet
```bash
# для запуска нужна отдельная консоль
dotnet run --project src/backend/Api
```
или
```bash
# через Visual Studio:
# 1. Выполни Clean/Rebuild Solution.  
# 2. Запусти профайл "Ap" — автоматически откроется **Swagger**.
```
> ⚠️ Не устанавливайте поддержку Docker/Orchestrator в VS — могут возникнуть проблемы. Полное отключение возможно только переустановкой VS.

### 🔹 Использование DBeaver (GUI tool for DB)
```bash
choco install dbeaver
```

Подключение:
- Host: `localhost`
- Port: `55432`
- Database: `workshopcode`
- User: `wc_app`
- Password: `wc_app_pwd`

> 💡 При работе чаще делайте **Refresh (F5)** — автообновления нет.

---

## 5. Остановка сервисов backend и db
Для выгрузки контейнеров из памяти (экономия ресурсов локального ПК).

### Вариант 0 
Можно просто закрыть соответствующие консоли

### Вариант 1 — через Docker Desktop
Просто останови запущенные контейнеры.

### Вариант 2 — через консоль
После нажатия **Ctrl + C** в консоли БД выполни:

## Примечание
```bash
docker compose up          # запуск в контейнере db + API
docker compose down     # останов в контейнере db + API (если в запускались не в консоли)

docker compose up db          # запуск в контейнере только db
docker compose down db    # останов в контейнере  только db (если в запускалась не в консоли)
```

> ⚠️ Не используй без необходимости опцию `-v` — она очищает диски БД! при останове контейнеров,
> и потом снова надо будет проводить миграцию