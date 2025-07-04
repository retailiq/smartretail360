# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY EmailWorker.sln ./
COPY EmailWorker/ ./EmailWorker/
COPY ../../src/SmartRetail360.Shared/ ./src/SmartRetail360.Shared/
COPY ../../src/SmartRetail360.Logging/ ./src/SmartRetail360.Logging/
COPY ../../src/SmartRetail360.Execution/ ./src/SmartRetail360.Execution/
COPY ../../src/SmartRetail360.Platform/ ./src/SmartRetail360.Platform/
COPY ../../src/SmartRetail360.Notifications/ ./src/SmartRetail360.Notifications/
COPY ../../src/SmartRetail360.Messaging/ ./src/SmartRetail360.Messaging/
COPY ../../src/SmartRetail360.Persistence/ ./src/SmartRetail360.Persistence/
COPY ../../src/SmartRetail360.WorkerBootstrap/ ./src/SmartRetail360.WorkerBootstrap/

RUN dotnet restore
RUN dotnet publish EmailWorker/EmailWorker.csproj -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "EmailWorker.dll"]