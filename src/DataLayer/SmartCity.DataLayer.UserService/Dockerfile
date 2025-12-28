FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/DataLayer/SmartCity.DataLayer.UserService/SmartCity.DataLayer.UserService.csproj", "src/DataLayer/SmartCity.DataLayer.UserService/"]
RUN dotnet restore "src/DataLayer/SmartCity.DataLayer.UserService/SmartCity.DataLayer.UserService.csproj"

COPY . .
WORKDIR "/src/src/DataLayer/SmartCity.DataLayer.UserService"
RUN dotnet build "SmartCity.DataLayer.UserService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmartCity.DataLayer.UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5101

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.DataLayer.UserService.dll"]