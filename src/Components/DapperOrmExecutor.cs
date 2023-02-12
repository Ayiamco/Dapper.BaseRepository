using AdoNetCore.AseClient;
using Dapper.BaseRepository.interfaces;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.BaseRepository.Components
{
    public class DapperOrmExecutor : IDbExecutor
    {
        #region SqlServer 
        //Command Section
        public int ExecuteCommand(string command, string connString, object? param = default)
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

        public int ExecuteCommandProc(string procedureName, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default) return conn.Execute(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        //Query Section
        public IEnumerable<T> Query<T>(string query, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(query, commandType: CommandType.Text);

                return conn.Query<T>(query, param, commandType: CommandType.Text);
            }
        }

        public IEnumerable<T> QueryProc<T>(string procedureName, string connString, object? param = default)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public object QueryScalar(string procedureName, string connString, object? param = default)
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
        public int ExecuteOracleCommand(string command, string connString, object? param = default)
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

        public int ExecuteOracleCommandProc(string procedureName, string connString, object? param = default)
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
        public IEnumerable<T> QueryOracle<T>(string query, string connString, object? param = default)
        {
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(query, commandType: CommandType.Text);

                return conn.Query<T>(query, param, commandType: CommandType.Text);
            }
        }

        public IEnumerable<T> QueryOracleProc<T>(string procedureName, string connString, object? param = default)
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
        public int ExecuteSybaseCommand(string command, string connString, object? param = default)
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

        public int ExecuteSybaseCommandProc(string procedureName, string connString, object? param = default)
        {
            using (AseConnection conn = new AseConnection(connString))
            {
                conn.Open();
                if (param == default) return conn.Execute(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Execute(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        //Query Section
        public IEnumerable<T> QuerySybase<T>(string query, string connString, object? param = default)
        {
            using (AseConnection conn = new AseConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(query, commandType: CommandType.Text);

                return conn.Query<T>(query, param, commandType: CommandType.Text);
            }
        }

        public IEnumerable<T> QuerySybaseProc<T>(string procedureName, string connString, object? param = default)
        {
            using (AseConnection conn = new AseConnection(connString))
            {
                conn.Open();
                if (param == default)
                    return conn.Query<T>(procedureName, commandType: CommandType.StoredProcedure);

                return conn.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
            }
        }

        public object QuerySybaseScalar(string procedureName, string connString, object? param = default)
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
