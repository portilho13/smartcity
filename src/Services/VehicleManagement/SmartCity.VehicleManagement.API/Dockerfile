FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/VehicleManagement/SmartCity.VehicleManagement.API/SmartCity.VehicleManagement.API.csproj", "src/Services/VehicleManagement/SmartCity.VehicleManagement.API/"]
COPY ["src/Services/VehicleManagement/SmartCity.VehicleManagement.Core/SmartCity.VehicleManagement.Core.csproj", "src/Services/VehicleManagement/SmartCity.VehicleManagement.Core/"]
RUN dotnet restore "src/Services/VehicleManagement/SmartCity.VehicleManagement.API/SmartCity.VehicleManagement.API.csproj"

COPY . .
WORKDIR "/src/src/Services/VehicleManagement/SmartCity.VehicleManagement.API"
RUN dotnet build "SmartCity.VehicleManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN rm -f /src/src/DataLayer/SmartCity.DataLayer.VehicleService/appsettings*.json

RUN dotnet publish "SmartCity.VehicleManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5002

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.VehicleManagement.API.dll"]