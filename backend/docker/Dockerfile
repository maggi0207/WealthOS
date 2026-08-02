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

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WealthOS.Api.dll"]
