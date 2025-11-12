FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN dotnet tool install --global dotnet-ef --version 9.0
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app

COPY src/Api/Api.csproj src/Api/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/

COPY src/Api ./src/Api/
COPY src/Infrastructure ./src/Infrastructure/