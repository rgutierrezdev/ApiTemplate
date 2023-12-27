# Project Structure

Here is how the .NET ApiTemplate APIs are structured.

This solution is based on Clean Architecture. In other words, Onion / Hexagonal Architecture. [Read about the advantages
and principles of Onion Architecture here](https://codewithmukesh.com/blog/onion-architecture-in-aspnet-core/)

## General Structure

This means that the entire solution is built in such a way that it can be scaled, maintained easily by teams of
developers. This Solution Primarily consists of the following .csproj files.

```
├── src
│   ├── Presentation
│   │   ├── CustomersAPI.csproj
│   │   ├── UsersAPI.csproj
│   │   └── VendorsAPI.csproj
│   ├── Core
│   │   ├── Application.csproj
│   │   └── Domain.csproj
│   ├── Infrastructure
│   │   ├── Infrastructure.csproj
│   │   └── Persistence.csproj
```

The idea is to build a very loosely coupled architecture following best practices and packages. Let’s see in brief what
responsibilities each of these projects handle.

## API Projects (CustomersAPI, UsersAPI, VendorsAPI)

Contains the API Controllers and Startup Logic. This is the entry point of the application. Also, the configuration
file lives under this project.

```
├── APIProject
│   ├── Controllers
│   ├── Helpers
│   └── appsettings.json
```

Note that each API project depends on:

- Application
- Infrastructure
- Persistence

## Application

This is one of the projects in the `Core` Folder apart from the Domain Project. Here you get to see Abstract Classes and
Interfaces that are inherited and implemented in the Infrastructure Project. This refers to Dependency Inversion.

```
├── Core
│   ├── Application
│   │   ├── Common
│   │   ├── EventHandlers
│   │   ├── Features
```

Each new feature should be added to the `Features` folder as a folder. This makes easier for developers to understand
the folder structure. Each of the feature folders like Catalog will have all the files related to it’s scope including
validators, dtos, handlers and so on.

Thus everything related to a feature will be found directly under that Feature folder.

Note that the Application project depends on:

- Domain

## Domain

Note that the Domain project does not depend on any other project.

As per Clean Architecture principles, the Core of this Solution i.e, Application and Domain projects do not depend on
any other projects. This helps achieve Dependency Inversion (The ‘D’ Principle of ‘SOLID’).
