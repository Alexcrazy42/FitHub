FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем только .csproj для восстановления зависимостей
COPY ServiceB.csproj ./
RUN dotnet restore "ServiceB.csproj"

# Копируем ВСЕ остальные файлы проекта
COPY . .

# Собираем проект
RUN dotnet build "ServiceB.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Публикуем в папку /app/publish
RUN dotnet publish "ServiceB.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
# Копируем опубликованные файлы
COPY --from=publish /app/publish .
# Запускаем приложение
ENTRYPOINT ["dotnet", "ServiceB.dll"]