# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Exponer el puerto 80 para la aplicación
EXPOSE 80

# Copiar todos los archivos del proyecto al contenedor
COPY ["Medicines.API/Medicines.API.csproj", "Medicines.API/"]
COPY ["Medicines.Infrastructure/Medicines.Infrastructure.csproj", "Medicines.Infrastructure/"]
COPY ["Medicines.Application/Medicines.Application.csproj", "Medicines.Application/"]
COPY ["Medicines.Core/Medicines.Core.csproj", "Medicines.Core/"]

# Restaurar las dependencias
RUN dotnet restore "Medicines.API/Medicines.API.csproj"

COPY . .

# Construir la aplicación en modo Release
RUN dotnet publish "Medicines.API/Medicines.API.csproj" -c Release -o /app/out

# Etapa 2: Imagen final para ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar los archivos compilados de la etapa de build
COPY --from=build /app/out .

# Setup TimeZone as Argentina
ENV TZ=America/Buenos_Aires

RUN apt-get update && \
    apt-get install -y gnupg curl && \
    curl -fsSL https://www.mongodb.org/static/pgp/server-4.4.asc | gpg --dearmor -o /usr/share/keyrings/mongodb-archive-keyring.gpg && \
    echo "deb [arch=amd64,arm64 signed-by=/usr/share/keyrings/mongodb-archive-keyring.gpg] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/4.4 multiverse" | tee /etc/apt/sources.list.d/mongodb-org-4.4.list && \
    apt-get update && \
    apt-get install -y mongodb-org-tools && \
    rm -rf /var/lib/apt/lists/*

# Configurar el punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "Medicines.API.dll"]
