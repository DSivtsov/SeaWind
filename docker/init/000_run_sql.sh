#!/usr/bin/env bash
set -euo pipefail

# Тесты для переменных
#APP_DB_EFF="${APP_DB:-${POSTGRES_DB}}"
#APP_USER_EFF="${APP_USER:-wc_app}"
#APP_PASSWORD_EFF="${APP_PASSWORD:-wc_app_pwd}"


echo "===DEMO SHOW VAR"
echo "APP_USER=${APP_USER}"
echo "APP_PASSWORD=${APP_PASSWORD}"
echo "APP_DB=${APP_DB}"
echo "POSTGRES_USER=${POSTGRES_USER}"
echo "POSTGRES_PASSWORD=${POSTGRES_PASSWORD}"
echo "POSTGRES_DB=${POSTGRES_DB}"

###############################################################################
# 1) CREATE ROLE, если её ещё нет (WHERE NOT EXISTS + \gexec)
###############################################################################
echo "CREATED ROLE?"

psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" \
  -v APP_USER="$APP_USER" \
  -v APP_PASSWORD="$APP_PASSWORD" <<'SQL'
-- Сформируем CREATE ROLE только если роли нет
SELECT format('CREATE ROLE %I LOGIN PASSWORD %L', :'APP_USER', :'APP_PASSWORD')
WHERE NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = :'APP_USER');
\gexec
SQL

###############################################################################
# 2) ALTER DATABASE owner → APP_USER
###############################################################################
echo "ALTERED DATABASE?"

psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -v APP_DB="$APP_DB" -v APP_USER="$APP_USER" <<'SQL'
ALTER DATABASE :"APP_DB" OWNER TO :"APP_USER";
SQL

###############################################################################
# 3) Права на схему public и дефолтные привилегии
###############################################################################
echo "GRANTED CONNECT?"

psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -v APP_DB="$APP_DB" -v APP_USER="$APP_USER" <<'SQL'
-- 1. Разрешаем пользователю подключаться к БД (делаем это явно)
GRANT CONNECT ON DATABASE :"APP_DB" TO :"APP_USER";
SQL

echo "GRANTED USAGE ON SCHEMA AND ON OPERATIONS?"

psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -v APP_DB="$APP_DB" -v APP_USER="$APP_USER" <<'SQL'
-- 3. Основные DML-права
GRANT USAGE ON SCHEMA public TO :"APP_USER";
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO :"APP_USER";
SQL

echo "ALTERED DEFAULT PRIVILEGES IN SCHEMA?"

psql -U "$POSTGRES_USER" -d "$POSTGRES_DB" -v APP_DB="$APP_DB" -v APP_USER="$APP_USER" <<'SQL'
-- 4. Дефолтные привилегии для будущих таблиц
ALTER DEFAULT PRIVILEGES IN SCHEMA public
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO :"APP_USER";
SQL
