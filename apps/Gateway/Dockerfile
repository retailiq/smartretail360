# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and restore
COPY Server.sln ./
COPY ../../src/SmartRetail360.ABAC/ ./src/SmartRetail360.ABAC/
COPY ../../src/SmartRetail360.Auth/ ./src/SmartRetail360.Auth/
COPY ../../src/SmartRetail360.Caching/ ./src/SmartRetail360.Caching/
COPY ../../src/SmartRetail360.Caching.Abstractions/ ./src/SmartRetail360.Caching.Abstractions/
COPY ../../src/SmartRetail360.Contracts/ ./src/SmartRetail360.Contracts/
COPY ../../src/SmartRetail360.Domain/ ./src/SmartRetail360.Domain/
COPY ../../src/SmartRetail360.Execution/ ./src/SmartRetail360.Execution/
COPY ../../src/SmartRetail360.Logging/ ./src/SmartRetail360.Logging/
COPY ../../src/SmartRetail360.Logging.Abstractions/ ./src/SmartRetail360.Logging.Abstractions/
COPY ../../src/SmartRetail360.Messaging/ ./src/SmartRetail360.Messaging/
COPY ../../src/SmartRetail360.Notifications/ ./src/SmartRetail360.Notifications/
COPY ../../src/SmartRetail360.Persistence/ ./src/SmartRetail360.Persistence/
COPY ../../src/SmartRetail360.Platform/ ./src/SmartRetail360.Platform/
COPY ../../src/SmartRetail360.Shared/ ./src/SmartRetail360.Shared/
COPY ../../src/SmartRetail360.Shared.Contexts/ ./src/SmartRetail360.Shared.Contexts/

RUN dotnet restore

# Build
RUN dotnet publish SmartRetail360.API/SmartRetail360.API.csproj -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose API port
EXPOSE 65002

ENTRYPOINT ["dotnet", "SmartRetail360.API.dll"]