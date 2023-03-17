using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Extensions;
using TatBlog.WebApp.Mapster;
using TatBlog.WebApp.Validations;

var builder = WebApplication.CreateBuilder(args);
{
    builder
        .ConfigureMvc()
        .ConfigureNLog()
        .ConfigureServies()
        .ConfigureMapster()
        .ConfigureFluentValidation();
}

var app = builder.Build();
{
    app.UseRequestPipeLine();
    app.UseBlogRoutes();
    app.UseDataSeeder();
}
//var builder = WebApplication.CreateBuilder(args);


//{
//    builder.Services.AddControllersWithViews();

//    builder.Services.AddDbContext<BlogDbContext>(options => options.UseSqlServer(
//                            builder.Configuration.GetConnectionString("DefaultConnection")));

//    builder.Services.AddScoped<IBlogRepository, BlogRepository>();
//    builder.Services.AddScoped<IDataSeeder, DataSeeder>();

//    builder
//        .ConfigureMvc()
//        .ConfigureServies();
//}




//var app = builder.Build();
//{
//    if (app.Environment.IsDevelopment())
//    {
//        app.UseDeveloperExceptionPage();
//    }

//    else
//    {
//        app.UseExceptionHandler("/Blog/Error");
//        app.UseHsts();
//    }


//    app.UseHttpsRedirection();
//    app.UseStaticFiles();
//    app.UseRouting();
//    app.MapControllerRoute(name: "default",
//        pattern: "{controller=Blog}/{action=Index}/{id?}");

//    app.UseRequestPipeLine();
//    app.UseBlogRoutes();
//    app.UseDataSeeder();
//}

//using(var scope = app.Services.CreateScope())
//{
//    var seeder=scope.ServiceProvider.GetRequiredService<IDataSeeder>();
//    seeder.Initialize();
//}


app.Run();