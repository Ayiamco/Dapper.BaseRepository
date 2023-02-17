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
            var resp = await baseRepo.RunCommand(dummySqlCommand, new { });

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Once);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldExecuteOnSqlServer_WhenNoDbTypeIsSpecified)}"), Times.Once);
            Assert.Equal(CommandResp.Success, resp);
        }

        [Fact]
        public async Task ShouldCallSybase_WhenDbTypeIsSybase()
        {
            //Act
            var resp = await baseRepo.RunCommand(dummySqlCommand, new { }, DbType.Sybase);

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SybaseConnection, new { }), Times.Once);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Never);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldCallSybase_WhenDbTypeIsSybase)}"), Times.Once);
            Assert.Equal(CommandResp.Success, resp);
        }

        [Fact]
        public async Task ShouldCallOracle_WhenDbTypeIsOracle()
        {
            //Act
            var resp = await baseRepo.RunCommand(dummySqlCommand, new { }, DbType.Oracle);

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SybaseConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteOracleCommand(dummySqlCommand, ConnectionStrings.OracleConnection, new { }), Times.Once);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldCallOracle_WhenDbTypeIsOracle)}"), Times.Once);
            Assert.Equal(CommandResp.Success, resp);
        }

        [Fact]
        public async Task ShouldNotUseDefaultConnectionString_WhenConnectionIsProvided()
        {
            //Arrange
            var connString1 = "newConnectionString";
            var connString2 = "newConnectionString";
            var connString3 = "newConnectionString";

            //Act
            var resp1 = await baseRepo.RunCommand(dummySqlCommand, new { }, connString1, DbType.Sybase);
            var resp2 = await baseRepo.RunCommand(dummySqlCommand, new { }, connString2, DbType.SqlServer);
            var resp3 = await baseRepo.RunCommand(dummySqlCommand, new { }, connString3, DbType.Oracle);

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SybaseConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.OracleConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, connString1, new { }), Times.Once);
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlCommand, connString2, new { }), Times.Once);
            dbExecutorMock.Verify(x => x.ExecuteOracleCommand(dummySqlCommand, connString3, new { }), Times.Once);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldNotUseDefaultConnectionString_WhenConnectionIsProvided)}"));
            Assert.Equal(CommandResp.Success, resp1);
            Assert.Equal(CommandResp.Success, resp2);
            Assert.Equal(CommandResp.Success, resp3);
        }

        [Fact]
        public async Task ShouldLogFailures_WhenErrorsOccurs()
        {
            //Arrange
            dbExecutorMock.Setup(x => x.ExecuteCommand(It.IsAny<string>(), ConnectionStrings.SqlServerConnection, new { })).
                Throws(new Exception("Exception happened"));

            //Act
            var resp = await baseRepo.RunCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { });

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Once);
            loggerMock.Verify(x => x.LogError(It.IsAny<string>()));
        }

        //should return failure
    }
}