namespace Dapper.Repository.interfaces
{
    public interface IRepositoryLogger<TRepo>
    {
        void LogInformation(string message);

        void LogInformation(string message, Exception ex);

        void LogError(string message);

        void LogError(string message, Exception ex);
    }
}
