#!/bin/bash
set -e

# 参数校验
if [ -z "$1" ]; then
  echo "Usage: ./scripts/deploy-lambda.sh <function-name> [lambda-dir]"
  echo "Example: ./scripts/deploy-lambda.sh SmartRetailAuthLambda lambdas/auth"
  exit 1
fi

FUNCTION_NAME="$1"
LAMBDA_DIR="${2:-lambdas/auth}"  # 默认为 lambdas/auth

cd "$(dirname "$0")/../$LAMBDA_DIR"

echo "[+] Deploying Lambda from: $(pwd)"
echo "[+] Target Function Name: $FUNCTION_NAME"

echo "[+] Cleaning up old dependencies..."
rm -rf node_modules lambda.zip

echo "[+] Installing production dependencies..."
pnpm install --prod

echo "[+] Creating zip package..."
zip -r lambda.zip index.js package.json node_modules

echo "[+] Uploading to AWS Lambda..."
aws lambda update-function-code \
  --function-name "$FUNCTION_NAME" \
  --zip-file fileb://lambda.zip \
  --region ap-southeast-2

echo "[✓] Lambda '$FUNCTION_NAME' deployed successfully."