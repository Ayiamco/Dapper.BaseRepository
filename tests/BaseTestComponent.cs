using Dapper.BaseRepository.Components;
using Dapper.BaseRepository.Config;
using Dapper.BaseRepository.interfaces;
using Dapper.Repository.interfaces;
using Moq;

namespace DapperBaseRepo.Tests
{
    public class BaseTestComponent : IDisposable
    {
        protected readonly Repository repo;
        protected readonly Mock<IRepositoryLogger<Repository>> loggerMock;
        protected readonly Mock<IDbExecutor> dbExecutorMock;
        protected readonly string dummySqlCommand = "Delete * from table";
        public BaseTestComponent()
        {
            ConnectionStrings.SybaseConnection = "SybaseConnectionTestString";
            ConnectionStrings.SqlServerConnection = "SqlServerConnectionTestString";
            ConnectionStrings.OracleConnection = "OracleConnectionTestString";

            loggerMock = new Mock<IRepositoryLogger<Repository>>();
            dbExecutorMock = new Mock<IDbExecutor>();
            repo = new Repository(loggerMock.Object, dbExecutorMock.Object);
        }

        public void Dispose()
        {
            loggerMock.Reset();
            dbExecutorMock.Reset();
        }
    }

    public class Repository : BaseRepository<Repository, IRepositoryLogger<Repository>>
    {
        public Repository(IRepositoryLogger<Repository> logger, IDbExecutor dbExecutor) : base(logger, dbExecutor)
        {
        }
    }

    public class TestObject { }
}
