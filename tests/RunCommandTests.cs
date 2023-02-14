using Dapper.BaseRepository.Components;
using Dapper.BaseRepository.Config;
using Moq;

namespace DapperBaseRepo.Tests
{
    public class RunCommandTests : BaseTestComponent
    {
        [Fact]
        public async Task ShouldExecuteOnSqlServer_WhenNoDbTypeIsSpecified()
        {
            //Act
            var resp = await baseRepo.RunCommand(dummySqlString, new { });

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlString, ConnectionStrings.SqlServerConnection, new { }), Times.Once);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldExecuteOnSqlServer_WhenNoDbTypeIsSpecified)}"), Times.Once);
            Assert.Equal(CommandResp.Success, resp);
        }

        [Fact]
        public async Task ShouldCallSybase_WhenDbTypeIsSybase()
        {
            //Act
            var resp = await baseRepo.RunCommand(dummySqlString, new { }, DbType.Sybase);

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlString, ConnectionStrings.SybaseConnection, new { }), Times.Once);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlString, ConnectionStrings.SqlServerConnection, new { }), Times.Never);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldCallSybase_WhenDbTypeIsSybase)}"), Times.Once);
            Assert.Equal(CommandResp.Success, resp);
        }
    }
}