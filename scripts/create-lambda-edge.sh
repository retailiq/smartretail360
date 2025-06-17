#!/bin/bash
set -euo pipefail

# Usage: ./scripts/create-lambda-edge.sh <lambda-function-name> [lambda-dir]

if [ -z "${1:-}" ]; then
  echo "Usage: $0 <lambda-function-name> [lambda-dir]"
  exit 1
fi

FUNCTION_NAME="$1"
LAMBDA_DIR="${2:-lambdas/traceId}"
REGION="us-east-1"  # Lambda@Edge functions must be created in us-east-1

# Change to the lambda source directory
cd "$(dirname "$0")/../$LAMBDA_DIR"
echo "[+] Working directory: $(pwd)"

# Clean up previous builds
echo "[+] Cleaning up..."
rm -rf node_modules lambda.zip

# Install production dependencies
echo "[+] Installing production dependencies..."
pnpm install --prod

# Zip the Lambda package
echo "[+] Creating zip package..."
zip -r lambda.zip index.js package.json node_modules > /dev/null

# Retrieve the current AWS account ID
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)

# Check if the function already exists
set +e
aws lambda get-function --function-name "$FUNCTION_NAME" --region "$REGION" > /dev/null 2>&1
EXISTS=$?
set -e

if [ "$EXISTS" -ne 0 ]; then
  echo "[+] Function does not exist. Creating..."
  aws lambda create-function \
    --region "$REGION" \
    --function-name "$FUNCTION_NAME" \
    --runtime "nodejs18.x" \
    --role "arn:aws:iam::${ACCOUNT_ID}:role/lambda-edge-role" \
    --handler "index.handler" \
    --zip-file "fileb://lambda.zip" \
    --description "Lambda@Edge function for injecting trace ID" \
    --publish
else
  echo "[+] Function exists. Updating code..."
  aws lambda update-function-code \
    --region "$REGION" \
    --function-name "$FUNCTION_NAME" \
    --zip-file "fileb://lambda.zip"
fi

# Publish a new version
echo "[+] Publishing new version..."
aws lambda publish-version \
  --function-name "$FUNCTION_NAME" \
  --region "$REGION"

echo "[✓] Lambda@Edge function '$FUNCTION_NAME' created and published."