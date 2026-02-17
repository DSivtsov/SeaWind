## ADR 0035 — EnsureSuperAdmin через Seeding + Configuration

### Context

1. В DEV нужен гарантированный доступ с ролью `Admin`.
2. Пароль не хранится в репозитории.
3. Seeding AppUsers выполняется через `Seeding/MainRunner.cs` с очисткой таблиц перед загрузкой demo‑данных.

---

### Decision

Добавлен метод `EnsureSuperAdmin` в `Seeding/MainRunner.cs`.

Метод вызывается в конце общего процесса MainRunner.cs (после сидирования пользователей).

Источники конфигурации (`IConfiguration`):

- `SuperAdmin:Email` — из `appsettings.json`
- `SuperAdmin:UserName` — из `appsettings.json`
- `SuperAdmin:Password` — из user‑secrets проекта `Api`

Пароль в `appsettings` не хранится.

---

### Поведение метода EnsureSuperAdmin

1. Найти пользователя по Email.
2. Если пользователь отсутствует — создать его через `UserManager`, используя пароль из `SuperAdmin:Password`.
3. После создания (или если пользователь уже существует) убедиться, что он добавлен в роль `Admin`.

Операция выполняется идемпотентно: повторный запуск не создаёт дубликатов и лишь приводит состояние к ожидаемому.

---

### Настройка developer‑окружения

Для корректной работы механизма разработчик должен добавить секрет `SuperAdmin:Password` в проект `Api`. Без этого метод не сможет создать пользователя, так как пароль не хранится в конфигурационных файлах.

```powershell
cd H:\_ASP.NET\SeaWind\src\backend\Api

# Инициализация user-secrets (один раз на проект)
dotnet user-secrets init

# Установка пароля SuperAdmin (любой, соответствующий требованиям Identity)
dotnet user-secrets set "SuperAdmin:Password" "1234qQ"

```
После выполнения команды секрет будет сохранён локально для проекта `Api` и станет доступен через `IConfiguration` в Development окружении.

Проверка, что секрет действительно сохранён:
```powershell
dotnet user-secrets list
````
Ожидаемый вывод:

```
SuperAdmin:Password = 1234qQ
```

