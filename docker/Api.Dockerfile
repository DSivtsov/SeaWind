# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Критично: скопировать файлы, задающие TFM и версии, ДО restore
COPY ./global.json ./
COPY ./Directory.Build.props ./
COPY ./Directory.Packages.props ./

# сначала только csproj — лучше кэшируется (все зависимые проекты тоже)
COPY ./src/backend/Api/Api.csproj ./Api/
COPY ./src/backend/Application/Application.csproj ./Application/
COPY ./src/backend/Infrastructure.Postgres/Infrastructure.Postgres.csproj ./Infrastructure.Postgres/
COPY ./src/backend/Infrastructure.Mongo/Infrastructure.Mongo.csproj ./Infrastructure.Mongo/

RUN dotnet restore ./Api/Api.csproj

# теперь весь код
COPY ./src/backend/Api/ ./Api/
COPY ./src/backend/Infrastructure.Postgres/ ./Infrastructure.Postgres/
COPY ./src/backend/Infrastructure.Mongo/ ./Infrastructure.Mongo/
COPY ./src/backend/Application/ ./Application/

# publish (wwwroot внутри Api будет включён автоматически)
RUN dotnet publish ./Api -c Release -o /app/publish

# run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# нужен curl для healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish ./
ARG API_PORT_INT
EXPOSE ${API_PORT_INT}
ENTRYPOINT ["dotnet","SeaWind.Api.dll"]
