FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LightFeatherWebAPI.csproj", "./"]
RUN dotnet restore "LightFeatherWebAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "LightFeatherWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LightFeatherWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LightFeatherWebAPI.dll"]
