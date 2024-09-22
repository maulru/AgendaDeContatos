FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BuscaDddAPI/BuscaDddAPI.csproj", "BuscaDddAPI/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "./BuscaDddAPI/BuscaDddAPI.csproj"
COPY . .
WORKDIR "/src/BuscaDddAPI"
RUN dotnet build "./BuscaDddAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BuscaDddAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BuscaDddAPI.dll"]