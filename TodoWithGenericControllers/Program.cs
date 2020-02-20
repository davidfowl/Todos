using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddInMemoryCollection(builder.Configuration.KeyValueDbSetAndGenericApi(typeof(GenericApiController<,>), (typeof(Todo), typeof(int), nameof(Todo.Id))));
            builder.Services.AddDbContext<EfCoreStorageDbContext>(options => options.UseInMemoryDatabase("Todos")).AddScoped(typeof(IStorage<,>), typeof(EfCoreStorage<,>)).AddSwaggerGenCommon("Todo API");
            builder.Services.AddControllers(o => o.Conventions.Add(new GenericApiNameConvention())).ConfigureApplicationPartManager(o => o.FeatureProviders.Add(new GenericApiFeature(builder.Configuration)));
            
            var app = builder.Build();
            app.UseSwaggerCommon();
            app.MapControllers();

            await app.RunAsync();
        }
    }
}