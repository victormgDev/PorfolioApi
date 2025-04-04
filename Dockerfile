# Imagen base para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Imagen para construir la app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY ["PorfolioApi.csproj", "./"]
RUN dotnet restore "PorfolioApi.csproj"

# Copiar el resto del código
COPY . .

# Compilar la aplicación
RUN dotnet build "PorfolioApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar para producción
FROM build AS publish
RUN dotnet publish "PorfolioApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Railway usará el puerto que esté en $PORT
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "PorfolioApi.dll"]
