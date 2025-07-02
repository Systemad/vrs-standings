# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build Stage: Build the React app
FROM node:22 AS builder

WORKDIR /app
COPY package.json package-lock.json ./

#RUN corepack enable
RUN npm install
COPY . .
RUN npm run build

# Production Stage: Serve the app with Caddy
FROM caddy:2.9-alpine
WORKDIR /usr/share/caddy

# Copy built static files including index.html, etc.
COPY --from=builder /app/build/ .

# Copy the Caddyfile with reverse proxy config
COPY Caddyfile /etc/caddy/Caddyfile

EXPOSE 80

# Run Caddy
CMD ["caddy", "run", "--config", "/etc/caddy/Caddyfile", "--adapter", "caddyfile"]

# This stage is used when running from VS in fast mode (Default for Debug configuration)
#FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081


# This stage is used to build the service project
#FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS with-node
#RUN apt-get update
#RUN apt-get install curl
#RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
#RUN apt-get -y install nodejs
#RUN corepack enable

#FROM with-node AS build
#ARG BUILD_CONFIGURATION=Release
#RUN apt-get update && apt-get install -y curl git && rm -rf /var/lib/apt/lists/*
#WORKDIR /src
#COPY ["vrsranking.Server/vrsranking.Server.csproj", "vrsranking.Server/"]
#COPY ["vrsranking.client/vrsranking.client.esproj", "vrsranking.client/"]
#RUN dotnet restore "./vrsranking.Server/vrsranking.Server.csproj"
#COPY . .
#WORKDIR "/src/vrsranking.Server"
#RUN dotnet build "./vrsranking.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./vrsranking.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#COPY --from=build /repos /app/repos

#COPY setup.sh /app/setup.sh
#RUN chmod +x /app/setup.sh
#RUN /app/setup.sh
#ENTRYPOINT ["dotnet", "vrsranking.Server.dll"]
