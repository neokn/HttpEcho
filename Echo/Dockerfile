ARG IMAGE_TAG

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-cbl-mariner2.0-distroless AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS sdk
WORKDIR /src
RUN apt update
RUN apt install -y clang zlib1g-dev

FROM sdk AS build
WORKDIR /src
COPY ["Echo/Echo.csproj", "Echo/"]
RUN dotnet restore "Echo/Echo.csproj"
COPY . .
WORKDIR "/src/Echo"
RUN dotnet build --no-restore "Echo.csproj" -c Release -p:AssemblyVersion=$IMAGE_TAG -p:FileVersion=$IMAGE_TAG

FROM build AS publish
RUN dotnet publish --no-build "Echo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish --chown=1654:1654 /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["./Echo"]
