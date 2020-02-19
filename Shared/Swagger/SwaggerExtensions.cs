using Microsoft.Extensions.DependencyInjection;

using Microsoft.OpenApi.Models;

namespace Microsoft.AspNetCore.Builder
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerGenCommon(
            this IServiceCollection services,
            string title,
            string version = "v1")
        {
            services.AddSwaggerGen(o => o.SwaggerDoc(version, new OpenApiInfo { Title = title }));
            return services;
        }

        public static IApplicationBuilder UseSwaggerCommon(
            this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API");
                o.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}
