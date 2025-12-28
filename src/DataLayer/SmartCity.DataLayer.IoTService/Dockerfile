FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/DataLayer/SmartCity.DataLayer.IoTService/SmartCity.DataLayer.IoTService.csproj", "src/DataLayer/SmartCity.DataLayer.IoTService/"]
RUN dotnet restore "src/DataLayer/SmartCity.DataLayer.IoTService/SmartCity.DataLayer.IoTService.csproj"

COPY . .
WORKDIR "/src/src/DataLayer/SmartCity.DataLayer.IoTService"
RUN dotnet build "SmartCity.DataLayer.IoTService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.DataLayer.IoTService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5105

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.DataLayer.IoTService.dll"]