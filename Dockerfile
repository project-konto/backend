# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

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
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80

# Configure the app to listen on container port 80
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "KontoApi.Api.dll"]
