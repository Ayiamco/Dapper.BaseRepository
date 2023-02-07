namespace Dapper.BaseRepository.Components
{
    using Dapper;
    using Dapper.BaseRepository.Config;
    using Dapper.Repository.interfaces;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Base repository supported db types.
    /// </summary>
    public enum DbType
    {
        Oracle,
        SqlServer,
        Sybase
    }

    public abstract class BaseRepository<TRepo>
    {

        //TODO: Figure out how libraries log
        protected readonly Dictionary<DbType, Func<string, string, object?, int>> CommandsMap;

        protected BaseRepository()
        {
            CommandsMap = new()
            {
                {DbType.Sybase, DapperOrmExecutor.ExecuteSybaseCommand },
                {DbType.Oracle, DapperOrmExecutor.ExecuteOracleCommand },
                {DbType.SqlServer, DapperOrmExecutor.ExecuteCommand},
            };
        }

        /// <summary>
        /// Executes a sql DDL/DML on a sql server database. The connectionString is gotten from
        /// the DefaultConnection property on the <see cref="ConnectionStrings"/> property of the  <see cref="BaseAppConfig"/> configuration object.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
             [CallerMemberName] string callerMemberName = "")
        {
            if (queryParam.GetType() == typeof(string))
                throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

            DapperOrmExecutor.ExecuteCommand(sqlCommand, GetConnectionString(string.Empty, DbType.SqlServer), queryParam);
            //logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
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
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
            string connectionString, [CallerMemberName] string callerMemberName = "")
        {
            if (queryParam is string)
                throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

            connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                GetConnectionString(connectionString, DbType.SqlServer) : connectionString;

            DapperOrmExecutor.ExecuteCommand(sqlCommand, connectionString, queryParam);
            //logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
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
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
            DbType commandType, [CallerMemberName] string callerMemberName = "")
        {
            var map = new Dictionary<DbType, DbType>
                {
                    { DbType.SqlServer, DbType.SqlServer },
                    { DbType.Oracle, DbType.Oracle },
                    { DbType.Sybase, DbType.Sybase },
                };

            if (queryParam.GetType() == typeof(string))
                throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

            CommandsMap[commandType](sqlCommand, GetConnectionString(string.Empty, map[commandType]), queryParam);
            //logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
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
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
            DbType commandType, string connectionString, [CallerMemberName] string callerMemberName = "")
        {
            if (queryParam.GetType() == typeof(string))
                throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

            var map = new Dictionary<DbType, DbType> {
                { DbType.SqlServer, DbType.SqlServer },
                { DbType.Oracle, DbType.Oracle },
                { DbType.Sybase, DbType.Sybase },
            };

            connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                GetConnectionString(connectionString, map[commandType]) : connectionString;

            CommandsMap[commandType](sqlCommand, connectionString, queryParam);
            //logger.LogInformation($"Successfully ran command from function: {callerMemberName}");
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
        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new ArgumentException("sqlQuery cannot be empty");

            var conn = ConnectionStrings.SqlServerConnection;
            if (string.IsNullOrWhiteSpace(conn))
                throw new ArgumentNullException("SqlConnection string is null");

            IEnumerable<TResult> resp = DapperOrmExecutor.Query<TResult>(sqlQuery, conn, new { });
            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp);
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

            IEnumerable<TResult> resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp);
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam, string connectionString,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {

            if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

            IEnumerable<TResult> resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp);
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

            IEnumerable<TResult> resp = null;
            switch (queryDbType)
            {
                case DbType.SqlServer:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, new { });
                    break;
                case DbType.Sybase:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, new { });
                    break;
                case DbType.Oracle:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, new { });
                    break;
            }
            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

            IEnumerable<TResult> resp = null;
            switch (queryDbType)
            {
                case DbType.SqlServer:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
                    break;
                case DbType.Sybase:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
                    break;
                case DbType.Oracle:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
                    break;
            }
            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, string connectionString,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                throw new ArgumentException("sqlQuery cannot be empty.");

            IEnumerable<TResult> resp = null;
            switch (queryDbType)
            {
                case DbType.SqlServer:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, new { });
                    break;
                case DbType.Sybase:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, new { });
                    break;
                case DbType.Oracle:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, new { });
                    break;
            }
            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam, string connectionString,
           DbType queryDbType, [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

            IEnumerable<TResult> resp = null;
            switch (queryDbType)
            {
                case DbType.SqlServer:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                    break;
                case DbType.Sybase:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                    break;
                case DbType.Oracle:
                    resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                    break;
            }
            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp ?? Enumerable.Empty<TResult>());
        }

        protected Task<DynamicParameters> RunCommandProc<TInputParam>(string storedProcedure, TInputParam paramObject,
             [CallerMemberName] string callerMemberName = "")
        {
            var dynamicParameters = CreateDynamicParameter(paramObject);
            DapperOrmExecutor.ExecuteCommandProc(storedProcedure, ConnectionStrings.SqlServerConnection, dynamicParameters);
            return Task.FromResult(dynamicParameters);
        }

        /// <summary>
        /// Creates <see cref="DynamicParameters"/> object or adds additional parameters from the storedProcParams.
        /// </summary>
        /// <param name="storedProcParams">Object containing stored proc input, outpust and return paramaters</param>
        /// <param name="dynamicParameters">Dynamic parameter that the storedProcParams would be added to.</param>
        /// <returns><see cref=" DynamicParameters "/> containing the stored procedure input, output and return parameters.</returns>
        protected DynamicParameters CreateDynamicParameter(object storedProcParams, DynamicParameters? dynamicParameters = default)
        {
            dynamicParameters = dynamicParameters == null ? new DynamicParameters() : dynamicParameters;
            if (storedProcParams == null)
                return dynamicParameters;

            var properties = storedProcParams.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var key = prop.Name;
                Attribute[] attrs = Attribute.GetCustomAttributes(prop);
                if (attrs.Length == 0)
                {
                    var value = prop.GetValue(storedProcParams);
                    dynamicParameters.Add(key, value);
                    continue;
                }
                BaseUtility.AddOutputParam(dynamicParameters, key, attrs);
            }
            return dynamicParameters;
        }

        protected Task<decimal?> RunScalar(string query, [CallerMemberName] string callerMemberName = "")
        {
            //logger.LogInformation($"Sending query from  function: {callerMemberName}...");
            decimal? resp = (decimal)DapperOrmExecutor.QueryScalar(query, ConnectionStrings.SqlServerConnection, new { }); ;

            //logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
            return Task.FromResult(resp);
        }


        #region private methods
        private string GetLogMessage(string name, Exception ex) =>
            @$"Error occured at while running query from function :{name}; Message:{ex.Message}. 
{ex.StackTrace}";

        private static string GetConnectionString(string? conn, DbType sqlType)
        {
            if (!string.IsNullOrWhiteSpace(conn)) return conn;


            //TODO: Finish map
            var connectionStringMap = new Dictionary<DbType, string>()
            {
                {DbType.SqlServer,ConnectionStrings.SqlServerConnection },
                {DbType.Sybase,ConnectionStrings.SybaseConnection },
                {DbType.Oracle,ConnectionStrings.OracleConnection },
            };

            var dbTypeNameMap = new Dictionary<DbType, string>()
            {
                {DbType.SqlServer,"SqlServer database" },
                {DbType.Sybase,"Sybase database" },
                {DbType.Oracle,"Oracle database" },
            };
            var connectionString = connectionStringMap[sqlType];
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException($"Connection string for {dbTypeNameMap[sqlType]} in BaseAppConfig is not setup.Please pass in connectionString or setup BaseAppConfig");
            return connectionString;
        }


        #endregion
    }

    public abstract class BaseRepository<TRepo, TLogger> where TLogger : IRepositoryLogger<TRepo> where TRepo : class
    {
        protected readonly IRepositoryLogger<TRepo> logger;
        protected readonly Dictionary<DbType, Func<string, string, object?, int>> CommandsMap;

        protected BaseRepository(IRepositoryLogger<TRepo> logger)
        {
            this.logger = logger;
            CommandsMap = new()
            {
                {DbType.Sybase, DapperOrmExecutor.ExecuteSybaseCommand },
                {DbType.Oracle, DapperOrmExecutor.ExecuteOracleCommand },
                {DbType.SqlServer, DapperOrmExecutor.ExecuteCommand},
            };
        }

        /// <summary>
        /// Executes a sql DDL/DML on a sql server database. The connectionString is gotten from
        /// the DefaultConnection property on the <see cref="ConnectionStrings"/> property of the  <see cref="BaseAppConfig"/> configuration object.
        /// </summary>
        /// <param name="sqlCommand">sql statement</param>
        /// <param name="queryParam">queryParam parameters for the sql command.</param>
        /// <returns></returns>
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
             [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (queryParam.GetType() == typeof(string))
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

                DapperOrmExecutor.ExecuteCommand(sqlCommand, BaseUtility.GetConnectionString(string.Empty, DbType.SqlServer), queryParam);
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
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
            string connectionString, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (queryParam is string)
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

                connectionString = string.IsNullOrWhiteSpace(connectionString) ?
                    BaseUtility.GetConnectionString(connectionString, DbType.SqlServer) : connectionString;

                DapperOrmExecutor.ExecuteCommand(sqlCommand, connectionString, queryParam);
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
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
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
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

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
        protected Task<CommandResp> RunCommand(string sqlCommand, object queryParam,
            DbType commandType, string connectionString, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                if (queryParam.GetType() == typeof(string))
                    throw new ArgumentException("queryParam must be an object containing the sqlCommand queryParam parameters");

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
        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery))
                    throw new ArgumentException("sqlQuery cannot be empty");

                var conn = ConnectionStrings.SqlServerConnection;
                if (string.IsNullOrWhiteSpace(conn))
                    throw new ArgumentNullException("SqlConnection string is null");

                IEnumerable<TResult> resp = DapperOrmExecutor.Query<TResult>(sqlQuery, conn, new { });
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                    throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

                IEnumerable<TResult> resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam, string connectionString,
            [CallerMemberName] string callerMemberName = "") where TResult : class
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sqlQuery) || queryParam.GetType() == typeof(string))
                    throw new ArgumentException("sqlQuery cannot be empty and queryParam must be sqlQuery parameter object");

                IEnumerable<TResult> resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                logger.LogInformation($"Successfully ran query from function: {callerMemberName}");
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(BaseUtility.GetLogMessage(callerMemberName, ex));
                return Task.FromResult(Enumerable.Empty<TResult>());
            }
        }

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery,
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
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, new { });
                        break;
                    case DbType.Sybase:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, new { });
                        break;
                    case DbType.Oracle:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, new { });
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

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam,
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
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
                        break;
                    case DbType.Sybase:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
                        break;
                    case DbType.Oracle:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, ConnectionStrings.SqlServerConnection, queryParam);
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

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, string connectionString,
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
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, new { });
                        break;
                    case DbType.Sybase:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, new { });
                        break;
                    case DbType.Oracle:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, new { });
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

        protected Task<IEnumerable<TResult>> RunQuery<TResult>(string sqlQuery, object queryParam, string connectionString,
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
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                        break;
                    case DbType.Sybase:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
                        break;
                    case DbType.Oracle:
                        resp = DapperOrmExecutor.Query<TResult>(sqlQuery, connectionString, queryParam);
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
        protected Task<TResult> RunStoredProcedure<TParam, TResult>(string storedProcedureName, TParam paramObject,
             [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                var dynamicParameters = new DynamicParameters();

                if (paramObject != null)
                    dynamicParameters = BaseUtility.CreateDynamicParameter(paramObject);

                if (string.IsNullOrWhiteSpace(ConnectionStrings.SqlServerConnection))
                    throw new NullReferenceException("ConnectionStrings.SqlServerConnection is null, please set a default value for sqlserver connection.");

                DapperOrmExecutor.ExecuteCommandProc(storedProcedureName, ConnectionStrings.SqlServerConnection, dynamicParameters);
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
        protected Task<TResult> RunStoredProcedure<TParam, TResult>(string storedProcedureName, TParam paramObject, DbType dbType,
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
                        DapperOrmExecutor.ExecuteCommandProc(storedProcedureName, connectionString, dynamicParameters);
                        break;
                    case DbType.Sybase:
                        DapperOrmExecutor.ExecuteSybaseCommandProc(storedProcedureName, connectionString, dynamicParameters);
                        break;
                    case DbType.Oracle:
                        DapperOrmExecutor.ExecuteOracleCommandProc(storedProcedureName, connectionString, dynamicParameters);
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

        protected Task<decimal?> RunScalar(string query, [CallerMemberName] string callerMemberName = "")
        {
            try
            {
                logger.LogInformation($"Sending query from  function: {callerMemberName}...");
                decimal? resp = (decimal)DapperOrmExecutor.QueryScalar(query, ConnectionStrings.SqlServerConnection, new { }); ;

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
}