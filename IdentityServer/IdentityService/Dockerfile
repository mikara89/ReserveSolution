#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["IdentityServer/IdentityService/IdentityService.csproj", "IdentityServer/IdentityService/"]
COPY ["IdentityServer/IdentityService.Data/IdentityService.Data.csproj", "IdentityServer/IdentityService.Data/"]
COPY ["IdentityServer/IdentityService.Messaging.Sender/IdentityService.Messaging.Sender.csproj", "IdentityServer/IdentityService.Messaging.Sender/"]
RUN dotnet restore "IdentityServer/IdentityService/IdentityService.csproj"
COPY . .
WORKDIR "/src/IdentityServer/IdentityService"
RUN dotnet build "IdentityService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.dll"]
