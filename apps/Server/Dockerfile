# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and restore
COPY Server.sln ./
COPY SmartRetail360.API/ ./SmartRetail360.API/
                                               COPY SmartRetail360.Application/ ./SmartRetail360.Application/
                                               COPY SmartRetail360.Infrastructure/ ./SmartRetail360.Infrastructure/
                                               
                                               RUN dotnet restore
                                               
                                               # Build
                                               RUN dotnet publish -c Release -o out
                                               
                                               # Runtime Stage
                                               FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
                                               WORKDIR /app
                                               COPY --from=build /app/out ./
                                               
                                               # Expose API port 
                                               EXPOSE 65002
                                               
                                               # Environment
                                               ENV ASPNETCORE_URLS=http://+:65002
                                               
                                               ENTRYPOINT ["dotnet", "SmartRetail360.API.dll"]