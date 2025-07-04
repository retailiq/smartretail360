# ======== Build Stage ========
# Use a lightweight Node.js base image for building
FROM node:24-alpine AS builder

# Set working directory
WORKDIR /app

# Set Arguments for build
ARG NEXT_PUBLIC_BACKEND_SERVER_URL
ARG NEXT_PUBLIC_BACKEND_DATA_URL
ARG NODE_ENV=production

# Set environment variables for the build
ENV NEXT_PUBLIC_BACKEND_SERVER_URL=${NEXT_PUBLIC_BACKEND_SERVER_URL}
ENV NEXT_PUBLIC_BACKEND_DATA_URL=${NEXT_PUBLIC_BACKEND_DATA_URL}
ENV NODE_ENV=${NODE_ENV}

# Copy dependency files first to leverage Docker layer caching
COPY package.json yarn.lock ./

# Install dependencies (includes both dev and prod)
RUN yarn install --frozen-lockfile --prefer-offline

# Copy the rest of the source code
COPY . .

# Build the Next.js app (uses next.config.ts with `output: "standalone"`)
RUN yarn build

# ======== Runtime Stage ========
# Use a fresh lightweight image for running the app
FROM node:24-alpine AS runner

# Set working directory
WORKDIR /app

# Copy the standalone server output (includes server.js and required node_modules)
COPY --from=builder /app/.next/standalone ./

# Copy static assets (public/ and .next/static/)
COPY --from=builder /app/public ./public
COPY --from=builder /app/.next/static ./.next/static

# Expose the desired port
ENV PORT=55004
EXPOSE 55004

# Run the application using the generated server.js
CMD ["node", "server.js"]