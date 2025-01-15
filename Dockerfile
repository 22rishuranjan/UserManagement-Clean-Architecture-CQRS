# Use the official .NET 8 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file
COPY UserManagement.sln ./

# Copy project files individually for all projects in the solution
COPY src/UserManagement.Api/*.csproj ./src/UserManagement.Api/
COPY src/UserManagement.Application/*.csproj ./src/UserManagement.Application/
COPY src/UserManagement.Domain/*.csproj ./src/UserManagement.Domain/
COPY src/UserManagement.Infrastructure/*.csproj ./src/UserManagement.Infrastructure/

# Restore dependencies for the solution
RUN dotnet restore

# Copy the entire project into the container
COPY . .

# Set the working directory to the API project
WORKDIR /app/src/UserManagement.Api

# Build and publish the application
RUN dotnet publish -c Release -o /out

# Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Expose the port your app runs on
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "UserManagement.Api.dll"]
