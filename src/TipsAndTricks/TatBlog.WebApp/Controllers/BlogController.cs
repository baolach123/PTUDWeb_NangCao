using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Constants;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;



        public BlogController(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }
        

        public IActionResult About()
            => View();

        public IActionResult Contact()
            => View();

        public IActionResult Rss()
            => Content("Noi dung se duoc cap nhat");

        public async Task<IActionResult> Category(string slug, int pageNumber=1, int pageSize=10)
        {
                var postQuery = new PostQuery()
                {
                    PublishedOnly = true,
                    CategorySlug = slug                   
                };
                
                var postList = await _blogRepository
                    .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

                ViewBag.PostQuery = postQuery;

                return View(postList);
        }

        public async Task<IActionResult> Author(string slug, int pageNumber = 1, int pageSize = 10)
        {
            var postQuery = new PostQuery()
            {
                PublishedOnly = true,
                AuthorSlug = slug
            };

            var postList = await _blogRepository
                .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;

            return View(postList);
        }

        public async Task<IActionResult> Tag(string slug, int pageNumber = 1, int pageSize = 10)
        {
            var postQuery = new PostQuery()
            {
                PublishedOnly = true,
                TagSlug = slug
            };

            var postList = await _blogRepository
                .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;

            return View(postList);
        }

        public async Task<IActionResult> Post(
             int year,
             int month,
             int day,
             string slug = null,
             int pageNumber = 1,
             int pageSize = 2)
        {
            // Tạo đối tượng chứa các điều kiện truy vấn
            var postQuery = new PostQuery()
            {
                // Chỉ lấy những bài viết có trạng thái Published
                PublishedOnly = true,

                // Tìm bài viết theo từ khóa
                PostSlug = slug,
                PostedYear = year,
                PostedMonth = month,
                PostedDay = day
            };

            // Truy vấn các bài viết theo điều kiện đã tạo
            var postsList = await _blogRepository.GetPagedPostsAsync(postQuery, pageNumber, pageSize);
            await _blogRepository.IncreaseViewCountAsync(postsList.FirstOrDefault().Id);

            // Lưu lại điều kiện truy vấn để hiển thị trong View
            ViewBag.PostQuery = postQuery;

            return View(postsList);
        }

        public async Task<IActionResult> Index(
            [FromQuery(Name ="slug")]string slug=null,
            [FromQuery(Name ="p")]int pageNumber=1,
            [FromQuery(Name ="ps")] int pageSize=10)
        {
            var postQuery = new PostQuery()
            {
                PublishedOnly = true,                
                KeyWord=slug
            };

            var postList = await _blogRepository
                .GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;

            return View(postList);
        }
   

    }
}
