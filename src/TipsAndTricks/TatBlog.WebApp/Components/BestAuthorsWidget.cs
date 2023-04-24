using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
    public class BestAuthorsWidget : ViewComponent
    {
        private readonly IBlogRepository _blogRepository;
        private readonly AuthorRepository _authorRepository;

        public BestAuthorsWidget(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var postList = await _authorRepository.GetCachedAuthorByIdAsync(4);
            return View(postList);
        }
    }
}
