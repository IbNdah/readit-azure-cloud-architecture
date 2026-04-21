FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY cart/ ./cart/
RUN dotnet restore ./cart/cart.csproj
RUN dotnet publish ./cart/cart.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "cart.dll"]