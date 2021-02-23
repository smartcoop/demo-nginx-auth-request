# Build temporary build image.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build-env

WORKDIR /app

# Restore as distinct layers.
COPY AuthenticationServer.csproj AuthenticationServer/AuthenticationServer.csproj
RUN dotnet restore AuthenticationServer/AuthenticationServer.csproj

# Copy everything else and build.
COPY . AuthenticationServer/
RUN dotnet build -c Release -o out AuthenticationServer/AuthenticationServer.csproj


# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "AuthenticationServer.dll"]