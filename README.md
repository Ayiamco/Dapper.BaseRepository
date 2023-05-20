# Dapper BaseRepository
Dapper.BaseRepository is .netcore class library containing helper methods for running raw sql and stored procedures on [Sqlserver](https://en.wikipedia.org/wiki/Microsoft_SQL_Server) , [Sybase](https://en.wikipedia.org/wiki/Adaptive_Server_Enterprise) and [Oracle](https://en.wikipedia.org/wiki/Oracle_Database) databases. It uses [Dapper ORM](https://github.com/DapperLib/Dapper) for actual data access , helper methods are just wrappers.

<br>

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





The BaseRepository class is an abstract class 
```
public class QueryParam {

}
```
