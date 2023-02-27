#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ZME.API/ZME.API.csproj", "ZME.API/"]
COPY ["ZME.API.Shared/ZME.API.Shared.csproj", "ZME.API.Shared/"]
RUN dotnet restore "ZME.API/ZME.API.csproj"
COPY . .
WORKDIR "/src/ZME.API"
RUN dotnet build "ZME.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ZME.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZME.API.dll"]