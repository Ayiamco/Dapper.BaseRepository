# Dapper BaseRepository
Dapper.BaseRepository is .netcore class library containing helper methods for running raw sql and stored procedures on [Sqlserver](https://en.wikipedia.org/wiki/Microsoft_SQL_Server) , [Sybase](https://en.wikipedia.org/wiki/Adaptive_Server_Enterprise) and [Oracle](https://en.wikipedia.org/wiki/Oracle_Database) databases. It uses [Dapper ORM](https://github.com/DapperLib/Dapper) for actual data access , helper methods are just wrappers.

## Installation

Run the following from your terminal or commandline.
''''
dotnet add package Dapper.BaseRepository
''''

## Setup
Default connections for any of the supported databases could be set during startup from the dependency injection container. If the default connection strings are not specified during start up helper methods with connection string as an argument should be used.

```

using Dapper.BaseRepository.Config;

public class Program.cs
{
    var builder = WebApplication.CreateBuilder(args);

    //TODO: Add application services to DI container

    //Default connection string setup : you only need to setup databases you'll be using
    builder.Services.AddBaseRepostiorySetup(options =>
    {
        options.DefaultSqlServerConnectionString = "your default connection string for sql server database";
        options.DefaultOracleConnectionString = "your default connection string for oracle database";
        options.DefaultSybaseConnectionString = "your default connection string for sybase database";
    });

    var app = builder.Build();

    //TODO: Configure application Middlewares

    app.Run();
}


```

## Code Samples

- When connection string is not passed during BaseRepository helper method call DefaultConnectionStrings set at  
DI container would be used.
- When DbType is not passed during BaseRepository helper method call will default to Microsoft SqlServer 

```
using Dapper.BaseRepository.Components;

public class Customer
{
    public string Name {get; set;}

    public string Address {get; set;}
}


public class AppRepository : BaseRepository<AppRepository>
{
    /**
        Run command without specifying the DbType and ConnectionString : Query will be executed  
        on SqlServer and the connection string will be the DefaultSqlServerConnectionString setup from the DI  
        container during application startup.
    */ 
    public async Task<CommandResp> AddCustomer(Customer customer)
    {
        var sql = $@" INSERT INTO Customers ( Name,Address) VALUES ( @Name,@Address)";
        return await RunCommand(sql, customer);
    }

    public async Task<CommandResp> AddCustomer(Customer customer)
    {
        var sql = $@"Select Name,Address from Customers ( Name,Address) VALUES ( @Name,@Address)";
        return await RunCommand(sql, customer);
    }

    /**
        Run query on a specified DbType : Query will be executed  
        on specified dbType and the connection string would be the connection setup from the DI  
        container during application startup.
    */ 
    public Task<CommandResp> AddCustomer(Customer customer)
    {
        var sql = $@" INSERT INTO Customers ( Name,Address) VALUES ( @Name,@Address)";

        //DbType is an enum of supported databases
        return RunCommand(sql, customer,DbType.Oracle);
    }
}
```
