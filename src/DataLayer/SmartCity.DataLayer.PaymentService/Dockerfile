FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/DataLayer/SmartCity.DataLayer.PaymentService/SmartCity.DataLayer.PaymentService.csproj", "src/DataLayer/SmartCity.DataLayer.PaymentService/"]
RUN dotnet restore "src/DataLayer/SmartCity.DataLayer.PaymentService/SmartCity.DataLayer.PaymentService.csproj"

COPY . .
WORKDIR "/src/src/DataLayer/SmartCity.DataLayer.PaymentService"
RUN dotnet build "SmartCity.DataLayer.PaymentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.DataLayer.PaymentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5104

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.DataLayer.PaymentService.dll"]