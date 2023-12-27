# Configurations

Within each Presentation API project you can find the main configuration file `appsettings.json`. Where each setting
field can be replaced using environment settings file i.e. `appsettings.Development.json` for local development.

The `Startup` class inside the folder is responsible for loading all the configurations described below.

## Database

As mentioned in the [Development Environment](development-environment.md) section, the database provider for this
solution is SQL Server.

You can set to `true` the setting `LogSQLData` to see the SQL sentences in the console and log the actual data passed to
the sentenced respectively.

Also, if you are using Visual Studio you may have to set `LogToOutput` to `true` in order to see the logs in the
console.

```json
{
  "DatabaseSettings": {
    "ConnectionString": "Server=localhost;Database=master;uid=username;pwd=password;Trusted_Connection=True",
    "Logging": {
      "LogSQLData": false,
      "LogToOutput": false
    }
  }
}
```

## Cache

By default, the application uses in-memory cache. To enable Distributed caching for all the APIs
set `UseDistributedCache` to `true`

You can also use use Redis, just set the `PreferRedis` to `true` and give a valid redis url!

```json
{
  "CacheSettings": {
    "UseDistributedCache": true,
    "PreferRedis": true,
    "RedisUrl": "localhost:6379"
  }
}
```

## Exceptions

By default when an exception occurs, for development or test environments you may want to see the exception's stack
trace in the response, you can set `AddStackTrace` to `true` to do that.

```json
{
  "ExceptionsSettings": {
    "AddStackTrace": true
  }
}
```

## CORS

Depends on the client consuming the APIs like an SPA frontend or Mobile App.

```json
{
  "CorsSettings": {
    "Origins": [
      "http://localhost:4200"
    ],
    "Methods": [
      "*"
    ],
    "Headers": [
      "*"
    ]
  }
}
```

## JWT

Here you can set the settings related to the access and refresh JWT. By default, the access token expires 15 after being
generated and the refresh token expires 7 days after being generated.

For development or test environments when you don't have the same base domain for the JWT cookies you can
set `SameSiteStrictCookies` to `false`. **DO NOT** do this in a production environment.

```json
{
  "JwtSettings": {
    "AccessTokenSecret": "SOMERANDMS3CR3T!1!P3AKRAPI!1!",
    "RefreshTokenSecret": "SOMEOTHERRANDMS3CR3T!1!P3AKRAPI!1!",
    "AccessTokenExpirationInMinutes": 15,
    "RefreshTokenExpirationInDays": 7,
    "SameSiteStrictCookies": false
  }
}
```

## Swagger

To enable API documentation with [Swagger](https://swagger.io/) simply set `Enable` to `true`

```json
{
  "SwaggerSettings": {
    "Enable": true,
    "Title": "ApiTemplate - .NET 7.0 UsersAPI"
  }
}
```

## Hangfire

Settings for [Hangfire](https://www.hangfire.io/) regarding background jobs for queuing and scheduling jobs.

```json
{
  "HangfireSettings": {
    "Dashboard": {
      "Route": "/hangfire",
      "Options": {
        "StatsPollingInterval": 5000,
        "DashboardTitle": "Hangfire Dashboard"
      }
    },
    "Server": {
      "HeartbeatInterval": "00:00:30",
      "SchedulePollingInterval": "00:00:15",
      "ServerCheckInterval": "00:05:00",
      "WorkerCount": 5
    },
    "Storage": {
      "ConnectionString": "Server=localhost;Database=master;uid=username;pwd=password;Trusted_Connection=True;MultipleActiveResultSets=True",
      "Options": {
        "QueuePollInterval": "00:00:00",
        "UseRecommendedIsolationLevel": true,
        "DisableGlobalLocks": true
      }
    }
  }
}
```

## Mailing

We are using [Mailgun](https://www.mailgun.com/) to send emails.

```json
{
  "MailSettings": {
    "BaseUrl": "https://api.mailgun.net/v3",
    "Domain": "",
    "Apikey": "",
    "TestMode": false,
    "FromAddress": "",
    "FromName": ""
  }
}
```
