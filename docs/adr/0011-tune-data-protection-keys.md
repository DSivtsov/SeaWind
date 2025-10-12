# ADR 0011: Хранение Data Protection Keys в контейнерах

**Дата:** 2025-10-10

## Решение
Ключи ASP.NET Data Protection сохраняем в постоянное хранилище (том), а не во временном каталоге контейнера.
* Внутри контейнера ASP.NET настроено хранилище `/root/.aspnet/DataProtection-Keys`.
* В `Program.cs` добавлен метод `AddStorageForContainers()` (см. коммит [Feature #17: Настройка docker compose для сервисов backend и db](https://github.com/DSivtsov/SeaWind/pull/59)), который подключает это хранилище к приложению.

## Почему
Если ничего не настраивать:

* На Windows-машине ключи пишутся в `%LOCALAPPDATA%\ASP.NET\DataProtection-Keys`.
* В **контейнере Linux** (как у нас) — в `/root/.aspnet/DataProtection-Keys`.

То есть система сама выбирает дефолтную директорию и кладёт туда XML-файлы с ключами.
В контейнере этот каталог временный — при `docker compose down` или пересборке образа он стирается, и старые куки уже нельзя расшифровать → пользователи разлогиниваются.
При использовании приложения ASP.NET внутри контейнера появляется предупреждение
> "Storing keys in '/root/.aspnet/DataProtection-Keys' that may not be persisted outside of the container"

## Следствие
Ключи сохраняются между перезапусками, и пользователи остаются залогинены даже после `docker compose up --build`.

## Примечания
Рекомендации на будущее: для окружений **TST/PRD** использовать **общий key-ring** (например, NFS или Redis), чтобы все инстансы приложения могли расшифровывать одни и те же ключи.
Подробнее см. *Architecture_Overview_Current.md* → раздел **Data Protection**.

---
