using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class TagCloudWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;

        public TagCloudWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // display 5 post random
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var postList = await _blogRepository.GetTagsAsync();
            return View(postList);
        }
    }
}
