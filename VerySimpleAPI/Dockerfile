# Build temporary build image.
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build-env

WORKDIR /app

# Restore as distinct layers.
COPY VerySimpleAPI.csproj VerySimpleAPI/VerySimpleAPI.csproj
RUN dotnet restore VerySimpleAPI/VerySimpleAPI.csproj

# Copy everything else and build.
COPY . VerySimpleAPI/
RUN dotnet build -c Release -o out VerySimpleAPI/VerySimpleAPI.csproj


# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "VerySimpleAPI.dll"]