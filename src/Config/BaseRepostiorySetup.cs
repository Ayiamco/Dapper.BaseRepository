using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DapperBaseRepo.Tests")]
namespace Dapper.BaseRepository.Config
{
    internal static class ConnectionStrings
    {
        public static string? SqlServerConnection { get; set; }

        public static string? SybaseConnection { get; set; }

        public static string? OracleConnection { get; set; }

        public static bool ThrowErrors { get; set; }
    }

    public class ConnectionStringOptions
    {
        /// <summary>
        /// The Default connection string for Sql server connections.
        /// </summary>
        public string? DefaultSqlServerConnectionString { get; set; }

        /// <summary>
        /// The Default connection string for Sybase connectionf.
        /// </summary>
        public string? DefaultSybaseConnectionString { get; set; }

        /// <summary>
        /// The Default connection string for Oracle connections.
        /// </summary>
        public string? DefaultsOracleConnectionString { get; set; }

        /// <summary>
        /// Flag to throw errors or use logger to log errors. Default is <see cref="false"/>
        /// </summary>
        public bool ThrowErrors { get; set; }
    }

    public static class BaseRepostiorySetup
    {
        public static IServiceCollection AddBaseRepostiorySetup(this IServiceCollection services, Action<ConnectionStringOptions> options)
        {

            var connectionStringOptions = new ConnectionStringOptions();
            options(connectionStringOptions);

            ConnectionStrings.SqlServerConnection = connectionStringOptions.DefaultSqlServerConnectionString;
            ConnectionStrings.SybaseConnection = connectionStringOptions.DefaultSybaseConnectionString;
            ConnectionStrings.OracleConnection = connectionStringOptions.DefaultsOracleConnectionString;
            ConnectionStrings.ThrowErrors = connectionStringOptions.ThrowErrors;
            return services;
        }
    }

    public enum CommandResp
    {
        Success, Failure, UniqueKeyViolation
    }
}
