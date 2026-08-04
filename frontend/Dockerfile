# Multi-stage production image for WealthOS SPA (static assets via nginx).
# Build args inject VITE_* at compile time.
#
# Use Debian (glibc) for the Node build stage — Alpine/musl often misses
# lightningcss / @tailwindcss/oxide optional native binaries when the lockfile
# was generated on Windows.

FROM node:22-bookworm-slim AS build
WORKDIR /app

RUN apt-get update \
  && apt-get install -y --no-install-recommends ca-certificates \
  && rm -rf /var/lib/apt/lists/*

COPY package.json package-lock.json ./
RUN npm install --no-audit --no-fund --include=optional \
  && npm install --no-save --no-audit --no-fund \
       lightningcss-linux-x64-gnu@1.33.0 \
       @tailwindcss/oxide-linux-x64-gnu@4.3.3

COPY . .

ARG VITE_API_MODE=api
ARG VITE_API_BASE_URL=https://api.wealthos.devenlight.com
ARG VITE_API_VERSION=1

ENV VITE_API_MODE=$VITE_API_MODE \
    VITE_API_BASE_URL=$VITE_API_BASE_URL \
    VITE_API_VERSION=$VITE_API_VERSION

RUN npm run build

FROM nginx:1.27-alpine AS final

COPY --from=build /app/.output/public /usr/share/nginx/html
COPY nginx.docker.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://127.0.0.1/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
