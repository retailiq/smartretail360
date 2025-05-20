ASG_NAME=eks-smartretail-nodes-68cb5a7c-e564-f8bd-f5ab-501dc24e64ef
REGION=ap-southeast-2

start-node:
	@echo "⏳ Starting EKS node..."
	aws autoscaling update-auto-scaling-group \
	  --auto-scaling-group-name $(ASG_NAME) \
	  --desired-capacity 1 \
	  --region $(REGION)
	@echo "✅ Node start requested."

stop-node:
	@echo "🛑 Stopping EKS node..."
	aws autoscaling update-auto-scaling-group \
	  --auto-scaling-group-name $(ASG_NAME) \
	  --desired-capacity 0 \
	  --region $(REGION)
	@echo "✅ Node stop requested."

node-status:
	@echo "📦 Fetching current node status..."
	aws autoscaling describe-auto-scaling-groups \
	  --auto-scaling-group-names $(ASG_NAME) \
	  --region $(REGION) \
#	  --no-pager \
	  --query "AutoScalingGroups[0].Instances[*].{State:LifecycleState,Id:InstanceId}" \
	  --output table

stop-node-force:
	@echo "🛑 Stopping EKS node (with lifecycle hook check)..."
	aws autoscaling update-auto-scaling-group \
	  --auto-scaling-group-name $(ASG_NAME) \
	  --desired-capacity 0 \
	  --region $(REGION)
	@echo "⏳ Node stop requested. Checking for stuck lifecycle hook..."
	@sleep 3
	@INSTANCE_ID=$$(aws autoscaling describe-auto-scaling-groups \
		--auto-scaling-group-names $(ASG_NAME) \
		--region $(REGION) \
		--query "AutoScalingGroups[0].Instances[?LifecycleState=='Terminating:Wait'].InstanceId" \
		--output text); \
	if [ -n "$$INSTANCE_ID" ]; then \
		echo "⚠️  Found stuck instance: $$INSTANCE_ID. Forcing termination..."; \
		aws autoscaling complete-lifecycle-action \
			--lifecycle-hook-name Terminate-LC-Hook \
			--auto-scaling-group-name $(ASG_NAME) \
			--lifecycle-action-result CONTINUE \
			--instance-id $$INSTANCE_ID \
			--region $(REGION); \
		echo "✅ Lifecycle hook completed for $$INSTANCE_ID."; \
	else \
		echo "✅ No stuck instance found."; \
	fi

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