FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/UserManagement/SmartCity.UserManagement.API/SmartCity.UserManagement.API.csproj", "src/Services/UserManagement/SmartCity.UserManagement.API/"]
COPY ["src/Services/UserManagement/SmartCity.UserManagement.Core/SmartCity.UserManagement.Core.csproj", "src/Services/UserManagement/SmartCity.UserManagement.Core/"]
RUN dotnet restore "src/Services/UserManagement/SmartCity.UserManagement.API/SmartCity.UserManagement.API.csproj"

COPY . .
WORKDIR "/src/src/Services/UserManagement/SmartCity.UserManagement.API"
RUN dotnet build "SmartCity.UserManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
# Delete conflicting appsettings files from the DataLayer before publishing
RUN rm -f /src/src/DataLayer/SmartCity.DataLayer.UserService/appsettings*.json
RUN dotnet publish "SmartCity.UserManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5001

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SmartCity.UserManagement.API.dll"]