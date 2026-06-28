# 1. Base runtime stage using Linux ASP.NET Core image
FROM microsoft AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Build stage using Linux .NET SDK image
FROM microsoft AS with-node
WORKDIR /src

# Install Node.js v18 and build essentials for Linux
RUN apt-get update && apt-get install -y \
    curl \
    gnupg \
    && mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://nodesource.com | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://nodesource.com nodistro main" | tee /etc/apt/sources.list.dir/nodesource.list \
    && apt-get update && apt-get install -y nodejs \
    && rm -rf /var/lib/apt/lists/*

# 3. Project compilation stage
FROM with-node AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the API project file and restore dependencies
COPY ["AspCoreForReactApi.csproj", "AspCoreForReactApi/"]
RUN dotnet restore "AspCoreForReactApi/AspCoreForReactApi.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/AspCoreForReactApi"

# Build the project using Linux environment variable syntax ($ instead of %)
RUN dotnet build "AspCoreForReactApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 4. Publication stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AspCoreForReactApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 5. Final production image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspCoreForReactApi.dll"]
