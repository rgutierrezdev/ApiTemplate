# Migrations

### Adding Database Migrations for Entity Framework Core

So, you have already added new entities into the Domain project, modified an existing entity or want to recreate all the
pre-generated migrations? Here is how to proceed.

Remember to download the database provider and install the EF Core Command Tools as mentioned
here - [Development Environment](development-environment.md)

As of now there is only one EF Core DB Context class `ApplicationDbContext`, where you would ideally reference your new
entities.

To start with generating the database migrations:

- Make sure that you got to have valid database configuration in your `appsettings.Development.json`
- Ensure that you have PostgreSQL up and running on your development machine.
- Open your terminal on to the `UsersAPI` Project (**Note**: regardless of which project you are working on, always use
  this
  project to generate migrations).
- Add the new migration with the following command:

```
dotnet ef migrations add <MigrationName> --project ../../Infra/Persistence
```

Other option:
```
dotnet ef migrations add <MigrationName> -s .\src\Presentation\CustomersAPI -p .\src\Infra\Persistence -v 
```

where `<MigrationName>` should be replaced by an appropriate name that describes the Migration.

- Finally, you just have to tell EF to update the database with the new migrations

```
dotnet ef database update
```

Other option:
```
dotnet ef database update -s <ProjectName> -p .\src\Infra\Persistence --verbose 
```
