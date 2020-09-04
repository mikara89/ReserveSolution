#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["APIs/Team/Team.Api/Team.Api.csproj", "APIs/Team/Team.Api/"]
COPY ["APIs/Team/Team.Data/Team.Data.csproj", "APIs/Team/Team.Data/"]
RUN dotnet restore "APIs/Team/Team.Api/Team.Api.csproj"
COPY . .
WORKDIR "/src/APIs/Team/Team.Api"
RUN dotnet build "Team.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Team.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Team.Api.dll"]