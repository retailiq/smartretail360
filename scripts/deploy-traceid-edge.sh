#!/bin/bash
set -e

# Instructions for deploying a Lambda@Edge function to inject Trace IDs into requests
if [ -z "$1" ]; then
  echo "Usage: ./scripts/deploy-traceid-edge.sh <function-name> [lambda-dir]"
  echo "Example: ./scripts/deploy-traceid-edge.sh InjectTraceIdLambdaEdge lambdas/traceId"
  exit 1
fi

FUNCTION_NAME="$1"
LAMBDA_DIR="${2:-lambdas/traceId}"
AWS_REGION="us-east-1"  # Must be us-east-1 for Lambda@Edge

cd "$(dirname "$0")/../$LAMBDA_DIR"

echo "[+] Deploying Lambda@Edge from: $(pwd)"
echo "[+] Target Function Name: $FUNCTION_NAME"
echo "[+] AWS Region: $AWS_REGION"

echo "[+] Cleaning up..."
rm -rf node_modules lambda.zip

echo "[+] Installing production dependencies..."
pnpm install --prod

echo "[+] Creating zip package..."
zip -r lambda.zip index.js package.json node_modules

echo "[+] Uploading to AWS Lambda in us-east-1..."
aws lambda update-function-code \
  --function-name "$FUNCTION_NAME" \
  --zip-file fileb://lambda.zip \
  --region "$AWS_REGION"

echo "[✓] Lambda '$FUNCTION_NAME' updated in $AWS_REGION."

echo ""
echo "🚀 If this is the first deployment, bind it to CloudFront via console:"
echo "   AWS Console > Lambda > $FUNCTION_NAME > Actions > Deploy to Lambda@Edge"