# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Yandex Cloud CA certificate
RUN mkdir -p /app/certs
ADD https://storage.yandexcloud.net/cloud-certs/CA.pem /app/certs/root.crt
RUN chmod 644 /app/certs/root.crt

# Copy csproj and restore as distinct layers
COPY src/Api/KontoApi.Api.csproj src/Api/
COPY src/Application/KontoApi.Application.csproj src/Application/
COPY src/Domain/KontoApi.Domain.csproj src/Domain/
COPY src/Infrastructure/KontoApi.Infrastructure.csproj src/Infrastructure/
RUN dotnet restore src/Api/KontoApi.Api.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish src/Api/KontoApi.Api.csproj -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install missing dependencies
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/out .
COPY --from=build /app/certs /app/certs

EXPOSE 80

ENTRYPOINT ["sh", "-c", "dotnet KontoApi.Api.dll --urls http://0.0.0.0:${PORT:-80}"]
