using Dapper.BaseRepository.Config;
using Moq;

namespace DapperBaseRepo.Tests
{
    public class RunQueryTests : BaseTestComponent
    {
        private IEnumerable<TestObject> testQueryResult { get; set; }

        public RunQueryTests()
        {
            testQueryResult = new List<TestObject>() { new TestObject() };
        }

        [Fact]
        public async Task ShouldReturnRowsAsEnumerable_WhenQueryIsSuccessful()
        {
            //Arrange
            dbExecutorMock.Setup(x => x.Query<TestObject>(dummySqlCommand, ConnectionStrings.SqlServerConnection!, new { }))
            .Returns(testQueryResult);

            //Act
            var resp = await repo.RunQuery<TestObject>(dummySqlCommand, new { });

            //Assert
            Assert.Equal(resp, testQueryResult);
        }

        [Fact]
        public async Task ShouldReturnEmptyEnumerable_WhenErrorOccurs()
        {
            //Arrange
            dbExecutorMock.Setup(x => x.Query<TestObject>(dummySqlCommand, ConnectionStrings.SqlServerConnection!, new { }))
            .Throws(new Exception("error occured"));

            //Act
            var resp = await repo.RunQuery<TestObject>(dummySqlCommand, new { });

            //Assert
            Assert.Equal(resp, Enumerable.Empty<TestObject>());
        }

        [Fact]
        public async Task ShouldUseSqlServerAsDefaultConnection_WhenDbTypeIsNotPassed()
        {
            //Arrange

            dbExecutorMock.Setup(x => x.Query<TestObject>(dummySqlCommand, It.IsAny<string>(), new { }))
            .Returns(new List<TestObject>().AsEnumerable());

            //Act
            var resp = await repo.RunQuery<TestObject>(dummySqlCommand);

            //Assert
            Assert.Equal(resp, new List<TestObject>().AsEnumerable());
            dbExecutorMock.Verify(x => x.Query<TestObject>(dummySqlCommand, ConnectionStrings.SqlServerConnection!, It.IsAny<object>()), Times.Once);
        }


        //should log error
        //should log successful query
    }
}
