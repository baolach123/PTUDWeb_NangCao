using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class ArchivesWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;

        public ArchivesWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        // display 5 post random
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var postList = await _blogRepository.ListMonth(12);
            return View(postList);
        }
    }
}
