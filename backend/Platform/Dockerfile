FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /sources

# ✅ Очищаем все NuGet кэши
RUN dotnet nuget locals all --clear

# ✅ Создаем NuGet.config
RUN echo '<?xml version="1.0" encoding="utf-8"?>' > NuGet.config && \
    echo '<configuration>' >> NuGet.config && \
    echo '  <fallbackPackageFolders>' >> NuGet.config && \
    echo '    <clear />' >> NuGet.config && \
    echo '  </fallbackPackageFolders>' >> NuGet.config && \
    echo '  <packageSources>' >> NuGet.config && \
    echo '    <clear />' >> NuGet.config && \
    echo '    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />' >> NuGet.config && \
    echo '  </packageSources>' >> NuGet.config && \
    echo '  <packageSourceMapping>' >> NuGet.config && \
    echo '    <packageSource key="nuget.org">' >> NuGet.config && \
    echo '      <package pattern="*"/>' >> NuGet.config && \
    echo '    </packageSource>' >> NuGet.config && \
    echo '  </packageSourceMapping>' >> NuGet.config && \
    echo '</configuration>' >> NuGet.config

# 1. Кэшируем зависимости
COPY ["Platform/", "Platform/"]
COPY ["Common/", "Common/"]
COPY ["BankManager/src/Rabbit.Contracts/", "BankManager/src/Rabbit.Contracts/"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["FitHub.sln", "FitHub.sln"]

# ✅ Восстанавливаем
RUN dotnet restore "Platform/src/Host/Host.csproj" \
    --configfile NuGet.config \
    --force \
    --no-cache \
    /p:RestoreDisableParallel=true

# 2. Копируем всё и собираем
COPY . .

# ✅ Очищаем кэш перед publish
RUN dotnet clean "Platform/src/Host/Host.csproj" -c Release && \
    dotnet nuget locals all --clear

# ✅ Публикуем с принудительной пересборкой
RUN dotnet publish "Platform/src/Host/Host.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:ResolvePackageAssets=false \
    /p:ResolveAssemblyReferences=false


# 3. Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# ✅ Устанавливаем FFmpeg в финальном образе
RUN apt-get update && \
    apt-get install -y --no-install-recommends ffmpeg && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* && \
    ffmpeg -version

# Проверяем, что ffprobe тоже установлен
RUN which ffprobe && ffprobe -version || echo "ffprobe not found"

COPY --from=build /app/publish .

# Настраиваем путь к FFmpeg
ENV Video__FFmpegBinaryFolder=/usr/bin

ENTRYPOINT ["dotnet", "FitHub.Host.dll"]
