FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/TripManagement/SmartCity.TripManagement.API/SmartCity.TripManagement.API.csproj", "src/Services/TripManagement/SmartCity.TripManagement.API/"]
COPY ["src/Services/TripManagement/SmartCity.TripManagement.Core/SmartCity.TripManagement.Core.csproj", "src/Services/TripManagement/SmartCity.TripManagement.Core/"]
COPY ["src/Services/VehicleManagement/SmartCity.VehicleManagement.Core/SmartCity.VehicleManagement.Core.csproj", "src/Services/VehicleManagement/SmartCity.VehicleManagement.Core/"]
COPY ["src/Services/PaymentService/SmartCity.PaymentService.Core/SmartCity.PaymentService.Core.csproj", "src/Services/PaymentService/SmartCity.PaymentService.Core/"]
COPY ["src/Services/IoTService/SmartCity.IoTService.Core/SmartCity.IoTService.Core.csproj", "src/Services/IoTService/SmartCity.IoTService.Core/"]
COPY ["src/Services/Weather/SmartCity.Weather.Core/SmartCity.Weather.Core.csproj", "src/Services/Weather/SmartCity.Weather.Core/"]
RUN dotnet restore "src/Services/TripManagement/SmartCity.TripManagement.API/SmartCity.TripManagement.API.csproj"

COPY . .
WORKDIR "/src/src/Services/TripManagement/SmartCity.TripManagement.API"
RUN dotnet build "SmartCity.TripManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN rm -f /src/src/DataLayer/SmartCity.DataLayer.TripService/appsettings*.json
RUN dotnet publish "SmartCity.TripManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5003

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.TripManagement.API.dll"]