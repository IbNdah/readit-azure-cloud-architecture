# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY catalog/ ./catalog/
WORKDIR /app/catalog
RUN dotnet restore 
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/catalog/out .

ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "catalog.dll"]