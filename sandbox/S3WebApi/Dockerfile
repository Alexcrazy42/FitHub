FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/FileStorageApi/FileStorageApi.csproj", "."]
RUN dotnet restore "FileStorageApi.csproj"
COPY . .
RUN dotnet publish "src/FileStorageApi/FileStorageApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FileStorageApi.dll"]