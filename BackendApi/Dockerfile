# Estadio 1: Ejecución base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Estadio 2: Compilacion y Restore SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackendApi.csproj", "./"]
RUN dotnet restore "BackendApi.csproj"
COPY . .
RUN dotnet build "BackendApi.csproj" -c Release -o /app/build

# Estadio 3: Publicación
FROM build AS publish
RUN dotnet publish "BackendApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estadio 4: Imagen final de producción
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "BackendApi.dll"]
