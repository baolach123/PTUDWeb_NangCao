using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class BestAuthorsWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;

        public BestAuthorsWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // display 5 post random
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var postList = await _blogRepository.GetPopularAuthorsAsync(4);
            return View(postList);
        }
    }
}
