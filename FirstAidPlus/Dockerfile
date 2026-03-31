# Stage 1: Build Frontend (React/Vite)
FROM node:20 AS frontend-build
WORKDIR /src
# Copy package files
COPY FirstAidPlus/QAApp/package.json FirstAidPlus/QAApp/package-lock.json* ./FirstAidPlus/QAApp/
WORKDIR /src/FirstAidPlus/QAApp
RUN npm install
# Copy the rest
COPY FirstAidPlus/QAApp/ .
# Build the frontend (outputs to /src/FirstAidPlus/wwwroot/react/qa)
RUN npm run build

# Stage 2: Build Backend (.NET)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy project file and restore dependencies
COPY ["FirstAidPlus/FirstAidPlus.csproj", "FirstAidPlus/"]
RUN dotnet restore "FirstAidPlus/FirstAidPlus.csproj"
# Copy the entire source code
COPY . .
# Copy the pre-built frontend assets from the frontend-build stage
COPY --from=frontend-build /src/FirstAidPlus/wwwroot/react/qa /src/FirstAidPlus/wwwroot/react/qa

WORKDIR "/src/FirstAidPlus"
RUN dotnet build "FirstAidPlus.csproj" -c Release -o /app/build

# Stage 3: Publish
FROM build AS publish
RUN dotnet publish "FirstAidPlus.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 4: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "FirstAidPlus.dll"]
