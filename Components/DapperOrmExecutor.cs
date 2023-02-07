using AdoNetCore.AseClient;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.BaseRepository.Components
{
    internal class DapperOrmExecutor
    {
        #region SqlServer 
        //Command Section
        internal static int ExecuteCommand(string command, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Execute(command, commandType: CommandType.Text);
                else
                    return conn.Execute(command, param, commandType: CommandType.Text);
            }
        }

        internal static int ExecuteCommandProc(string procedureName, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default) return conn.Execute(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        //Query Section
        internal static IEnumerable<T> Query<T>(string query, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(query, commandType: CommandType.Text);

                return conn.Query<T>(query, param, commandType: CommandType.Text);
            }
        }

        internal static IEnumerable<T> QueryProc<T>(string procedureName, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        internal static object QueryScalar(string procedureName, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.ExecuteScalar(procedureName, commandType: CommandType.Text);

                return conn.ExecuteScalar(procedureName, param, commandType: CommandType.Text);
            }
        }
        #endregion


        #region Oracle 
        //Command Section
        internal static int ExecuteOracleCommand(string command, string connString, object? param = default)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Execute(command, commandType: CommandType.Text);
                else
                    return conn.Execute(command, param, commandType: CommandType.Text);
            }
        }

        internal static int ExecuteOracleCommandProc(string procedureName, string connString, object? param = default)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Execute(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        //Query Section
        internal static IEnumerable<T> QueryOracle<T>(string query, string connString, object? param = default)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(query, commandType: CommandType.Text);

                return conn.Query<T>(query, param, commandType: CommandType.Text);
            }
        }

        internal static IEnumerable<T> QueryOracleProc<T>(string procedureName, string connString, object? param = default)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }
        #endregion

        #region Sybase
        //Command Section
        internal static int ExecuteSybaseCommand(string command, string connString, object? param = default)
        {
            using (AseConnection conn = new AseConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Execute(command, commandType: CommandType.Text);
                else
                    return conn.Execute(command, param, commandType: CommandType.Text);
            }
        }

        internal static int ExecuteSybaseCommandProc(string procedureName, string connString, object? param = default)
        {
            using (AseConnection conn = new AseConnection(connString))
            {
                conn.Open();
                if (param == default) return conn.Execute(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        //Query Section
        internal static IEnumerable<T> QuerySybase<T>(string query, string connString, object? param = default)
        {
            using (AseConnection conn = new AseConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(query, commandType: CommandType.Text);

                return conn.Query<T>(query, param, commandType: CommandType.Text);
            }
        }

        internal static IEnumerable<T> QuerySybaseProc<T>(string procedureName, string connString, object? param = default)
        {
            using (AseConnection conn = new AseConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        internal static object QuerySybaseScalar(string procedureName, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.ExecuteScalar(procedureName, commandType: CommandType.Text);

                return conn.ExecuteScalar(procedureName, param, commandType: CommandType.Text);
            }
        }
        #endregion
    }
}
