#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["GatewayAPI/GateWayAPI.Web/GateWayAPI.Web.csproj", "GatewayAPI/GateWayAPI.Web/"]
RUN dotnet restore "GatewayAPI/GateWayAPI.Web/GateWayAPI.Web.csproj"
COPY . .
WORKDIR "/src/GatewayAPI/GateWayAPI.Web"
RUN dotnet build "GateWayAPI.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GateWayAPI.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GateWayAPI.Web.dll"]