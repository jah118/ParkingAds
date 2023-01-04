FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /appx

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
#COPY ["RouteSlip/RouteSlip.csproj", "RouteSlip/"]
COPY . ./RouteSlip/
RUN dotnet restore "RouteSlip/RouteSlip.csproj"
COPY . .
WORKDIR "/src/RouteSlip"
RUN dotnet build "RouteSlip.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RouteSlip.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RouteSlip.dll"]
