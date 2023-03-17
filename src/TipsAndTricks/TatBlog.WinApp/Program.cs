using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.WinApp;




namespace TatBlog.WinApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            var context = new BlogDbContext();
            var seeder = new DataSeeder(context);
            seeder.Initialize();
            Menu menu = new Menu();
            menu.XuatMenu();
            int n;
            do
            {
                n = menu.ChonMenu();
                menu.XyLyMenu(n);
            } while (n > 0 && n < 20);

        }
    }
}



//#region FirstRun
//var seeder = new DataSeeder(context);
//var context = new BlogDbContext();


//seeder.Initialize();

//var authors = context.Author.ToList();

//Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12}",
//    "ID", "Full Name", "Email", "Joined Date");

//foreach (var author in authors)
//{
//    Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12:MM/dd/yyyy}",
//        author.Id, author.FullName, author.Email, author.JoinedDate);
//}
//#endregion

//#region SecondRun
//var context = new BlogDbContext();
//var posts = context.Posts
//    .Where(x => x.Published)
//    .OrderBy(x => x.Title)
//    .Select(p => new
//    {
//        Id = p.Id,
//        Title = p.Title,
//        ViewCount = p.ViewCount,
//        PostedDate = p.PostedDate,
//        Author = p.Author.FullName,
//        Category = p.Category.Name,
//    })
//    .ToList();

//foreach (var post in posts)
//{
//    Console.WriteLine("ID       :{0}", post.Id);
//    Console.WriteLine("Title    :{0}", post.Title);
//    Console.WriteLine("View     :{0}", post.ViewCount);
//    Console.WriteLine("Date     :{0}:MM/dd/yyyy", post.PostedDate);
//    Console.WriteLine("Author   :{0}", post.Author);
//    Console.WriteLine("Category :{0}", post.Category);
//    Console.WriteLine("".PadRight(80, '-'));
//}
//#endregion

//#region ThirdRun
//var context = new BlogDbContext();

//IBlogRepository blogRepo = new BlogRepository(context);

//var posts = await blogRepo.GetPopularArticlesAsync(3);

//foreach (var post in posts)
//{
//    Console.WriteLine("ID       :{0}", post.Id);
//    Console.WriteLine("Title    :{0}", post.Title);
//    Console.WriteLine("View     :{0}", post.ViewCount);
//    Console.WriteLine("Date     :{0}:MM/dd/yyyy", post.PostedDate);
//    Console.WriteLine("Author   :{0}", post.Author.FullName);
//    Console.WriteLine("Category :{0}", post.Category.Name);
//    Console.WriteLine("".PadRight(80, '-'));
//}
//#endregion

//#region FourthRun
//var context = new BlogDbContext();

//IBlogRepository blogRepo = new BlogRepository(context);

//var categories = await blogRepo.GetCategoriesAsync();

//Console.WriteLine("{0,-5}{1,-50}{2,10}",
//    "ID", "Name", "Count");

//foreach (var item in categories)
//{
//    Console.WriteLine("{0,-5}{1,-50}{2,10}",
//        item.Id, item.Name, item.PostCount);
//}
//#endregion

//#region Fifth Run

//var context = new BlogDbContext();

//IBlogRepository blogRepo = new BlogRepository(context);

//DataSeeder seeder = new DataSeeder(context);

//seeder.Initialize();

//var pagingParams = new PagingParams
//{
//    PageNumber = 1,
//    PageSize = 5,
//    SortColumn = "Name",
//    SortOrder = "DESC"
//};

//var tagsList = await blogRepo.GetPagedTagsAsync(pagingParams);
//Console.WriteLine("{0,-5}{1,-50}{2,-10}",
//"ID", "Name", "Count");


//foreach (var item in tagsList)
//{
//    Console.WriteLine("{0,-5}{1,-50}{2,-10}",
//        item.Id, item.Name, item.PostCount);
//}


//var tagf = await blogRepo.SeekTagWithUrlslugAync("ADuGoogle");
//Console.WriteLine(tagf.Name + "");



//var tagList = await blogRepo.GetListTagAndAmountOfPostInTagAsync();
//foreach (var tag in tagList)
//{
//    Console.WriteLine("{0,-50}{1,-3}", tag.Name, tag.PostCount);
//}

//var category = await blogRepo.SeekCategoryAsync("NETCore");
//Console.WriteLine(category.Name + "");

//#endregion

