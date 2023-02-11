namespace Dapper.BaseRepository.interfaces
{
    public interface IDbExecutor
    {
        int ExecuteCommand(string command, string connString, object? param = null);
        int ExecuteCommandProc(string procedureName, string connString, object? param = null);
        int ExecuteOracleCommand(string command, string connString, object? param = null);
        int ExecuteOracleCommandProc(string procedureName, string connString, object? param = null);
        int ExecuteSybaseCommand(string command, string connString, object? param = null);
        int ExecuteSybaseCommandProc(string procedureName, string connString, object? param = null);
        IEnumerable<T> Query<T>(string query, string connString, object? param = null);
        IEnumerable<T> QueryOracle<T>(string query, string connString, object? param = null);
        IEnumerable<T> QueryOracleProc<T>(string procedureName, string connString, object? param = null);
        IEnumerable<T> QueryProc<T>(string procedureName, string connString, object? param = null);
        object QueryScalar(string procedureName, string connString, object? param = null);
        IEnumerable<T> QuerySybase<T>(string query, string connString, object? param = null);
        IEnumerable<T> QuerySybaseProc<T>(string procedureName, string connString, object? param = null);
        object QuerySybaseScalar(string procedureName, string connString, object? param = null);
    }
}