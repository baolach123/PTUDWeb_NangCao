using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class RandomPostsWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;

        public RandomPostsWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // display 5 post random
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var postList = await _blogRepository.GetRandomNPostAsync(5);
            return View(postList);
        }
    }
}
