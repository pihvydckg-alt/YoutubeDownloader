FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY DownloaderApi.csproj ./
RUN dotnet restore
COPY Program.cs ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENV PORT=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DownloaderApi.dll"]
