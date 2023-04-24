using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.Constants
{
    public class PostQuery
    {
        public int? AuthorId { get; set; }     
        public string CategorySlug { get; set; }
        public string TagSlug { get; set; }    

        public string TitleSlug { get; set; }
        public int PostId { get; set; }
        public bool PublishedOnly { get; set; }

        public int PostedMonth { get; set; }
        public int PostedDay { get; set; }
        public string AuthorSlug { get; set; }
        public int? CategoryId { get; set; }
        public int TagId { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string PostSlug { get; set; }

        public bool NotPublished { get; set; }
        public string KeyWord { get; set; }
        public int PostedYear { get; set; }
    }
}