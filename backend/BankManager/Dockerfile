FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /sources

# 1. Кэшируем зависимости
COPY ["BankManager/", "BankManager/"]
COPY ["Common/", "Common/"]
COPY ["Platform/src/Queue.Contracts/", "Platform/src/Queue.Contracts/"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["NuGet.config", "NuGet.config"]
COPY ["FitHub.sln", "FitHub.sln"]
RUN dotnet restore "BankManager/src/Host/Host.csproj"

# 2. Копируем всё и собираем
RUN dotnet publish "BankManager/src/Host/Host.csproj" -c Release -o /app/publish --no-restore

# 3. Финальный легкий образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FitHub.BankManager.Host.dll"]
