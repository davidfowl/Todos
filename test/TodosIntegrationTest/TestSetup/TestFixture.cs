using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
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
            string[] args = { };
            methodInfo.Invoke(null, new[] { args });

            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory());
            var solutionDirectory = $"{directoryInfo.Parent.Parent.Parent.Parent.FullName}";
            var projectName = type.FullName.Split('.')[0];
            var launchSettingsPath = $"{solutionDirectory}\\{projectName}\\Properties\\launchSettings.json";

            using var file = File.OpenText(launchSettingsPath);
            using var reader = new JsonTextReader(file);
            dynamic o2 = JToken.ReadFrom(reader);

            var port = o2.iisSettings.iisExpress.sslPort;
            Client = new HttpClient { BaseAddress = new Uri($"https://localhost:{port}") };
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClient Client { get; set; }
    }
}