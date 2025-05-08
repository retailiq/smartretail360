# Makefile
.PHONY: venv activate clean install-de install-ds

# Create virtual environment if not exists
venv:
	@if [ ! -d ".venv" ]; then \
		python3 -m venv .venv && \
		echo "✅ Virtual environment created. Run: source .venv/bin/activate"; \
	else \
		echo "⚠️  Virtual environment already exists. Skipping creation."; \
	fi

# Just a tip message
activate:
	@echo "Run this in your shell: source .venv/bin/activate"

# Install dependencies
add-deps:
	@echo "📦 Installing data science dependencies..."
	@.venv/bin/pip3 install -r requirements.txt

# Clean up virtual environment
clean:
	rm -rf .venv
	@echo "🧹 Virtual environment deleted."