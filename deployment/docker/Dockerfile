# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

# Set the working directory inside the container
WORKDIR /app

# Copy project files and restore dependencies
COPY GetTeacher.Server/*.csproj ./
RUN dotnet restore -r linux-musl-x64

# Copy the remaining source code and build the application
COPY GetTeacher.Server/. ./
RUN dotnet publish \
    -c Release \
    -r linux-musl-x64 \
    --self-contained=true \
    -o /out

# Stage 2: Use a minimal base image
FROM alpine:3.18

# Install required dependencies for .NET applications
RUN apk add --no-cache \
    libstdc++ \
    libgcc \
    icu-data-full \
    icu-libs

# Set the working directory for the runtime container
WORKDIR /app

# Copy the published application from the builder stage
COPY --from=builder /out ./

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Release

# Define the entry point for the container
ENTRYPOINT ["/app/GetTeacher.Server"]
