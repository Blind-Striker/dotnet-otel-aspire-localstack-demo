FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["local/OpenTelemetry.Demo.Local.Database/OpenTelemetry.Demo.Local.Database.csproj", "local/OpenTelemetry.Demo.Local.Database/"]
RUN dotnet restore "local/OpenTelemetry.Demo.Local.Database/OpenTelemetry.Demo.Local.Database.csproj"
COPY . .
WORKDIR "/src/local/OpenTelemetry.Demo.Local.Database"
RUN dotnet build "OpenTelemetry.Demo.Local.Database.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OpenTelemetry.Demo.Local.Database.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetry.Demo.Local.Database.dll"]
