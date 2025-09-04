# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files (optimize layer caching)
COPY AirJourney-Blog.sln .
COPY AirJourney-Blog.PL/*.csproj ./AirJourney-Blog.PL/
COPY AirJourney-Blog.DAL/*.csproj ./AirJourney-Blog.DAL/
COPY AirJourney-Blog.BLL/*.csproj ./AirJourney-Blog.BLL/
COPY AirJourney-Blog.Service/*.csproj ./AirJourney-Blog.Service/
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish AirJourney-Blog.PL -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# HTTPS Configuration
EXPOSE 5000 5001
ENV ASPNETCORE_URLS=https://+:5001;http://+:5000
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=your_ssl_password
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/https/aspnetapp.pfx

# Create directory for HTTPS certificate
RUN mkdir -p /app/https

# Health check
HEALTHCHECK --interval=30s --timeout=3s \
  CMD curl -f https://localhost:5001/health || exit 1

ENTRYPOINT ["dotnet", "AirJourney-Blog.PL.dll"]