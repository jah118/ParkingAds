#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["pullADs/pullADs.csproj", "pullADs/"]
RUN dotnet restore "pullADs/pullADs.csproj"
COPY . .
WORKDIR "/src/pullADs"
RUN dotnet build "pullADs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pullADs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pullADs.dll"]