using Dapper.BaseRepository.Components;
using Dapper.BaseRepository.Config;
using Dapper.BaseRepository.interfaces;
using Dapper.Repository.interfaces;
using Moq;

namespace DapperBaseRepo.Tests
{
    public class BaseTestComponent
    {
        protected readonly MockBaseRepository baseRepo;
        protected readonly Mock<IRepositoryLogger<MockBaseRepository>> loggerMock;
        protected readonly Mock<IDbExecutor> dbExecutorMock;
        protected readonly string dummySqlCommand = "Delete * from table";
        public BaseTestComponent()
        {
            ConnectionStrings.SybaseConnection = "SybaseConnectionTestString";
            ConnectionStrings.SqlServerConnection = "SqlServerConnectionTestString";
            ConnectionStrings.OracleConnection = "OracleConnectionTestString";

            loggerMock = new Mock<IRepositoryLogger<MockBaseRepository>>();
            dbExecutorMock = new Mock<IDbExecutor>();
            baseRepo = new MockBaseRepository(loggerMock.Object, dbExecutorMock.Object);
        }
    }

    public class MockBaseRepository : BaseRepository<MockBaseRepository, IRepositoryLogger<MockBaseRepository>>
    {
        public MockBaseRepository(IRepositoryLogger<MockBaseRepository> logger, IDbExecutor dbExecutor) : base(logger, dbExecutor)
        {
        }
    }

    public class TestObject { }
}
