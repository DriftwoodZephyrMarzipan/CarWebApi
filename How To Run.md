# Requirements

This assumes that you have already installed Visual Studio 2022, including the "ASP.NET and web development" and "Azure Development" workloads, and everything is up to date.

You will additionally want to install  .NET 9.0 or 10.0 from https://dotnet.microsoft.com/en-us/download/dotnet

If you are running this on windows, you'll need to install Docker for Windows if you want to use the container.

# Configuration files

## Appsettings

The appsettings file handles the endpoint specifications and database connection strings. I've left it to something default:

```
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=postgres;User Id=postgres;Password=Password;"
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://+:8080"
      },
      "Https": {
        "Url": "https://+:8081"
      }
    }
  }
``` 

It can be found in `/CarWebApi/CarWebApi/appsettings.json` prior to build, and in your output directory thereafter (if you wanted to copy the bin output to run it outside of visual studio/manually)

You may want to update yours.

## Docker-compose.override.yml

Of course, Using the container means we have to both override the connection string as well as add the port redirects. Here's the relevant portion:

```
services:
  carwebapi:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_HTTP_PORTS=8080
      - ASPNETCORE_HTTPS_PORTS=8081
      - ConnectionStrings__DefaultConnection=Server=host.docker.internal;Database=postgres;User Id=postgres;Password=Password;
    ports:
      - "8080:8080"
      - "8081:8081"
```

This file can be found in the same directory as the solution file, `/CarWebApi/docker-compose.override.yml`. Note that when used with linux, you may need to use the host `172.17.0.1` to reference localhost instead of `host.docker.internal`, if that alias is not setup.

# Launching

The easiest way is to start the application out of visual studio, and there's several profiles, but you can always use the raw dotnet commands as well.

Before any of these though, you should create your database and load it (see Loader below)

## CarWebApi

### Launch Profiles
Select the CarWebApi project as your startup and can use the following launch profiles:

#### Local

This will start the application running locally and automatically open up a browser instance with the swagger page, http://localhost:8080

#### Container (Dockerfile)

This will work as above, but the browser window will fail. This is because without docker compose, the ports assigned are random. Use `docker ps` on the command line to find the correct URL:

```
C:\Windows\system32>docker ps
CONTAINER ID   IMAGE           COMMAND                  CREATED          STATUS          PORTS                                              NAMES
72737ba6bb80   carwebapi:dev   "dotnet --roll-forwa…"   58 seconds ago   Up 58 seconds   0.0.0.0:32788->8080/tcp, 0.0.0.0:32789->8081/tcp   CarWebApi
```

In this case it's on ports 32788 and 32789. So open `http://localhost:32788/`.  Of course, unless you updated the connection string in the appsettings file (or otherwise overrode it), it will fail to hit a postgresql server on localhost so you have limited functionality.
 
Of course, you can always modify the settings in your container service to assign the ports or override the connection string (see the Docker-compose.override.yml section above for examples).

#### IIS Express

Works as per CarWebApi

### Run from command line

If you're feeling saucy, you can also just use the `dotnet run` command from the project directory `/CarWebApi/CarWebApi`, or `dotnet CarWebApi.dll` if you're in the bin directory.

## Loader

This program has it's own config file in `/CarWebApi/Loader/appsettings.json` to specify the PostGreSQL database connection string. It is not dockerized, so you can run it directly from VS or command line, and it takes only the data.json file from the ev download page as an argument.

It requires that you have first set up the schema, and then loads the ~100 meg data file in.

## docker-compose

This startup project will provide everything you need to handle the port redirects and connection string, using the docker compose settings specified above. Once you've set the appsettings and override files, you're good to go. 

However, it will not launch a browser for you, you'll have to do that yourself and open it to http://localhost:8080 yourself.

# Tests

The easiest way to run the two tests projects is just from visual studio's test explorer.

Make sure you've set up the `appsettings.json` file in `/CarWebApi/CarWebApiFunctionalTests/appsettings.json` properly first and created your database schema.

## Manually

You can also run the tests using `dotnet test` in their respective project folders, `/CarWebApi/CarWebApiFunctionalTests` and `/CarWebApi/CarWebApiUnitTests`
