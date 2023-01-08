FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RedisConsumerPullAds/RedisConsumerPullAds.csproj", "RedisConsumerPullAds/"]
RUN dotnet restore "RedisConsumerPullAds/RedisConsumerPullAds.csproj"
COPY . .
WORKDIR "/src/RedisConsumerPullAds"
RUN dotnet build "RedisConsumerPullAds.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedisConsumerPullAds.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedisConsumerPullAds.dll"]
