# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1) Критично: скопировать файлы, задающие TFM и версии, ДО restore
# (оставь только те, что реально есть в репо)
COPY ./global.json ./ 
COPY ./Directory.Build.props ./
COPY ./Directory.Packages.props ./

# 2) Скопировать только .csproj (все зависимые проекты тоже)
COPY ./src/backend/Api/ ./Api/
COPY ./src/backend/Infrastructure/ ./Infrastructure/
COPY ./src/backend/Application/ ./Application/
RUN dotnet restore ./Api
RUN dotnet publish ./Api -c Release -o /app/publish

# run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet","SeaWind.Api.dll"]

# нужен curl для healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
