# ADR 0022: Проектирование фронтенда (MVP)

## Почему
В рамках MVP потребовалось зафиксировать единые принципы проектирования фронтенда: структуру страниц, их связи, правила роутинга, guards и размещение логики получения данных.
Цель — обеспечить одинаковый подход к реализации всех экранов и избежать архитектурных расхождений.

## Контекст
В рамках реализации данного ADR были разработаны и используются следующие проектные документы и артефакты:

- **Эскизы (wireframes) всех страниц проекта**
  FigJam: [WorkshopCode v3](https://www.figma.com/board/0U1zyeOQB8GfTILsSDvuJt/WorkshopCode-v3?t=1009ocQJzva4jOZA-1)
  Используются для фиксации состава экранов, их содержимого и логических связей.
  ❗Не являются дизайн-макетами.

- **SiteMap проекта**
  Google Sheets: [SiteMap](https://docs.google.com/spreadsheets/d/1NxdBKKyEu1cZIKvhbvV7RslZDrEWH1-_/edit?usp=drive_link&ouid=104265253026337830638&rtpof=true&sd=true)
  Является **приоритетным источником** при любых расхождениях со wireframes.

- **Описание организации Guards (доступ к маршрутам)**
  Документ: [WorkshopCode_Frontend_guard](https://drive.google.com/file/d/1AYamyMW6OS6sPz63awMP1bcMDqkKqBdb/view?usp=drive_link)
  Используется как reference для реализации guards во фронтенде.

## Решение
### Общая структура приложения

Фронтенд строится на `BrowserRouter` с явным разделением:

*   публичных маршрутов,
*   маршрутов с guards,
*   layout-экранов с nested-роутами,
*   полноэкранных рабочих экранов (вне табов).

Auth-сценарии реализуются через **non-routing modals**.

### Общая архитектура страниц

```
App (providers)
→ AppRoutes (routes)
→ Layout (рамка / tabs / Outlet)
→ Page (Container)
→ PageShell
→ Content
```

#### Уровни фронтенд-архитектуры и их назначение

| Уровень | Что это | Назначение | Где |
|--------|--------|------------|------|
| App | Composition root | Инициализация приложения, providers, router, global config | App.tsx |
| Routing | Маршруты | Описание маршрутов, guards, layout-веток | AppRoutes.tsx |
| Layout | Рамка экрана | Header, tabs, navigation, Outlet | CoursesLayout, MentorLayout |
| Page | Логика экрана (Container) | fetch, useEffect, state, retry, data mapping | CoursesListPage, ExercisesPage |
| PageShell | Состояния UI | Отображение loading / empty / error / default | shared/PageShell.tsx |
| Content | Конкретный UI | Таблицы, формы, чаты без логики загрузки | *Content-компоненты* |

#### Принципы размещения данных и логики

**App (providers)**
- Composition root приложения
- Инициализация providers (theme, auth, router, global config)
- Не содержит бизнес-логики экранов и fetch

**AppRoutes (routes)**
- Декларативное описание маршрутов приложения
- Привязка guards и layout-веток
- Не содержит состояния экранов и загрузки данных

**Layout (рамка / tabs / Outlet)**
- Общая рамка и навигация раздела
- Tabs, header, sidebar, Outlet
- Не содержит fetch и бизнес-логики экранов


**Page (Container)**
- Получение данных (`fetch`)
- Управление состояниями экрана
- Retry-логика
- (Опционально) преобразование API → view model

**PageShell**
- Унифицированное отображение UI-состояний
- Не содержит бизнес-логики и fetch

**Content**
- Чистый UI
- Получает готовые данные
- Не знает о состоянии загрузки и источнике данных

### Guards
Guards используются на уровне маршрутов и отвечают за контроль доступа к экранам приложения в зависимости от состояния аутентификации и роли пользователя.
Детальные правила, матрица доступов, поведение при запрете доступа и MVP-упрощения вынесены в отдельный reference-документ и не дублируются в данном ADR.

## Следствия
- Все MVP-экраны реализуются по шаблону: Page → PageShell → Content.
- Fetch и состояние экрана запрещены вне Page.
- PageShell переиспользуется на всех экранах.
- SiteMap имеет приоритет над wireframes при расхождениях.

## Out of Scope
- UI-дизайн и визуальные макеты.
- Не-MVP расширения логики экранов.

