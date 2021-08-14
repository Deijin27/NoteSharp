using Notes.RouteUtil;
using Xunit;

namespace Tests.SetupParameterTests
{
    public class QueryBuilderTest
    {
        QueryBuilder QBuilder;
        public QueryBuilderTest()
        {
            QBuilder = new QueryBuilder();
        }

        [Fact]
        public void QueryBuilder_SingleQuery()
        {
            QBuilder.AddQuery("id", "20");
            Assert.Equal("?id=20", QBuilder.Result);
        }

        [Fact]
        public void QueryBuilder_TwoQueries()
        {
            QBuilder.AddQuery("name", "ember");
            QBuilder.AddQuery("age", "2");
            Assert.Equal("?name=ember&age=2", QBuilder.Result);
        }

        [Fact]
        public void QueryBuilder_ThreeQueries()
        {
            QBuilder.AddQuery("name", "ember");
            QBuilder.AddQuery("age", "2");
            QBuilder.AddQuery("ishungry", "true");
            Assert.Equal("?name=ember&age=2&ishungry=true", QBuilder.Result);
        }
    }
}
