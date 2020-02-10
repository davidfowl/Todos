using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace TodosIntegrationTest.TestSetup
{
    public class TestFixture<TProgram> where TProgram : class
    {
        public TestFixture()
        {
            var type = typeof(TProgram);
            var methodInfo = type.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);

            const string port = "5001";
            string[] args = { port };
            methodInfo.Invoke(null, new[] { args });

            Client = new HttpClient { BaseAddress = new Uri($"https://localhost:{port}") };
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClient Client { get; set; }
    }
}