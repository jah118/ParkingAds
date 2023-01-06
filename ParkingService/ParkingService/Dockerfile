FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ParkingService/ParkingService.csproj", "ParkingService/"]
RUN dotnet restore "ParkingService/ParkingService.csproj"
COPY . .
WORKDIR "/src/ParkingService"
RUN dotnet build "ParkingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParkingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParkingService.dll"]
