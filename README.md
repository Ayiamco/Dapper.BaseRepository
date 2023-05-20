# Dapper BaseRepository
Dapper.BaseRepository is .netcore class library containing helper methods for running raw sql and stored procedures on [Sqlserver](https://en.wikipedia.org/wiki/Microsoft_SQL_Server) , [Sybase](https://en.wikipedia.org/wiki/Adaptive_Server_Enterprise) and [Oracle](https://en.wikipedia.org/wiki/Oracle_Database) databases. It uses [Dapper ORM](https://github.com/DapperLib/Dapper) for actual data access , helper methods are just wrappers.

## Setup
Default connections for any of the supported databases could be set during startup from the dependency injection container. If the default connection strings are not specified during start up helper methods with connection string as an argument should be used.

```
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBaseRepostiorySetup(options =>
{
    options.DefaultSqlServerConnectionString = "default connection string for sql server database";
    options.DefaultOracleConnectionString = "default connection string for oracle database";
    options.DefaultSybaseConnectionString = "default connection string for sybase database";
});

```

## Code Samples

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
        Run query without specifying the DbType and ConnectionString : Query will be executed\
        on SqlServer and the connection string would be the connection setup from the DI\
        container during application startup.
    */ 
    public async Task<CommandResp> AddCustomer(Customer customer)
    {
        var sql = $@" INSERT INTO Customers ( Name,Address) VALUES ( @Name,@Address)";
        return await RunCommand(sql, customer);
    }

    public Task<CommandResp> AddCustomer(Customer customer)
    {
        var sql = $@" INSERT INTO Customers ( Name,Address) VALUES ( @Name,@Address)";

        //DbType is an enum of supported databases
        return RunCommand(sql, customer,DbType.Oracle);
    }
}
```
