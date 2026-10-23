FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos los archivos de proyectos individuales para restaurar dependencias
COPY ["SistemaVenta.AplicacionWeb/SistemaVenta.AplicacionWeb.csproj", "SistemaVenta.AplicacionWeb/"]
COPY ["SistemaVenta.BLL/SistemaVenta.BLL.csproj", "SistemaVenta.BLL/"]
COPY ["SistemaVenta.DAL/SistemaVenta.DAL.csproj", "SistemaVenta.DAL/"]
COPY ["SistemaVenta.Entity/SistemaVenta.Entity.csproj", "SistemaVenta.Entity/"]
COPY ["SistemaVenta.IOC/SistemaVenta.IOC.csproj", "SistemaVenta.IOC/"]

RUN dotnet restore "SistemaVenta.AplicacionWeb/SistemaVenta.AplicacionWeb.csproj"

# Copiamos el resto del código fuente
COPY . .
WORKDIR "/src/SistemaVenta.AplicacionWeb"
RUN dotnet build "SistemaVenta.AplicacionWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SistemaVenta.AplicacionWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SistemaVenta.AplicacionWeb.dll"]