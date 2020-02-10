using Xunit;

namespace TodosIntegrationTest
{
    [TestCaseOrderer("TodosIntegrationTest.TestSetup.TestPriorityOrderer", "TodosIntegrationTest")]
    public abstract class TestBase
    {
    }
}