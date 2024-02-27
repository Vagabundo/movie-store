FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
# EXPOSE 5004
# EXPOSE 7039

# ENV ASPNETCORE_URLS=http://+:5004;https://+:7039
# ENV ASPNETCORE_Kestrel__Certificates__Default__Password=secret
# ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx

# USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG configuration=Release
WORKDIR /src
COPY ["./src", "."]
RUN dotnet restore "MovieStore.Api/MovieStore.Api.csproj"
COPY . .
WORKDIR "/src/MovieStore.Api"
RUN dotnet build "MovieStore.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "MovieStore.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieStore.Api.dll"]
