FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
COPY UserManagement/UserManagement.csproj UserManagement/
RUN dotnet restore "UserManagement/UserManagement.csproj"

# Copy the entire project and build
COPY . .
WORKDIR /App/UserManagement
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "UserManagement.dll"]