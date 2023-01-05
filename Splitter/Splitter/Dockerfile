FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Splitter/Splitter.csproj", "Splitter/"]
RUN dotnet restore "Splitter/Splitter.csproj"
COPY . .
WORKDIR "/src/Splitter"
RUN dotnet build "Splitter.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Splitter.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Splitter.dll"]
