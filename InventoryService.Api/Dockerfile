#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 15269
EXPOSE 15475

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG DEV_CERT_PASSWORD="simple-cert"
WORKDIR /src
COPY ["InventoryService.Api/InventoryService.Api.csproj", "InventoryService.Api/"]
RUN dotnet restore "InventoryService.Api/InventoryService.Api.csproj"
COPY . .
WORKDIR "/src/InventoryService.Api"
RUN dotnet build "InventoryService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Install the dotnet dev-certs tool and generate a certificate
RUN dotnet dev-certs https --clean
#RUN dotnet dev-certs https --trust
RUN dotnet dev-certs https --export-path /src/aspnetapp.pfx --password $DEV_CERT_PASSWORD

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "InventoryService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG DEV_CERT_PASSWORD="simple-cert"
ARG DEV_CERT_PATH="/certs/aspnetapp.pfx"
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the generated certificate from the build stage
COPY --from=build /src/aspnetapp.pfx $DEV_CERT_PATH

# Set environment variables to use the certificate
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=$DEV_CERT_PASSWORD
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=$DEV_CERT_PATH
ENV ASPNETCORE_URLS="http://0.0.0.0:15269;https://0.0.0.0:15475"

ENTRYPOINT ["dotnet", "InventoryService.Api.dll"]
