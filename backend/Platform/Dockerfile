FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /sources

COPY ["Platform/", "Platform/"]
COPY ["Common/", "Common/"]
COPY ["BankManager/src/Rabbit.Contracts/", "BankManager/src/Rabbit.Contracts/"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["FitHub.sln", "FitHub.sln"]
COPY ["NuGet.config", "NuGet.config"]

RUN dotnet restore "Platform/src/Host/Host.csproj"


RUN dotnet publish "Platform/src/Host/Host.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Устанавливаем FFmpeg
RUN apt-get update && \
    apt-get install -y --no-install-recommends ffmpeg && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV Video__FFmpegBinaryFolder=/usr/bin

ENTRYPOINT ["dotnet", "FitHub.Host.dll"]
