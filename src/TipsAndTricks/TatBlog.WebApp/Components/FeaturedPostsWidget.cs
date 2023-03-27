using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class FeaturedPostsWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;

        public FeaturedPostsWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // display 5 post random
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var postList = await _blogRepository.GetPopularArticlesAsync(3);
            return View(postList);
        }
    }
}
