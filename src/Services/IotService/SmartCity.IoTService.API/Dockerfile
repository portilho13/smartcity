FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/IoTService/SmartCity.IoTService.API/SmartCity.IoTService.API.csproj", "src/Services/IoTService/SmartCity.IoTService.API/"]
COPY ["src/Services/IoTService/SmartCity.IoTService.Core/SmartCity.IoTService.Core.csproj", "src/Services/IoTService/SmartCity.IoTService.Core/"]
RUN dotnet restore "src/Services/IoTService/SmartCity.IoTService.API/SmartCity.IoTService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/IoTService/SmartCity.IoTService.API"
RUN dotnet build "SmartCity.IoTService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN rm -f /src/src/DataLayer/SmartCity.DataLayer.IoTService/appsettings*.json
RUN dotnet publish "SmartCity.IoTService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5005

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.IoTService.API.dll"]