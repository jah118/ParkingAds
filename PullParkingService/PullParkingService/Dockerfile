﻿FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PullParkingService/PullParkingService.csproj", "PullParkingService/"]
RUN dotnet restore "PullParkingService/PullParkingService.csproj"
COPY . .
WORKDIR "/src/PullParkingService"
RUN dotnet build "PullParkingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PullParkingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PullParkingService.dll"]
