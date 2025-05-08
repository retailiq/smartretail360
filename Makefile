install:
	pnpm install

build:
	just build-client
	just build-api
	just build-ai

test:
	cd tests && pytest

deploy:
	echo "TODO: implement deploy pipeline (GitHub Actions or Jenkins)"