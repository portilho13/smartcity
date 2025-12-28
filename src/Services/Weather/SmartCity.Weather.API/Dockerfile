FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/Weather/SmartCity.Weather.API/SmartCity.Weather.API.csproj", "src/Services/Weather/SmartCity.Weather.API/"]
COPY ["src/Services/Weather/SmartCity.Weather.Core/SmartCity.Weather.Core.csproj", "src/Services/Weather/SmartCity.Weather.Core/"]
RUN dotnet restore "src/Services/Weather/SmartCity.Weather.API/SmartCity.Weather.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Weather/SmartCity.Weather.API"
RUN dotnet build "SmartCity.Weather.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.Weather.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5001

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.Weather.API.dll"]