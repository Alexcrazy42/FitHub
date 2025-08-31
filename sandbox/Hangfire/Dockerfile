FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /
COPY ["HangfireJobs/HangfireJobs.csproj", "."]
RUN dotnet restore "HangfireJobs.csproj"
COPY . .
RUN dotnet publish "HangfireJobs/HangfireJobs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HangfireJobs.dll"]
