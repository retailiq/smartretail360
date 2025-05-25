# 开发命令
dev-client:
	cd apps/client && pnpm dev

dev-nestjs:
	cd apps/data-gateway && pnpm start:dev

dev-net:
	cd apps/server && dotnet watch run

dev-ai:
	cd apps/ai-services && uvicorn app:main --reload

# 构建命令
build-client:
	cd apps/client && pnpm build

build-api:
	cd apps/server && dotnet publish -c Release -o dist

build-ai:
	cd apps/ai-services && python train.py

email-worker:
    cd apps/workers/EmailWorker && dotnet run

# 清理命令
clean:
	rm -rf node_modules dist .turbo

# 一键启动
dev-all:
	tmux kill-session -t dev || true
	tmux new-session -d -s dev -n email 'cd apps/workers/EmailWorker && dotnet watch run'
	tmux select-window -t dev:0
	tmux attach-session -t dev
#	tmux new-session -d -s dev -n client 'cd apps/client && pnpm dev'
#	tmux new-window -t dev:1 -n nestjs 'cd apps/data-gateway && pnpm start:dev'
#	tmux new-window -t dev:2 -n .net 'cd apps/server/SmartRetail360.API && dotnet watch run'
#	tmux new-window -t dev:3 -n ai 'cd apps/ai-services && uvicorn app:main --reload'


# 一键关闭
kill-dev:
	tmux kill-session -t dev || echo "Session already closed."

# just add-client "-D openapi-typescript"
add-client dep:
    pnpm add {{dep}} -F client

add-nestjs dep:
    pnpm add {{dep}} -F data-gateway