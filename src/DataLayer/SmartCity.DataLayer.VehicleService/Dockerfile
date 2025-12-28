FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/DataLayer/SmartCity.DataLayer.VehicleService/SmartCity.DataLayer.VehicleService.csproj", "src/DataLayer/SmartCity.DataLayer.VehicleService/"]
RUN dotnet restore "src/DataLayer/SmartCity.DataLayer.VehicleService/SmartCity.DataLayer.VehicleService.csproj"

COPY . .
WORKDIR "/src/src/DataLayer/SmartCity.DataLayer.VehicleService"
RUN dotnet build "SmartCity.DataLayer.VehicleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.DataLayer.VehicleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5102

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.DataLayer.VehicleService.dll"]