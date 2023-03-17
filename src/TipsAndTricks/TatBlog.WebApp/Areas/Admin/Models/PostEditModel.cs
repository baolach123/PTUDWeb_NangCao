using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;



namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class PostEditModel
    {
        public int Id { get; set; }

        [DisplayName("tieu de")]       
         public string Title { get; set; }

        [DisplayName("Giới thiệu")]
         public string ShortDescription { get; set; }

        [DisplayName("Nội dung")]
        public string Description { get; set; }

        [DisplayName("Metadata")]
        public string Meta { get; set; }

        [DisplayName("Slug")]
        [Remote("VerifyPostSlug","Posts", "Admin",
            HttpMethod = "POST", AdditionalFields = "Id")]
        public string UrlSlug { get; set; }

        [DisplayName("Chon hinh anh")]
        public IFormFile ImageFile { get; set; }

        [DisplayName("Hinh hien tai")]
        public string ImageUrl { get; set; }

        [DisplayName("xuat ban ngay")]
        public bool Published { get; set; }

        [DisplayName("Chu de")]

        public int CategoryId { get; set; }

        [DisplayName("Tac gia")]

        public int AuthorId { get; set; }

        [DisplayName("tu khoa (moi tu mot dong)")]

        public string SelectedTags { get; set; }

        public IEnumerable<SelectListItem> AuthorList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        public List<string> GetSelectedTags()
        {
            return (SelectedTags ?? "")
                .Split(new[] { ',', ';', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }





    }
}
