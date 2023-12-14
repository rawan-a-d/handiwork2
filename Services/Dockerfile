# https://hub.docker.com/_/microsoft-dotnet-sdk
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

# set the working directory
WORKDIR /app

# for installing dependencies
COPY *.csproj ./
# pull down dependencies/dependencies
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# build run time version
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
# copy the output from the build environment
COPY --from=build-env /app/out .

# what we want to run
ENTRYPOINT [ "dotnet", "Services.dll" ]