FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["VerySimpleAPI/VerySimpleAPI.csproj", "VerySimpleAPI/"]
RUN dotnet restore "VerySimpleAPI/VerySimpleAPI.csproj"
COPY . .
WORKDIR "/src/VerySimpleAPI"
RUN dotnet build "VerySimpleAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VerySimpleAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "VerySimpleAPI.dll"]