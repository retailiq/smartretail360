# Add File
add-client-file name:
    cd apps/client && touch {{name}}

# Dev Cmds
dev-client:
    cd apps/client && pnpm dev

dev-admin-client:
    cd apps/admin-client && pnpm dev

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
	tmux new-session -d -s dev -n client 'cd apps/client && pnpm dev'
	tmux new-window -t dev:1 -n admin 'cd apps/admin-client && pnpm dev'
	tmux new-window -t dev:2 -n landing 'cd apps/landing && pnpm dev'
	tmux select-window -t dev:0
	tmux attach-session -t dev

# Kill Dev Session
kill-dev:
    tmux kill-session -t dev || echo "Session already closed."

# Add Dependencies
# just add-client "-D openapi-typescript"
add-client dep:
    pnpm add {{ dep }} -F client
    
add-admin-client dep:
	pnpm add -D {{ dep }} -F admin-client

add-traceId dep:
    pnpm add {{ dep }} -F traceId    
    
add-nestjs dep:
    pnpm add {{ dep }} -F data-gateway

add-client-dev dep:
    pnpm add -D {{ dep }} -F client

add-nestjs-dev dep:
    pnpm add -D {{ dep }} -F data-gateway
    
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