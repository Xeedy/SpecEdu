# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/SpecEdu.Web.App/SpecEdu.Web.App.csproj", "src/SpecEdu.Web.App/"]
COPY ["src/SpecEdu.Domain/SpecEdu.Domain.csproj", "src/SpecEdu.Domain/"]
COPY ["src/SpecEdu.Application/SpecEdu.Application.csproj", "src/SpecEdu.Application/"]
COPY ["src/SpecEdu.Infrastructure/SpecEdu.Infrastructure.csproj", "src/SpecEdu.Infrastructure/"]

RUN dotnet restore "src/SpecEdu.Web.App/SpecEdu.Web.App.csproj"

COPY . .

WORKDIR "/src/src/SpecEdu.Web.App"

RUN dotnet build "SpecEdu.Web.App.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish

RUN dotnet publish "SpecEdu.Web.App.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SpecEdu.Web.App.dll"]
