FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/DataLayer/SmartCity.DataLayer.AnalyticsService/SmartCity.DataLayer.AnalyticsService.csproj", "src/DataLayer/SmartCity.DataLayer.AnalyticsService/"]
RUN dotnet restore "src/DataLayer/SmartCity.DataLayer.AnalyticsService/SmartCity.DataLayer.AnalyticsService.csproj"

COPY . .
WORKDIR "/src/src/DataLayer/SmartCity.DataLayer.AnalyticsService"
RUN dotnet build "SmartCity.DataLayer.AnalyticsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.DataLayer.AnalyticsService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5106

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.DataLayer.AnalyticsService.dll"]