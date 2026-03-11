# ETAPA 1: Construcción (Build Environment)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos solo el archivo del proyecto principal (Más seguro que depender del .sln)
COPY ["src/IncidentApi/IncidentApi.csproj", "src/IncidentApi/"]
RUN dotnet restore "src/IncidentApi/IncidentApi.csproj"

# Copiamos el resto del código fuente
COPY . .
WORKDIR "/src/src/IncidentApi"

# Compilamos la aplicación en modo Release
RUN dotnet publish "IncidentApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ETAPA 2: Producción (Runtime Environment)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copiamos SOLO los binarios compilados de la Etapa 1
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

# Comando de arranque
ENTRYPOINT ["dotnet", "IncidentApi.dll"]