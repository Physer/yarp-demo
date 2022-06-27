#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Yarp.Forwarder/Yarp.Forwarder.csproj", "src/Yarp.Forwarder/"]
RUN dotnet restore "src/Yarp.Forwarder/Yarp.Forwarder.csproj"
COPY . .
WORKDIR "/src/src/Yarp.Forwarder"
RUN dotnet build "Yarp.Forwarder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Yarp.Forwarder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Yarp.Forwarder.dll"]