using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;

namespace TodoBlazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<Main>("app");

            await builder.Build().RunAsync();
        }
    }
}
