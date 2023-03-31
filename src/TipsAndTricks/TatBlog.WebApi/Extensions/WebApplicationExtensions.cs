using Microsoft.EntityFrameworkCore;
using NLog.Web;
using TatBlog.Data.Contexts;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.Services.Timing;

namespace TatBlog.WebApi.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();

            builder.Services.AddDbContext<BlogDbContext>(options=>options
            .UseSqlServer(builder.Configuration.GetConnectionString("LAPTOP-IHERN55O")));

            builder.Services.AddScoped<ITimeProvider,LocalTimeProvider>();
            builder.Services.AddScoped<IMediaManager,LocalFileSystemMediaManager>();
            builder.Services.AddScoped<IBlogRepository,BlogRepository>();
            builder.Services.AddScoped<IAuthorRepository,AuthorRepository>();

            return builder;

        }

        public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("TatBlogApp", policeBuilder =>
                policeBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
            return builder;
        }

        public static WebApplicationBuilder ConfigureNlog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
            return builder;
        }

        public static WebApplicationBuilder ConfigureSwaggerOpenAPI(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            return builder;
        }

        public static WebApplication SetupRequestPipeLibe(this WebApplication app) {

            if (app.Environment.IsDevelopment()){
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseCors("TatBlogApp");


            return app;
        }
    }
}
