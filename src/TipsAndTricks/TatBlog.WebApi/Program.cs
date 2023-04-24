using TatBlog.WebApi.Endpoints;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Mapsters;
using TatBlog.WebApi.Validations;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .ConfigureCors()
        .ConfigureNlog()
        .ConfigureServices()
        .ConfigureSwaggerOpenAPI()
        .ConfigureMapster()
        .ConfigureFluentValidation();    
}



var app = builder.Build();
{
    app.SetupRequestPipeLibe();
    app.MapAuthorEndPoints();
    app.MapCategoryEndpoints();
    app.MapPostEndpoints();
    app.Run();
}
