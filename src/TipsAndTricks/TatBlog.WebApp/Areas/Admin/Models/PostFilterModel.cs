using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class PostFilterModel
    {
        [DisplayName("tu khoa")]
        public string Keyword { get; set; }

        [DisplayName("tac gia")]
        public int? AuthorId { get; set; }

        [DisplayName("chu de")]
        public int? CategoryId { get; set; }

        [DisplayName("nam")]
        public int? Year { get; set; }

        [DisplayName("thang")]
        public int? Month  { get; set; }

        [DisplayName("Chua xuat ban")]
        public bool NotPublished { get; set; } = false;


        public IEnumerable<SelectListItem> AuthorList { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> MonthList { get; set; }

        public PostFilterModel() {
        MonthList=Enumerable.Range(1,12)
                .Select(i => new SelectListItem()
                {
                    Value=i.ToString(),
                    Text=CultureInfo.CurrentCulture
                    .DateTimeFormat.GetMonthName(i)
                }).ToList();
        }

    }
}
