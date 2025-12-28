FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/Gateway/SmartCity.ApiGateway/SmartCity.ApiGateway.csproj", "src/Services/Gateway/SmartCity.ApiGateway/"]
RUN dotnet restore "src/Services/Gateway/SmartCity.ApiGateway/SmartCity.ApiGateway.csproj"

COPY . .
WORKDIR "/src/src/Services/Gateway/SmartCity.ApiGateway"
RUN dotnet build "SmartCity.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5010

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.ApiGateway.dll"]