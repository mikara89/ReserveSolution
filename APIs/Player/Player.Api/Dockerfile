#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["APIs/Player/Player.Api/Player.Api.csproj", "APIs/Player/Player.Api/"]
COPY ["APIs/Player/Player.Data/Player.Data.csproj", "APIs/Player/Player.Data/"]
RUN dotnet restore "APIs/Player/Player.Api/Player.Api.csproj"
COPY . .
WORKDIR "/src/APIs/Player/Player.Api"
RUN dotnet build "Player.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Player.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Player.Api.dll"]
