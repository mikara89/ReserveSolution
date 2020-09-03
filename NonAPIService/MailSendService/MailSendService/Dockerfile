#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NonAPIService/MailSendService/MailSendService/SendMailService.csproj", "NonAPIService/MailSendService/MailSendService/"]
COPY ["NonAPIService/MailSendService/MailSendService.Service/SendMailService.Service.csproj", "NonAPIService/MailSendService/MailSendService.Service/"]
COPY ["NonAPIService/MailSendService/MailSendService.Messaging.Receive/SendMailService.Messaging.Receive.csproj", "NonAPIService/MailSendService/MailSendService.Messaging.Receive/"]
RUN dotnet restore "NonAPIService/MailSendService/MailSendService/SendMailService.csproj"
COPY . .
WORKDIR "/src/NonAPIService/MailSendService/MailSendService"
RUN dotnet build "SendMailService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SendMailService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SendMailService.dll"]
