#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["GCPCloudRunSample.csproj", ""]
RUN dotnet restore "./GCPCloudRunSample.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "GCPCloudRunSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GCPCloudRunSample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN apt-get update -y
RUN apt-get install -y libc6-dev
RUN apt-get install -y iputils-ping
ENTRYPOINT ["dotnet", "GCPCloudRunSample.dll"]