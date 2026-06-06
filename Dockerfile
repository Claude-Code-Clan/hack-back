FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["XakUjin2026.csproj", "."]
RUN dotnet restore "XakUjin2026.csproj"
COPY . .
RUN dotnet build "XakUjin2026.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "XakUjin2026.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Миграции применяются автоматически кодом приложения при старте (db.Database.Migrate()),
# поэтому SDK, dotnet-ef и исходники в финальном образе не нужны.
ENTRYPOINT ["dotnet", "XakUjin2026.dll"]
