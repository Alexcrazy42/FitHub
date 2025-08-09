##################### Base stage #####################
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080

RUN groupadd --gid 3071 dotnet && \
    useradd --create-home --home-dir /app --uid 3071 --gid dotnet dotnet

USER 3071:3071

##################### Build stage #####################
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG Version=0.0.0.0
ARG AssemblyVersion=0.0.0.0
ARG FileVersion=0.0.0.0
ARG InformationalVersion=0.0.0.0

# Output version info
RUN echo "Application Version: ${Version}" && \
    echo "Assembly Version: ${AssemblyVersion}" && \
    echo "File Version: ${FileVersion}" && \
    echo "Informational Version: ${InformationalVersion}"

WORKDIR /app/src
COPY . .

# Additional build steps injected from code
{{ for step in AdditionalBuildSteps }}
RUN {{ step }}
{{ end }}

RUN dotnet restore
RUN dotnet build \
    --configuration Release \
    --no-restore \
    --nologo \
    -fl1 "/flp1:logFile=buildErrors.log;errorsonly" \
    -p:Version="$Version" \
    -p:AssemblyVersion="$AssemblyVersion" \
    -p:FileVersion="$FileVersion" \
    -p:InformationalVersion="$InformationalVersion" \
    || exit 0

# Making sure that the build errors are logged as errors
RUN sed -i -e 's/^/[build-error]/' buildErrors.log
RUN cat buildErrors.log
RUN test -s buildErrors.log && exit 1 || true

##################### Unit tests stage #####################
FROM build AS test

ARG TestFilter=""

RUN echo "Using test filter: ${TestFilter}"

RUN dotnet test \
    --filter "${TestFilter}" \
    --configuration Release \
    --verbosity normal \
    --no-build \
    --no-restore \
    --nologo

##################### Publish artifacts stage #####################
FROM test AS publish

RUN mkdir -p /app/artifacts

{{ if PublishArtifacts == true }}
RUN dotnet pack \
    --configuration Release \
    --output "/app/artifacts/nuget" \
    --no-build \
    --no-restore \
    --nologo \
    -p:PackageVersion="$Version"
{{ end }}

RUN dotnet publish \
    {{ProjectToPublish}} \
    --configuration Release \
    --output /app/publish \
    --no-build \
    --no-restore \
    --nologo

##################### Final stage #####################
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/artifacts /app/artifacts

USER root

# Additional final steps injected from code
{{ for step in AdditionalFinalSteps }}
RUN {{ step }}
{{ end }}

USER 3071:3071

ENTRYPOINT ["dotnet", "{{AssemblyName}}.dll"]
