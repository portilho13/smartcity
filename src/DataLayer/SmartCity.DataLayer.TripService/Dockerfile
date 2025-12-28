FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/DataLayer/SmartCity.DataLayer.TripService/SmartCity.DataLayer.TripService.csproj", "src/DataLayer/SmartCity.DataLayer.TripService/"]
RUN dotnet restore "src/DataLayer/SmartCity.DataLayer.TripService/SmartCity.DataLayer.TripService.csproj"

COPY . .
WORKDIR "/src/src/DataLayer/SmartCity.DataLayer.TripService"
RUN dotnet build "SmartCity.DataLayer.TripService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.DataLayer.TripService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5103

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.DataLayer.TripService.dll"]