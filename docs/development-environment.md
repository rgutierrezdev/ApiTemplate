# Development Environment

Let's get started with setting up the Development Environment for .NET ApiTemplate APIs

This Solution Development needs you to have the following applications / tools available on your machine. Please Note
that this project is being built on a Windows 10 Machine.

## .NET SDK

As mentioned earlier, this project is built with the latest available .NET SDK, which is .NET 7.0.

Ensure that you have the latest version of the SDK
available - [Download from Microsoft](https://dotnet.microsoft.com/download/dotnet/7.0)

## Entity Framework CLI

In order to run create EF migrations or update the database using EF, you need the Entity Framework Core Command Line
Tools. It can be installed by running the following into the terminal:

```
dotnet tool install --global dotnet-ef --version 7.0.14
```

## IDE

Jetbrains Rider is the recommended IDE to use. If you are not already using it, consider switching to it. It’s
definitely worth it!

However, you are always free to use your choice of IDEs as well.

if you are using VS Code make sure to install
the [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension.

## SQL Server

.NET ApiTemplate APIs uses SQL Server as its main database provider for main DB and Hangfire.

Download SQL Server
Installer - [Get from microsoft.com](https://www.microsoft.com/en-us/download/details.aspx?id=101064)

You may also want to use an IDE to manage the DB data:

- [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16#download-ssms):
  from microsoft (free)
- [DataGrip](https://www.jetbrains.com/datagrip/download): from JetBrains (paid)
- [DBeaver](https://dbeaver.io/download/): open source (free)

## API Testing Tools (Optional)

### POSTMAN

When it comes to API Testing, Postman is the recommended tool

Download Postman - [Get from postman.com](https://www.postman.com/downloads/)

### Thunderclient Extension (VS Code)

It’s lightweight when compared to Postman,and also let’s you test without leaving the IDE. Search for Thunderclient
under extensions and get it installed.

### Docker (Optional)

Ensure that Docker Desktop is installed on your machine.

Download Docker - [Get from docker.com](https://www.docker.com/products/docker-desktop/)

# Update Database

If you were given an updated ApiTemplate database backup you may not need to do this. in any case it is good to update your
database with all the migrations that are missing in your DB. To do it, you need do the following:

- Make sure that you got to have valid database configuration in your `appsettings.Development.json`
- Ensure that you have PostgreSQL up and running on your development machine.
- Open your terminal on to the `UsersAPI` Project.
- Run the following command to tell EF to update the database with the missing migrations

```
dotnet ef database update
```
