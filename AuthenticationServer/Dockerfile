#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AuthenticationServer/AuthenticationServer.csproj", "AuthenticationServer/"]
RUN dotnet restore "AuthenticationServer/AuthenticationServer.csproj"
COPY . .
WORKDIR "/src/AuthenticationServer"
RUN dotnet build "AuthenticationServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthenticationServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt-get update
RUN apt-get -y install libsasl2-modules-gssapi-mit 
ENTRYPOINT ["dotnet", "AuthenticationServer.dll"]