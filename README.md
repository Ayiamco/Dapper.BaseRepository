# Dapper BaseRepository
Dapper.BaseRepository is .netcore class library containing helper methods for running raw sql and stored procedures on [Sqlserver](https://en.wikipedia.org/wiki/Microsoft_SQL_Server) , [Sybase](https://en.wikipedia.org/wiki/Adaptive_Server_Enterprise) and [Oracle](https://en.wikipedia.org/wiki/Oracle_Database) databases. It uses [Dapper ORM](https://github.com/DapperLib/Dapper) for actual data access , helper methods are just wrappers.

## Installation

Run the following from your terminal or commandline.
```
dotnet add package Dapper.BaseRepository
```

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
        options.DefaultSqlServerConnectionString = "";
        options.DefaultOracleConnectionString = "";
        options.DefaultSybaseConnectionString = "";
    });

    var app = builder.Build();

    //TODO: Configure application Middlewares

    app.Run();
}
```
#### Note:
- Helper methods are gotten by inheriting the abstract class [BaseRespository](https://github.com/Ayiamco/Dapper.BaseRepository/blob/master/src/Components/BaseRepository.cs)  
- When connection string is not passed during method call, DefaultConnectionStrings set
at application startup would be used.
- When DbType is not passed during method call, package will default to Microsoft SqlServer.
- Query parameter objects must only contain properties used in the query.
- If you want the package to log your errors and handle them gracefully pass inherit BaseRepository that has logger.
- For Stored procedure output parameters add the corresponding SP attributes to the property.

## Code Samples

#### Raw sql samples
```
using Dapper.BaseRepository.Components;

public class Customer
{
    public string Name {get; set;}

    public string Address {get; set;}
}

public class GetCustomerParam
{
    public int Skip {get;set;}

    public int Take {get;set;}

    public string Name {get;set;}
}


public class AppRawSqlRepository : BaseRepository<AppRepository>
{
    /**
        Run raw sql command without specifying the DbType and ConnectionString :  
        command will be executed on SqlServer and the connection string will be  
        the DefaultSqlServerConnectionString setup during application startup.  
    */ 
    public Task<CommandResp> AddCustomer(Customer customer)
    {
        var sql = $@"INSERT INTO Customers ( Name,Address) VALUES ( @Name,@Address)";
        return RunCommand(sql, customer);
    }

    /**
        Run raw sql query without specifying the DbType and ConnectionString :  
        query will be executed on SqlServer and the connection string will be  
        the DefaultSqlServerConnectionString setup during application startup. 
    */ 
    public Task<IEnumerable<Customer>> GetCustomers(string name)
    {
        var sql = $@"
            Select Name,Address from Customers  
            Where Name like @Name
        ";
        return RunQuery<Customer>(sql, new { Name = name});
    }

    /**
        Run raw sql command on a specified DbType : Command will be executed  
        on specified dbType and the connection string would be the connection setup for  
        that particular dbType at application startup.
    */ 
    public Task<CommandResp> AddCustomer(Customer customer)
    {
        var sql = $@" INSERT INTO Customers ( Name,Address) VALUES ( @Name,@Address)";

        //DbType is an enum of supported databases
        return RunCommand(sql, customer,DbType.Oracle);
    }

    /**
        Run raw sql query on a specified DbType and connection : Command will be executed  
        on specified dbType and the connection string would be the connection setup for  
        that particular dbType at application startup.
    */ 
    public Task<IEnumerable<Customer>> GetCustomers(GetCustomerParam queryParam)
    {
        var connectionString ="A connection string";
        var sql = $@"
            Select Name,Address from Customers  
            Where Name like @Name  
            ORDER BY Price  
            OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
        ";
        return RunQuery<Customer>(sql, queryParam, connectionString, DbType.Sybase);
    }
}
```

#### Stored procedure code samples
- For stored procedure output parameters, add apply their corresponding SP attribute.  
[Here](https://github.com/Ayiamco/Dapper.BaseRepository/blob/master/src/Attributes/SpOutputAttributes.cs) is the file containing the Stored procedure output attributes.

```
using Dapper.BaseRepository.Components;

public class Customer
{
    public string Name {get; set;}

    public string Address {get; set;}
}

public class AddCustomerInputParam : AddCustomerOutputParam
{
    public string Address {get;set;}

    public string Name {get;set;}
}

public class AddCustomerOutputParam
{
    [SpOutputInt]
    public int ResponseCode {get;set;}

    [SpOutputString(250)]
    public int ErrorMessage {get;set;}
}

public class AppStoredProcedureRepository : BaseRepository<AppRepository>
{
    /**
        Run stored proc command without specifying the DbType and ConnectionString :  
        command will be executed on SqlServer and the connection string will be  
        the DefaultSqlServerConnectionString setup during application startup.  
    */ 
    public Task<CommandResp> AddCustomer(AddCustomerInputParam customerParam)
    {
        var storedOProcName = "TestProcedure";
        return RunStoredProcedure<AddCustomerInputParam,AddCustomerOutputParam>(  
            storedProcedure, customerParam);
    }
}
```
