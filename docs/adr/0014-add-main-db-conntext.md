# ADR 0014: Схема БД, конфигурация сущностей и миграции для MainDbContext

**Дата:** 2025-10-22  

## Почему
Было принято решение унифицировать хранение и описание таблиц основной БД для обеспечения чёткой структуры и разделения областей данных при наличии нескольких контекстов (например, `MainDbContext` и `IdentityDbContext`).

Использование единой схемы (`main`) упрощает сопровождение и миграции, предотвращает конфликты между контекстами и делает архитектуру прозрачной. Применение Code First с `DbSet` и `Fluent API` позволяет точно контролировать структуру таблиц, их имена, индексы и ограничения. Разделённые миграции и отдельная таблица `__EFMigrationsHistory` внутри схемы `main` обеспечивают независимость и управляемость версий модели данных.

## Решение
1. Все таблицы `MainDbContext` размещаются в схеме **`main`** через `builder.HasDefaultSchema("main")`.
2. Для каждой сущности основной БД:
   * добавить свойство `DbSet<TEntity>` в `MainDbContext`;
   * описать структуру таблицы через `OnModelCreating` (Fluent API).
3. Миграции `MainDbContext` хранятся в **Infrastructure.Postgres\\Main\\Migrations\\**, история миграций — в `main.__EFMigrationsHistory`.
4. Репозитории основного контекста размещаются в **Infrastructure.Postgres\\Main\\Repositories\\**, и регистрируются в сервисах с жизненным циклом `Scoped`.

## Использование решения

### 1. Схема по умолчанию
```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.HasDefaultSchema(MainDbContext.Schema); // "main"
    // ... остальная конфигурация сущностей
}
```
> Все таблицы `MainDbContext` по умолчанию создаются в схеме `main` (например, `main.Courses`).

### 2. Где описывать таблицы
**DbSet:** объявляем для каждой сущности, которая принадлежит основной БД:
```csharp
public DbSet<Course> Courses => Set<Course>();
```
**OnModelCreating:** описываем структуру и ограничения:
```csharp
builder.Entity<Course>(entity =>
{
    entity.ToTable("Courses");
    entity.HasKey(c => c.Id);
    entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
    entity.HasIndex(c => c.Code).IsUnique();
});
```

### 3. Репозитории
- Расположение: **Infrastructure.Postgres\\Main\\Repositories\\**
- Время жизни: `Scoped`
```csharp
services.AddScoped<ICourseRepository, CourseRepositoryPostgres>();
```
> Все репозитории `MainDbContext` регистрируются аналогично.

### 4. Миграции `MainDbContext`
- Папка: **Infrastructure.Postgres\\Main\\Migrations\\**
- Создание и применение:
```bash
cd src/backend/Infrastructure.Postgres

# Создание миграции
 dotnet ef migrations add InitMain ^
   -c MainDbContext ^
   -o Infrastructure/Postgres/Main/Migrations

# Применение миграций
 dotnet ef database update -c MainDbContext
```
> Ключ `-c` (context) и `-o` (output) гарантируют, что миграции `MainDbContext` создаются в правильной папке и не смешиваются с другими контекстами.

