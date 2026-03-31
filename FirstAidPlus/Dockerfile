# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["FirstAidPlus/FirstAidPlus.csproj", "FirstAidPlus/"]
RUN dotnet restore "FirstAidPlus/FirstAidPlus.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/FirstAidPlus"
RUN dotnet build "FirstAidPlus.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FirstAidPlus.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "FirstAidPlus.dll"]
