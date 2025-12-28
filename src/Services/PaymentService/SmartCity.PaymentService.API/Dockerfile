FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/PaymentService/SmartCity.PaymentService.API/SmartCity.PaymentService.API.csproj", "src/Services/PaymentService/SmartCity.PaymentService.API/"]
COPY ["src/Services/PaymentService/SmartCity.PaymentService.Core/SmartCity.PaymentService.Core.csproj", "src/Services/PaymentService/SmartCity.PaymentService.Core/"]
RUN dotnet restore "src/Services/PaymentService/SmartCity.PaymentService.API/SmartCity.PaymentService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/PaymentService/SmartCity.PaymentService.API"
RUN dotnet build "SmartCity.PaymentService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN rm -f /src/src/DataLayer/SmartCity.DataLayer.PaymentService/appsettings*.json
RUN dotnet publish "SmartCity.PaymentService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5004

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.PaymentService.API.dll"]