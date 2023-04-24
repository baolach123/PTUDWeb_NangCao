using TatBlog.WebApi.Models.Authors;
using TatBlog.WebApi.Models.Categories;
using TatBlog.WebApi.Models.Tags;

namespace TatBlog.WebApi.Models.Posts
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }

        public string UrlSlug { get; set; }

  
        public string ImageUrl { get; set; }

        public int ViewCount { get; set; }



        public CategoryDTO Category { get; set; }

        public AuthorDTO Author { get; set; }

        public IList<TagDTO> Tags { get; set; }

        public DateTime PostedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}