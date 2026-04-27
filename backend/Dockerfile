FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DanceAcademy.Api/DanceAcademy.Api.csproj", "DanceAcademy.Api/"]
COPY ["DanceAcademy.Domain/DanceAcademy.Domain.csproj", "DanceAcademy.Domain/"]
COPY ["DanceAcademy.Infrastructure/DanceAcademy.Infrastructure.csproj", "DanceAcademy.Infrastructure/"]
RUN dotnet restore "DanceAcademy.Api/DanceAcademy.Api.csproj"
COPY . .
WORKDIR "/src/DanceAcademy.Api"
RUN dotnet build "DanceAcademy.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DanceAcademy.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DanceAcademy.Api.dll"]
