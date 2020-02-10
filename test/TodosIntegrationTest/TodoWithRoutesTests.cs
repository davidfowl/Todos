using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodosIntegrationTest.TestSetup;
using Xunit;

namespace TodosIntegrationTest
{
    public class TodoWithRoutesTests : TestBase, IClassFixture<TestFixture<TodoWithRoutes.Program>>
    {
        private const string ApiUrl = "/api/todos";
        private readonly HttpClient _client;

        public TodoWithRoutesTests(TestFixture<TodoWithRoutes.Program> fixture)
        {
            _client = fixture.Client;
        }

        [Fact, TestPriority(0)]
        public async Task Create_Todo_Returns_Ok()
        {
            // Arrange
            var todo = new TodoBasic.Todo
            {
                Name = "Todo 1",
                IsComplete = false
            };
            var content = new StringContent(JsonSerializer.Serialize(todo), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(ApiUrl, content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact, TestPriority(0)]
        public async Task Get_Todo_Returns_Ok()
        {
            // Arrange
            await Create_Todo_Returns_Ok();

            // Act
            var response = await _client.GetAsync($"{ApiUrl}/1");
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact, TestPriority(0)]
        public async Task Get_All_Todos_Returns_Ok()
        {
            // Arrange
            await Create_Todo_Returns_Ok();
            await Create_Todo_Returns_Ok();

            // Act
            var response = await _client.GetAsync(ApiUrl);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact, TestPriority(1)]
        public async Task Delete_Todo_Returns_Ok()
        {
            // Arrange
            await Create_Todo_Returns_Ok();

            // Act
            var response = await _client.DeleteAsync($"{ApiUrl}/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}