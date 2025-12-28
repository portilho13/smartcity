FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/AnalyticsService/SmartCity.AnalyticsService.API/SmartCity.AnalyticsService.API.csproj", "src/Services/AnalyticsService/SmartCity.AnalyticsService.API/"]
COPY ["src/Services/AnalyticsService/SmartCity.AnalyticsService.Core/SmartCity.AnalyticsService.Core.csproj", "src/Services/AnalyticsService/SmartCity.AnalyticsService.Core/"]
RUN dotnet restore "src/Services/AnalyticsService/SmartCity.AnalyticsService.API/SmartCity.AnalyticsService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/AnalyticsService/SmartCity.AnalyticsService.API"
RUN dotnet build "SmartCity.AnalyticsService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN rm -f /src/src/DataLayer/SmartCity.DataLayer.AnalyticsService/appsettings*.json
RUN dotnet publish "SmartCity.AnalyticsService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5006

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.AnalyticsService.API.dll"]