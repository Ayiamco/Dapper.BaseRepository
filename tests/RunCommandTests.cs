using Dapper.BaseRepository.Config;
using Moq;

namespace DapperBaseRepo.Tests
{
    public class RunCommandTests : BaseTestComponent
    {
        [Fact]
        public async Task ShouldExecuteOnSqlServer_WhenCalledWithoutSpecifyingDbType()
        {
            //Arrange

            //Act
            var resp = await baseRepo.RunCommand(dummySqlString, new { });

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlString, ConnectionStrings.SqlServerConnection, new { }), Times.Once);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldExecuteOnSqlServer_WhenCalledWithoutSpecifyingDbType)}"), Times.Once);
            Assert.Equal(CommandResp.Success, resp);
        }
    }
}