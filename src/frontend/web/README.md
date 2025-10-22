# WorkshopCode Frontend — Quick Start & Architecture
**Version:** v4  
**Date:** 2025-10-22  
**Status:** In Review  

Короткий гид для быстрого старта фронтенда WorkshopCode (MVP) и единых правил работы со SPA. Адресат: фронтенд/фулл-стек разработчики команды.

## Содержание

* **Prerequisites** — требования к окружению для запуска.
* **Quick Start** — пошаговый запуск для бэкенда и фронтенда в DEV
* **Архитектура и паттерны (MVP)** — обзор SPA-архитектуры

## Prerequisites
- Настройка и проверка локального окружения перед запуском бэкенда и фронтенда см. [DEV Environment Setup Guide](https://github.com/DSivtsov/SeaWind/wiki/DEV_Environment_Setup)
- Запуск необходимых сервисов бекенда и фронтенда см. [WorkshopCode_ProjectRun](https://github.com/DSivtsov/SeaWind/wiki/WorkshopCode_ProjectRun)

## Архитектура и паттерны (MVP)
> Область: **один React SPA** (JavaScript + Vite), покрывающий MVP-сценарии.  
> Без SSR, без UI-kit, без React Query на день 1.
### 1. Область MVP (экраны)
- **Auth**: Вход / Регистрация (базовые), Выход.
- **Courses**: Список, Детали.
- **Lectures**: Список со ссылками.
- **Journal**: Только чтение для MVP.
- **Exercises**: Список, Детали задания + **только текстовый** чат.
- **Mentor/Admin**: Минимальные таблицы (чтение или базовые действия).
- **Profile**: Базовая информация аккаунта.

### 2. Технологии и ограничения
- **React 18**, **Vite**, **React Router**.
- **HTTP**: нативный `fetch` (без axios).
- **State**: локальное состояние компонентов + небольшой **AuthContext** (token, user).
- **Формы**: нативная HTML-валидация + минимальные проверки в коде.
- **Стили**: CSS (обычный или modules). Tailwind/UI-kit — позже.

### 3. Структура проекта (минимум)
```
frontend/
  src/
    app.tsx            # маршруты
    main.tsx           # React root
    api/
      client.js        # fetch-обёртка
      auth.js          # вход/регистрация
    pages/
      AuthLogin.tsx
      CoursesList.tsx
      CourseDetails.tsx
      Lectures.tsx
      Journal.tsx
      Exercises.tsx
      ExerciseChat.tsx
      MentorWork.tsx
      AdminTopups.tsx
      Profile.tsx
    components/
      Navbar.tsx
      Protected.tsx
  index.html
  vite.config.js
```

### 4. Routing (пример)
```tsx
// app.tsx
import { BrowserRouter, Routes, Route, useNavigate, Navigate } from 'react-router-dom'
import Protected from './components/Protected'
export default function App(){
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<Protected> <Hello /></Protected>}/>
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
```

### 5. API-обёртка (минимум)
```tsx
// api/client.tsx
const base = import.meta.env.VITE_API_BASE ?? "/api";
let token = null;
export const setToken = t => token = t;
export async function api(path, opts={}){
  const res = await fetch(base+path, {
    headers: { "Content-Type":"application/json", ...(token?{Authorization:`Bearer ${token}`}:{}) },
    ...opts
  });
  if(res.status===401) { window.location.href="/login"; return; }
  if(!res.ok) throw new Error(await res.text());
  return res.status===204 ? null : res.json();
}
```

### 6. Auth-поток (просто)
- **Login** → `POST /auth/login` → сохранить `access_token` через `setToken()` и в `localStorage` (для обновления страницы).
- **Guard**: `<Protected>` проверяет token, иначе перенаправляет на `/login`.
- **401-обработка**: глобально в `api()` (перенаправление на `/login`).

`Protected.tsx`: если нет token в памяти, читаем из `localStorage`; если пусто — переход на `/login`.

### 7. Запуск  Prod
**Prod**
```bash
# Сборка SPA
npm ci
npm run build   # вывод в /dist
```
- Копировать `/dist` в backend `wwwroot/`.
- Backend должен иметь `UseStaticFiles()` и SPA fallback для корневых маршрутов SPA.

### 8. Настройка окружения & прокси
`.env`
```
VITE_API_BASE=/api
```
`vite.config.js`
```tsx
const target = process.env.API_URL || 'http://localhost:5000'

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target,
        changeOrigin: true,
        secure: false
      }
    }
  }
})
```

### 9. Получение данных (паттерн)
- Использовать `useEffect` + `api()` для списков/деталей.
- Polling для чата/журнала (например, каждые 10–15 сек). SignalR/WebSockets добавить позже.

---

## Change Log
- v4 (2025-10-22) — замена `JSX -> TSX`
- v3 (2025-10-06) — убран раздел "Quick Start" добавлена ссылка на WorkshopCode_ProjectRun, уточнено форматирование
- v2 (2025-10-04) — добавлен раздел "Фронтенд Architecture & Patterns (MVP)"
- v1 (2025-10-02) — первоначальная версия