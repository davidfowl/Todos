using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Todos;

namespace TodoWithDI.IntegrationTest
{
    public class TestFixture
    {
        public TestFixture()
        {
            var type = typeof(Todo).Assembly.GetType("Todos.Program");
            var methodInfo = type.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);

            string[] args = { "urls=https://localhost:5002" };
            methodInfo.Invoke(null, new[] { args });

            Client = new HttpClient { BaseAddress = new Uri("https://localhost:5002") };
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClient Client { get; set; }
    }
}