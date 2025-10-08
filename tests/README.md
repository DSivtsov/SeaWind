# 🧪 Testing Approach — SeaWind

**Purpose:** единый подход к созданию и организации тестов в проекте.

---

## 📂 Структура каталогов

```
tests/
├─ Application.UnitTests/      # тесты бизнес-логики
├─ Infrastructure.UnitTests/   # тесты репозиториев, адаптеров
├─ Api.UnitTests/              # контроллеры без веб-хоста/БД
├─ Api.IntegrationTests/       # WebApplicationFactory + Testcontainers
├─ Api.E2E.Tests/              # сквозные пользовательские сценарии (опц.)
└─ TestUtilities/              # общие фикстуры, билдеры, хелперы
```

---

## 🔗 Ссылки между проектами

* `*.UnitTests` → ссылаются **только** на свой слой (`Api.UnitTests` → `Api`).
* `Api.IntegrationTests` → `Api` (+ `TestUtilities`).
* `TestUtilities` → без ссылок на прод-код, максимум — на общие абстракции.

---

## ⚙️ Минимальный набор пакетов

| Тип тестов  | Основные пакеты                                          |
| ----------- | -------------------------------------------------------- |
| Unit        | `xunit`, `xunit.runner.visualstudio`, `FluentAssertions` |
| Integration | + `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.*` |
| E2E         | REST (`HttpClient`) / UI (`Playwright`) по стеку         |

---

## 🧩 Нейминг и запуск

* Формат имени проекта: `<Layer>.<TypeOfTests>.csproj`.
* Запуск из CLI:

  ```bash
  dotnet test tests/Api.UnitTests
  dotnet test tests/Api.IntegrationTests
  ```

---

## 💡 Why is that?

Разделение по слоям и типам тестов:

* ускоряет локальный цикл (`unit → integration → e2e`);
* уменьшает зависимость тестов между собой;
* упрощает параллельный прогон и конфигурацию CI.
