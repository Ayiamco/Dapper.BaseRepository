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
    }
}
