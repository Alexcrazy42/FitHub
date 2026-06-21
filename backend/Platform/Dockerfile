FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /sources

# --- Загрузка и установка FFmpeg ---
# Используем предварительно собранные бинарники FFmpeg для экономии времени
RUN apt-get update && \
    apt-get install -y --no-install-recommends ffmpeg && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* && \
    ffmpeg -version

RUN ffmpeg -version

# 1. Кэшируем зависимости
COPY ["Platform/", "Platform/"]
COPY ["Common/", "Common/"]
COPY ["BankManager/src/Rabbit.Contracts/", "BankManager/src/Rabbit.Contracts/"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["NuGet.config", "NuGet.config"]
COPY ["FitHub.sln", "FitHub.sln"]
RUN dotnet restore "Platform/src/Host/Host.csproj"

# 2. Копируем всё и собираем
RUN dotnet publish "Platform/src/Host/Host.csproj" -c Release -o /app/publish --no-restore


# 3. Финальный легкий образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FitHub.Host.dll"]
