FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DesafioBackend.Web/DesafioBackend.Web.csproj", "src/DesafioBackend.Web/"]
RUN dotnet restore "src/DesafioBackend.Web/DesafioBackend.Web.csproj"

COPY . .
WORKDIR "/src/src/DesafioBackend.Web"
RUN dotnet build "DesafioBackend.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DesafioBackend.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DesafioBackend.Web.dll"]

