namespace Infrastructure.Postgres.Seeding.Shared;

public enum SeedOrder
{
    Base,       // независимые справочники
    Core,       // основные сущности
    Links,      // связи
    Post,       // дополнительные данные
    Extra,      // резерв под будущие зависимости (например, кэш, вторичные связи) **RESERVED**
    Final       // завершающая стадия (логирование, миграции, audit seed)  **RESERVED**
}