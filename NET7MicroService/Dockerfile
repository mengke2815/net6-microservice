#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NET7MicroService/NET7MicroService.csproj", "NET7MicroService/"]
COPY ["CommonLibrary/CommonLibrary.csproj", "CommonLibrary/"]
RUN dotnet restore "NET7MicroService/NET7MicroService.csproj"
COPY . .
WORKDIR "/src/NET7MicroService"
RUN dotnet build "NET7MicroService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NET7MicroService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NET7MicroService.dll"]