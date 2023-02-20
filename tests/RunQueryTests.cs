using Dapper.BaseRepository.Config;

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
            dbExecutorMock.Setup(x => x.Query<TestObject>(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }))
            .Returns(testQueryResult);

            //Act
            var resp = await baseRepo.RunQuery<TestObject>(dummySqlCommand, new { });

            //Assert
            Assert.Equal(resp, testQueryResult);
        }

        [Fact]
        public async Task ShouldReturnEmptyEnumerable_WhenErrorOccurs()
        {
            //Arrange
            dbExecutorMock.Setup(x => x.Query<TestObject>(dummySqlCommand, ConnectionStrings.SqlServerConnection, new { }))
            .Throws(new Exception("error occured"));

            //Act
            var resp = await baseRepo.RunQuery<TestObject>(dummySqlCommand, new { });

            //Assert
            Assert.Equal(resp, Enumerable.Empty<TestObject>());
        }

        //should log error
        //should log successful query
    }
}
