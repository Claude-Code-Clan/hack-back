# XakUjin2026

## Стек
- .NET 10 / ASP.NET Core Web API
- PostgreSQL (EF Core, Npgsql) — миграции применяются автоматически при старте
- JWT, BCrypt, MailKit, Swashbuckle (Swagger)

## Запуск (Docker)
```bash
docker compose up --build
```
Поднимается БД `pgsql` и приложение `dotnetapp`.

- API: http://localhost:8080 (HTTPS: 8081)
- PostgreSQL: localhost:5432

Параметры БД задаются через переменные окружения в `docker-compose.yml`
(`DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`).

## Локальный запуск (без Docker)
Нужен .NET 10 SDK и запущенный PostgreSQL.
```bash
dotnet restore
dotnet run --project XakUjin2026.csproj
```

## CI/CD
GitHub Actions — `.github/workflows/deploy.yml`, триггер: push в `main` или вручную (`workflow_dispatch`).

1. **build** — restore, build (Release) и сборка Docker-образа.
2. **deploy** (только для `main`, после успешного build) — заходит по SSH на сервер и обновляет деплой:
   ```bash
   git pull origin main
   docker compose up -d --build
   docker image prune -f
   ```

Требуются секреты репозитория: `DEPLOY_HOST`, `DEPLOY_USER`, `DEPLOY_SSH_KEY`, `DEPLOY_PORT`, `DEPLOY_PATH`.
