using System.Reflection;

namespace TodosIntegrationTest.TestSetup
{
    public class TestFixture<TProgram> where TProgram : class
    {
        public TestFixture()
        {
            var methodInfo = typeof(TProgram).GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);
            string[] args = { };
            methodInfo.Invoke(null, new[] { args });
        }
    }
}