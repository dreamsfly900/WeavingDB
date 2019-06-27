FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/WDBServer.Core/WDBServer.csproj", "src/WDBServer.Core/"]
COPY ["src/WeavingDB.Logical/WeavingDB.Logical.csproj", "src/WeavingDB.Logical/"]
COPY ["src/WeavingDB.DataEncoding.Core/WeavingDB.DataEncoding.csproj", "src/WeavingDB.DataEncoding.Core/"]
RUN dotnet restore "src/WDBServer.Core/WDBServer.csproj"
COPY . .
WORKDIR "/src/src/WDBServer.Core"
RUN dotnet build "WDBServer.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WDBServer.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WDBServer.dll"]