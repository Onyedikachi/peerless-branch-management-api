#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["retail-teams-management-api/Presentation-Api.csproj", "retail-teams-management-api/"]
COPY ["Retail.Branch.Services/Retail.Branch.Services.csproj", "Retail.Branch.Services/"]
COPY ["Retail.Branch.Core/Retail.Branch.Core.csproj", "Retail.Branch.Core/"]
COPY ["Retail.Branch.Infrastructure/Retail.Branch.Infrastructure.csproj", "Retail.Branch.Infrastructure/"]
RUN dotnet restore "retail-teams-management-api/Presentation-Api.csproj"
COPY . .
WORKDIR "/src/retail-teams-management-api"
RUN dotnet build "Presentation-Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Presentation-Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation-Api.dll"]
