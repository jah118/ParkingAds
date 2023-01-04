#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . ./PullADsToMQ/
#COPY ["PullADsToMQ.csproj", "PullADsToMQ/"]
RUN dotnet restore "PullADsToMQ/PullADsToMQ.csproj"
COPY . .
WORKDIR "/src/PullADsToMQ"
RUN dotnet build "PullADsToMQ.csproj" -c Release -o /app/build

FROM build AS publish
#RUN dotnet publish "PullADsToMQ.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish "PullADsToMQ.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PullADsToMQ.dll"]