# WorkshopCode — MVP

Минимальный запуск локально: backend + frontend.

## Prerequisites
см. [DEV Environment Setup Guide](https://github.com/DSivtsov/SeaWind/wiki/DEV_Environment_Setup)
## Quick Start

### 1) Backend (API)
```bash
# перейти в корень локальной копии репозитория SeaWind
cd <локальная_копия_репозитория_SeaWind>

# создать чистое окружение Release
dotnet clean -c Release

# восстановить пакеты
dotnet restore

# собрать решение (Release)
dotnet build -c Release --no-restore -warnaserror
````

### 2) Frontend (SPA)

```bash
cd <локальная_копия_репозитория_SeaWind>\src\frontend

# Нужно выполнить только при первом запуске или если измениться package.json
npm install

# Запускать всегда
npm run dev
```

Открой: [http://localhost:5173](http://localhost:5173) → ожидаемый редирект на `/login`.
После успешного входа доступны защищённые страницы.

## Что включено в MVP

* Роли и базовая защита роутов (Guest → /login).
* Редирект обратно на исходный маршрут после логина.
* Logout очищает хранилища и делает history replace.

## Полезно знать

* Порты по умолчанию: 
    - SPA (Vite) → http://localhost:5173
    - API (ASP.NET) → http://localhost:5000 (по умолчанию, см. launchSettings.json)
* Если в консоли видите 401 Unauthorized до логина — это ожидаемое поведение.

