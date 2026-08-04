# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/WealthOS.Api/WealthOS.Api.csproj", "src/WealthOS.Api/"]
COPY ["src/WealthOS.Application/WealthOS.Application.csproj", "src/WealthOS.Application/"]
COPY ["src/WealthOS.Domain/WealthOS.Domain.csproj", "src/WealthOS.Domain/"]
COPY ["src/WealthOS.Infrastructure/WealthOS.Infrastructure.csproj", "src/WealthOS.Infrastructure/"]
COPY ["Directory.Build.props", "./"]
COPY ["global.json", "./"]

RUN dotnet restore "src/WealthOS.Api/WealthOS.Api.csproj"

COPY . .
WORKDIR /src/src/WealthOS.Api
RUN dotnet publish "WealthOS.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

# curl is required for Docker HEALTHCHECK (aspnet image has neither wget nor curl by default)
USER root
RUN apt-get update \
  && apt-get install -y --no-install-recommends curl \
  && rm -rf /var/lib/apt/lists/* \
  && adduser --disabled-password --gecos "" appuser \
  && chown -R appuser /app

USER appuser

COPY --from=build --chown=appuser:appuser /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=60s --retries=5 \
  CMD curl -fsS http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "WealthOS.Api.dll"]
