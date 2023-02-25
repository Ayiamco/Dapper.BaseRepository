using Dapper.BaseRepository.Config;
using Dapper.BaseRepository.interfaces;
using Dapper.Repository.interfaces;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Dapper.BaseRepository.Components
{
    /// <summary>
    /// <see cref="BaseRepository"/> supported databases.
    /// </summary>
    public enum DbType
    {
        Oracle,
        SqlServer,
        Sybase
    }

    public abstract class BaseRepository<TRepo, TLogger> where TLogger : IRepositoryLogger<TRepo> where TRepo : class
    {
        private readonly IRepositoryLogger<TRepo> logger;
        private readonly IDbExecutor dbExecutor;
        private readonly Dictionary<DbType, Func<string, string, object?, int>> CommandsMap;

        internal BaseRepository(IRepositoryLogger<TRepo> logger, IDbExecutor dbExecutor)
        {
            this.logger = logger;
            this.dbExecutor = dbExecutor;
            CommandsMap = new()
            {
                {DbType.Sybase, dbExecutor.ExecuteSybaseCommand },
                {DbType.Oracle, dbExecutor.ExecuteOracleCommand },
                {DbType.SqlServer, dbExecutor.ExecuteCommand},
            };
        }

        public BaseRepository(IRepositoryLogger<TRepo> logger)
        {
            this.logger = logger;
            this.dbExecutor = new DapperOrmExecutor();
            CommandsMap = new()
            {
                {DbType.Sybase, dbExecutor.ExecuteSybaseCommand },
                {DbType.Oracle, dbExecutor.ExecuteOracleCommand },
                {DbType.SqlServer, dbExecutor.ExecuteCommand},
            };
        }

        /// <summary>
        /// Executes a sql DDL/DML on a sql server database. The connectionString is gotten from
        /// the DefaultConnection property on the <see cref="ConnectionStrings"/> property of the  <see cref="BaseAppConfig"/> configuration object.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
             [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (queryParam.GetType() == typeof(string) || queryParam.GetType().IsValueType)
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

                dbExecutor.ExecuteCommand(sqlCommand, BaseUtility.GetConnectionString(string.Empty, DbType.SqlServer), queryParam);
                logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
                return Task.FromResult(CommandResp.Success);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                    return Task.FromResult(CommandResp.UniqueKeyViolation);

                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(CommandResp.Failure);
            }
        }

        /// <summary>
        /// Executes a sql DDL/DML on a sql server database. If the connectionString is not specified 
        /// the DefaultConnection property on the BaseAppConfig Connectionstring is used.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="connectionString">db connection string.</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand,
            string connectionString, object queryParam, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (queryParam is string)
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

                connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                    BaseUtility.GetConnectionString(connectionString, DbType.SqlServer) : connectionString;

                dbExecutor.ExecuteCommand(sqlCommand, connectionString, queryParam);
                logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
                return Task.FromResult(CommandResp.Success);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                    return Task.FromResult(CommandResp.UniqueKeyViolation);

                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(CommandResp.Failure);
            }
        }

        /// <summary>
        /// Executes a sql DDL/DML on the specified database type. The connectionString is gotten from
        /// the corresponding BaseAppConfig Connectionstring property.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="connectionString">db connection string.</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
            DbType commandType, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                var map = new Dictionary<DbType, DbType>
                {
                    { DbType.SqlServer, DbType.SqlServer },
                    { DbType.Oracle, DbType.Oracle },
                    { DbType.Sybase, DbType.Sybase },
                };

                if (queryParam.GetType() == typeof(string))
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

                CommandsMap[commandType](sqlCommand, BaseUtility.GetConnectionString(string.Empty, map[commandType]), queryParam);
                logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
                return Task.FromResult(CommandResp.Success);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                    return Task.FromResult(CommandResp.UniqueKeyViolation);

                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(CommandResp.Failure);
            }
        }

        /// <summary>
        /// Executes a sql DDL/DML on the specified database type. If the connectionString is not specified 
        /// the corresponding BaseAppConfig Connectionstring property is used.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="connectionString">db connection string.</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand, object queryParam, string connectionString,
            DbType commandType, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (queryParam.GetType() == typeof(string))
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

                var map = new Dictionary<DbType, DbType>
                {
                    { DbType.SqlServer, DbType.SqlServer },
                    { DbType.Oracle, DbType.Oracle },
                    { DbType.Sybase, DbType.Sybase },
                };

                connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                    BaseUtility.GetConnectionString(connectionString, map[commandType]) : connectionString;
                CommandsMap[commandType](sqlCommand, connectionString, queryParam);
                logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
                return Task.FromResult(CommandResp.Success);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
                    return Task.FromResult(CommandResp.UniqueKeyViolation);

                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(CommandResp.Failure);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery))
                    throw new ArgumentException("sqlQuery cannot be empty");

                var conn = ConnectionStrings.SqlServerConnection;
                if (string.IsNullOrWhiteSpace(conn))
                    throw new ArgumentNullException("SqlConnection string is null");

                IEnumerable<TResult> resp = dbExecutor.Query<TResult>(sqlQuery, conn, new { });
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                    throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

                if (string.IsNullOrWhiteSpace(ConnectionStrings.SqlServerConnection))
                    throw new ArgumentException("DefaultSqlServerConnectionString was not set, please configure using ConnectionStringOptions or pass in connection string.");

                IEnumerable<TResult> resp = dbExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection!, queryParam);
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam, string connectionString,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                    throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

                IEnumerable<TResult> resp = dbExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery))
                    throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

                IEnumerable<TResult> resp = null;
                switch (queryDbType)
                {
                    case DbType.SqlServer:
                        resp = dbExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, new { });
                        break;
                    case DbType.Sybase:
                        resp = dbExecutor.QuerySybase<TResult>(sqlQuery, ConnectionStrings.SybaseConnection, new { });
                        break;
                    case DbType.Oracle:
                        resp = dbExecutor.QueryOracle<TResult>(sqlQuery, ConnectionStrings.OracleConnection, new { });
                        break;
                }
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                    throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

                IEnumerable<TResult> resp = null;
                switch (queryDbType)
                {
                    case DbType.SqlServer:
                        resp = dbExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
                        break;
                    case DbType.Sybase:
                        resp = dbExecutor.QuerySybase<TResult>(sqlQuery, ConnectionStrings.SybaseConnection, queryParam);
                        break;
                    case DbType.Oracle:
                        resp = dbExecutor.QueryOracle<TResult>(sqlQuery, ConnectionStrings.OracleConnection, queryParam);
                        break;
                }
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, string connectionString,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery))
                    throw new ArgumentException("sqlQuery cannot be empty.");

                IEnumerable<TResult> resp = null;
                switch (queryDbType)
                {
                    case DbType.SqlServer:
                        resp = dbExecutor.Query<TResult>(sqlQuery, connectionString, new { });
                        break;
                    case DbType.Sybase:
                        resp = dbExecutor.QuerySybase<TResult>(sqlQuery, connectionString, new { });
                        break;
                    case DbType.Oracle:
                        resp = dbExecutor.QueryOracle<TResult>(sqlQuery, connectionString, new { });
                        break;
                }
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam, string connectionString,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                    throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

                IEnumerable<TResult> resp = null;
                switch (queryDbType)
                {
                    case DbType.SqlServer:
                        resp = dbExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                        break;
                    case DbType.Sybase:
                        resp = dbExecutor.QuerySybase<TResult>(sqlQuery, connectionString, queryParam);
                        break;
                    case DbType.Oracle:
                        resp = dbExecutor.QueryOracle<TResult>(sqlQuery, connectionString, queryParam);
                        break;
                }
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        /// <summary>
        /// Runs a stored procedure on a sql server database.
        /// </summary>
        /// <typeparam name="TParam">The type of the object containing the stored procedure parameters including input, output and return parameters.</typeparam>
        /// <typeparam name="TResult">The type of the object containing the result returned from the stored procedure including output and return parameters. </typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure</param>
        /// <param name="paramObject">An object containing the input , output and return parameters of the stored procedure.</param>
        /// <param name="callerMemberName">The name of the calling function. (used for logging)</param>
        /// <returns></returns>
        public Task<TResult> RunStoredProcedure<TParam, TResult>(string storedProcedureName, TParam paramObject,
             [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                var dynamicParameters = new DynamicParameters();

                if (paramObject != null)
                    dynamicParameters = BaseUtility.CreateDynamicParameter(paramObject);

                if (string.IsNullOrWhiteSpace(ConnectionStrings.SqlServerConnection))
                    throw new NullReferenceException("ConnectionStrings.SqlServerConnection is null, please set a default value for sqlserver connection.");

                dbExecutor.ExecuteCommandProc(storedProcedureName, ConnectionStrings.SqlServerConnection, dynamicParameters);
                return Task.FromResult(BaseUtility.GetStoredProcedureResult<TResult>(dynamicParameters));
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage($"{callerMemberName}/{storedProcedureName}", ex));
                return Task.FromResult(Activator.CreateInstance<TResult>()); ;
            }
        }

        /// <summary>
        /// Runs a stored procedure on a sql server database.
        /// </summary>
        /// <typeparam name="TParam">The type of the object containing the stored procedure parameters including input, output and return parameters.</typeparam>
        /// <typeparam name="TResult">The type of the object containing the result returned from the stored procedure including output and return parameters. </typeparam>
        /// <param name="storedProcedureName">The name of the stored procedure</param>
        /// <param name="paramObject">An object containing input , output and return parameters of the stored procedure.</param>
        /// <param name="dbType">The database type.</param>
        /// <param name="callerMemberName">The name of the calling function. (used for logging)</param>
        /// <returns></returns>
        public Task<TResult> RunStoredProcedure<TParam, TResult>(string storedProcedureName, TParam paramObject, DbType dbType,
             [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                var dynamicParameters = new DynamicParameters();

                if (paramObject != null)
                    dynamicParameters = BaseUtility.CreateDynamicParameter(paramObject);

                var connectionString = BaseUtility.GetConnectionString(string.Empty, dbType);

                switch (dbType)
                {
                    case DbType.SqlServer:
                        dbExecutor.ExecuteCommandProc(storedProcedureName, connectionString, dynamicParameters);
                        break;
                    case DbType.Sybase:
                        dbExecutor.ExecuteSybaseCommandProc(storedProcedureName, connectionString, dynamicParameters);
                        break;
                    case DbType.Oracle:
                        dbExecutor.ExecuteOracleCommandProc(storedProcedureName, connectionString, dynamicParameters);
                        break;
                }
                return Task.FromResult(BaseUtility.GetStoredProcedureResult<TResult>(dynamicParameters));
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage($"{callerMemberName}/{storedProcedureName}", ex));
                return Task.FromResult(Activator.CreateInstance<TResult>());
            }
        }

        public Task<decimal?> RunScalar(string query, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                logger.LogInformation($"Sending query from  function: {callerMemberName}...");
                decimal? resp = (decimal)dbExecutor.QueryScalar(query, ConnectionStrings.SqlServerConnection, new { }); ;

                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));

                //TODO: Find a better return type for failure
                return default;
            }
        }
    }


    public abstract class BaseRepository<TRepo> where TRepo : class
    {
        private readonly IDbExecutor dbExecutor;
        private readonly Dictionary<DbType, Func<string, string, object?, int>> CommandsMap;

        internal BaseRepository(IDbExecutor dbExecutor)
        {
            this.dbExecutor = dbExecutor;
            CommandsMap = new()
            {
                {DbType.Sybase, dbExecutor.ExecuteSybaseCommand },
                {DbType.Oracle, dbExecutor.ExecuteOracleCommand },
                {DbType.SqlServer, dbExecutor.ExecuteCommand},
            };
        }

        public BaseRepository()
        {
            this.dbExecutor = new DapperOrmExecutor();
            CommandsMap = new()
            {
                {DbType.Sybase, dbExecutor.ExecuteSybaseCommand },
                {DbType.Oracle, dbExecutor.ExecuteOracleCommand },
                {DbType.SqlServer, dbExecutor.ExecuteCommand},
            };
        }

        /// <summary>
        /// Executes a sql DDL/DML on a sql server database. The connectionString is gotten from
        /// the DefaultConnection property on the <see cref="ConnectionStrings"/> property of the  <see cref="BaseAppConfig"/> configuration object.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
             [CallerMemberName] string callerMemberName = "")
        {
            if (queryParam.GetType() == typeof(string) || queryParam.GetType().IsValueType)
                throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

            dbExecutor.ExecuteCommand(sqlCommand, BaseUtility.GetConnectionString(string.Empty, DbType.SqlServer), queryParam);
            Debug.WriteLine($"Successfully ran command from function: {callerMemberName}");
            return Task.FromResult(CommandResp.Success);
        }

        /// <summary>
        /// Executes a sql DDL/DML on a sql server database. If the connectionString is not specified 
        /// the DefaultConnection property on the BaseAppConfig Connectionstring is used.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="connectionString">db connection string.</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand,
            string connectionString, object queryParam, [CallerMemberName] string callerMemberName = "")
        {
            if (queryParam is string)
                throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

            connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                BaseUtility.GetConnectionString(connectionString, DbType.SqlServer) : connectionString;

            dbExecutor.ExecuteCommand(sqlCommand, connectionString, queryParam);
            Debug.WriteLine($"Successfully ran command from function: {callerMemberName}");
            return Task.FromResult(CommandResp.Success);
        }

        /// <summary>
        /// Executes a sql DDL/DML on the specified database type. The connectionString is gotten from
        /// the corresponding BaseAppConfig Connectionstring property.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="connectionString">db connection string.</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
            DbType commandType, [CallerMemberName] string callerMemberName = "")
        {
            var map = new Dictionary<DbType, DbType>
                {
                    { DbType.SqlServer, DbType.SqlServer },
                    { DbType.Oracle, DbType.Oracle },
                    { DbType.Sybase, DbType.Sybase },
                };

            if (queryParam.GetType() == typeof(string))
                throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

            CommandsMap[commandType](sqlCommand, BaseUtility.GetConnectionString(string.Empty, map[commandType]), queryParam);
            Debug.WriteLine($"Successfully ran command from function: {callerMemberName}");
            return Task.FromResult(CommandResp.Success);
        }

        /// <summary>
        /// Executes a sql DDL/DML on the specified database type. If the connectionString is not specified 
        /// the corresponding BaseAppConfig Connectionstring property is used.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="connectionString">db connection string.</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        public Task<CommandResp> RunCommand(string sqlCommand, object queryParam, string connectionString,
            DbType commandType, [CallerMemberName] string callerMemberName = "")
        {
            if (queryParam.GetType() == typeof(string))
                throw new ArgumentException("queryParam must be an object containing the sqlCommand parameters");

            var map = new Dictionary<DbType, DbType>
                {
                    { DbType.SqlServer, DbType.SqlServer },
                    { DbType.Oracle, DbType.Oracle },
                    { DbType.Sybase, DbType.Sybase },
                };

            connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                BaseUtility.GetConnectionString(connectionString, map[commandType]) : connectionString;
            CommandsMap[commandType](sqlCommand, connectionString, queryParam);
            Debug.WriteLine($"Successfully ran command from function: {callerMemberName}");
            return Task.FromResult(CommandResp.Success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="callerMemberName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new ArgumentException("sqlQuery cannot be empty");

            var conn = ConnectionStrings.SqlServerConnection;
            if (string.IsNullOrWhiteSpace(conn))
                throw new ArgumentNullException("SqlConnection string is null");

            IEnumerable<TResult> resp = dbExecutor.Query<TResult>(sqlQuery, conn, new { });
            Debug.WriteLine($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp);
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

            if (string.IsNullOrWhiteSpace(ConnectionStrings.SqlServerConnection))
                throw new ArgumentException("DefaultSqlServerConnectionString was not set, please configure using ConnectionStringOptions or pass in connection string.");

            IEnumerable<TResult> resp = dbExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection!, queryParam);
            Debug.WriteLine($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp);
        }

        public Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam, string connectionString,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

            IEnumerable<TResult> resp = dbExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
            Debug.WriteLine($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp);
        }

    }
}