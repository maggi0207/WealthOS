# Multi-stage production image for WealthOS (TanStack Start / Nitro node-server).
# Build args inject VITE_* at compile time.
#
# Use Debian (glibc) for the Node build stage — Alpine/musl often misses
# lightningcss / @tailwindcss/oxide optional native binaries when the lockfile
# was generated on Windows.
#
# Do NOT serve .output/public alone via nginx: Cloudflare/Nitro builds put
# HTML shells in the server bundle, so public/ has assets but no index.html
# (Alpine nginx then shows its default "Welcome to nginx!" page).

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
    VITE_API_VERSION=$VITE_API_VERSION \
    NITRO_PRESET=node-server

RUN npm run build \
  && test -f .output/server/index.mjs

FROM node:22-bookworm-slim AS final
WORKDIR /app

ENV NODE_ENV=production \
    HOST=0.0.0.0 \
    PORT=3000 \
    NITRO_HOST=0.0.0.0 \
    NITRO_PORT=3000

RUN apt-get update \
  && apt-get install -y --no-install-recommends curl ca-certificates \
  && rm -rf /var/lib/apt/lists/* \
  && adduser --disabled-password --gecos "" appuser \
  && chown -R appuser /app

COPY --from=build --chown=appuser:appuser /app/.output ./.output

USER appuser

EXPOSE 3000

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=5 \
  CMD curl -fsS http://127.0.0.1:3000/ || exit 1

CMD ["node", ".output/server/index.mjs"]
