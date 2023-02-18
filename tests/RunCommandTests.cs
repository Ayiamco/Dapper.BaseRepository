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
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Never);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldCallSybase_WhenDbTypeIsSybase)}"), Times.Once);
            Assert.Equal(CommandResp.Success, resp);
        }

        [Fact]
        public async Task ShouldCallSqlServer_WhenDbTypeIsSqlServer()
        {
            //Act
            var resp = await baseRepo.RunCommand(dummySqlCommand, new { }, DbType.SqlServer);

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Once);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SybaseConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteOracleCommand(dummySqlCommand, ConnectionStrings.OracleConnection, new { }), Times.Never);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldCallSqlServer_WhenDbTypeIsSqlServer)}"), Times.Once);
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
            var sybaseConnString = "sybaseConnString";
            var sqlServerConnString = "sqlServerConnString";
            var oracleConnString = "oracleConnString";

            //Act
            var sybaseResp = await baseRepo.RunCommand(dummySqlCommand, new { }, sybaseConnString, DbType.Sybase);
            var sqlServerResp = await baseRepo.RunCommand(dummySqlCommand, new { }, sqlServerConnString, DbType.SqlServer);
            var oracleServerResp = await baseRepo.RunCommand(dummySqlCommand, new { }, oracleConnString, DbType.Oracle);

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SybaseConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, ConnectionStrings.OracleConnection, new { }), Times.Never);
            dbExecutorMock.Verify(x => x.ExecuteSybaseCommand(dummySqlCommand, sybaseConnString, new { }), Times.Once);
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlCommand, sqlServerConnString, new { }), Times.Once);
            dbExecutorMock.Verify(x => x.ExecuteOracleCommand(dummySqlCommand, oracleConnString, new { }), Times.Once);
            loggerMock.Verify(x => x.LogInformation($"Successfully ran command from function: {nameof(ShouldNotUseDefaultConnectionString_WhenConnectionIsProvided)}"));
            Assert.Equal(CommandResp.Success, sybaseResp);
            Assert.Equal(CommandResp.Success, sqlServerResp);
            Assert.Equal(CommandResp.Success, oracleServerResp);
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

        [Fact]
        public async Task ShouldReturnFailure_WhenErrorsOccurs()
        {
            //Arrange
            dbExecutorMock.Setup(x => x.ExecuteCommand(It.IsAny<string>(), ConnectionStrings.SqlServerConnection, new { })).
                Throws(new Exception("Exception happened"));

            //Act
            var resp = await baseRepo.RunCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { });

            //Assert
            dbExecutorMock.Verify(x => x.ExecuteCommand(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }), Times.Once);
            loggerMock.Verify(x => x.LogError(It.IsAny<string>()));
            Assert.Equal(CommandResp.Failure, resp);
        }
    }
}