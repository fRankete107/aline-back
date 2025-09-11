# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy source code and build the application
COPY . ./
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release --no-build -o out

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN addgroup --system --gid 1001 pilatesapp && \
    adduser --system --uid 1001 --gid 1001 --shell /bin/false pilatesapp

# Set working directory
WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/out .

# Create logs directory and set permissions
RUN mkdir -p /app/logs/errors /app/logs/performance /app/logs/business && \
    chown -R pilatesapp:pilatesapp /app/logs

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Expose port
EXPOSE 8080

# Switch to non-root user
USER pilatesapp

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "PilatesStudioAPI.dll"]