FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DesafioBackend.Api/DesafioBackend.Api.csproj", "src/DesafioBackend.Api/"]
COPY ["src/DesafioBackend.Application/DesafioBackend.Application.csproj", "src/DesafioBackend.Application/"]
COPY ["src/DesafioBackend.Domain/DesafioBackend.Domain.csproj", "src/DesafioBackend.Domain/"]
COPY ["src/DesafioBackend.Infrastructure/DesafioBackend.Infrastructure.csproj", "src/DesafioBackend.Infrastructure/"]
RUN dotnet restore "src/DesafioBackend.Api/DesafioBackend.Api.csproj"

COPY . .
WORKDIR "/src/src/DesafioBackend.Api"
RUN dotnet build "DesafioBackend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DesafioBackend.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DesafioBackend.Api.dll"]

