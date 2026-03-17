FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY catalog/ ./catalog/
RUN ls ./catalog

RUN dotnet restore ./catalog/catalog.csproj
RUN dotnet publish ./catalog/catalog.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "catalog.dll"]
