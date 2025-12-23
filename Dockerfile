FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /source
COPY . .
WORKDIR /source/BlazorApp2/Server
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "BlazorApp2.Server.dll"]