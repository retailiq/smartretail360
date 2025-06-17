# Dev Cmds
dev-client:
    cd apps/client && pnpm dev

dev-nestjs:
    cd apps/data-gateway && pnpm start:dev

dev-net:
    cd apps/server && dotnet watch run

dev-ai:
    cd apps/ai-services && uvicorn app:main --reload

# Build Cmds
build-client:
    cd apps/client && pnpm build

build-api:
    cd apps/server && dotnet publish -c Release -o dist

build-ai:
    cd apps/ai-services && python train.py

email-worker:
    cd apps/workers/EmailWorker && dotnet run

# Build All
clean:
    rm -rf node_modules dist .turbo

# Start Server
dev-all:
    tmux kill-session -t dev || true
    tmux new-session -d -s dev -n email 'cd apps/workers/EmailWorker && dotnet watch run'
    tmux select-window -t dev:0
    tmux attach-session -t dev

#	tmux new-session -d -s dev -n client 'cd apps/client && pnpm dev'
#	tmux new-window -t dev:1 -n nestjs 'cd apps/data-gateway && pnpm start:dev'
#	tmux new-window -t dev:2 -n .net 'cd apps/server/SmartRetail360.API && dotnet watch run'
#	tmux new-window -t dev:3 -n ai 'cd apps/ai-services && uvicorn app:main --reload'

# Kill Dev Session
kill-dev:
    tmux kill-session -t dev || echo "Session already closed."

# Add Dependencies

# just add-client "-D openapi-typescript"
add-client dep:
    pnpm add {{ dep }} -F client

add-traceId dep:
    pnpm add {{ dep }} -F traceId    
    
add-nestjs dep:
    pnpm add {{ dep }} -F data-gateway

add-client-dev dep:
    pnpm add -D {{ dep }} -F client

add-nestjs-dev dep:
    pnpm add -D {{ dep }} -F data-gateway
    
add-dotnet-logging pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./src/SmartRetail360.Logging/SmartRetail360.Logging.csproj package {{pkg}}; \
    else \
        dotnet add ./src/SmartRetail360.Logging/SmartRetail360.Logging.csproj package {{pkg}} --version {{ver}}; \
    fi

add-dotnet-shared pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./src/SmartRetail360.Shared/SmartRetail360.Shared.csproj package {{pkg}}; \
    else \
        dotnet add ./src/SmartRetail360.Shared/SmartRetail360.Shared.csproj package {{pkg}} --version {{ver}}; \
    fi

add-dotnet-abac pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./src/SmartRetail360.ABAC/SmartRetail360.ABAC.csproj package {{pkg}}; \
    else \
        dotnet add ./src/SmartRetail360.ABAC/SmartRetail360.ABAC.csproj package {{pkg}} --version {{ver}}; \
    fi
    
add-dotnet-messaging pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./src/SmartRetail360.Messaging/SmartRetail360.Messaging.csproj package {{pkg}}; \
    else \
        dotnet add ./src/SmartRetail360.Messaging/SmartRetail360.Messaging.csproj package {{pkg}} --version {{ver}}; \
    fi    
    
add-dotnet-persistence pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./src/SmartRetail360.Persistence/SmartRetail360.Persistence.csproj package {{pkg}}; \
    else \
        dotnet add ./src/SmartRetail360.Persistence/SmartRetail360.Persistence.csproj package {{pkg}} --version {{ver}}; \
    fi    

add-dotnet-app pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./apps/Server/SmartRetail360.Application/SmartRetail360.Application.csproj package {{pkg}}; \
    else \
        dotnet add ./apps/Server/SmartRetail360.Application/SmartRetail360.Application.csproj package {{pkg}} --version {{ver}}; \
    fi
    
add-dotnet-server-api pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./apps/Gateway/SmartRetail360.Gateway.API/SmartRetail360.Gateway.API.csproj package {{pkg}}; \
    else \
        dotnet add ./apps/Server/SmartRetail360.Gateway.API/SmartRetail360.Gateway.API.csproj package {{pkg}} --version {{ver}}; \
    fi    

add-dotnet-server-infra pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./apps/Server/SmartRetail360.Infrastructure/SmartRetail360.Infrastructure.csproj package {{pkg}}; \
    else \
        dotnet add ./apps/Server/SmartRetail360.Infrastructure/SmartRetail360.Infrastructure.csproj package {{pkg}} --version {{ver}}; \
    fi

add-dotnet-worker-bootstrap pkg ver="":
    if [ "{{ver}}" = "" ]; then \
        dotnet add ./apps/workers/SmartRetail360.WorkerBootstrap/SmartRetail360.WorkerBootstrap.csproj package {{pkg}}; \
    else \
        dotnet add ./apps/workers/SmartRetail360.WorkerBootstrap/SmartRetail360.WorkerBootstrap.csproj package {{pkg}} --version {{ver}}; \
    fi   

# Lambda deploy
deploy-auth:
  bash scripts/deploy-lambda.sh SmartRetailAuthLambda lambdas/auth
  
deploy-traceId:
  bash scripts/deploy-traceid-edge.sh InjectTraceIdLambdaEdge1 lambdas/traceId
  
# Create Lambda Layer 
create-lambda-edge name path="lambdas/traceId":
    bash scripts/create-lambda-edge.sh {{name}} {{path}}
    
# Migrations
migrate-update:
  dotnet ef database update \
    --project ./src/SmartRetail360.Persistence/SmartRetail360.Persistence.csproj \
    --startup-project ./apps/Server/SmartRetail360.API/SmartRetail360.API.csproj \
    --context AppDbContext
    
migrate-add name:
  dotnet ef migrations add {{name}} \
    --project ./src/SmartRetail360.Persistence/SmartRetail360.Persistence.csproj \
    --startup-project ./apps/Server/SmartRetail360.API/SmartRetail360.API.csproj \
    --output-dir Data/Migrations \
    --context AppDbContext

migrate-clean:
  rm -rf ./src/SmartRetail360.Persistence/Data/Migrations/*    