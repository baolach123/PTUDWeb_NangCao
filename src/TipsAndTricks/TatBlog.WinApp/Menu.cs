//using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Microsoft.Extensions.Caching.Memory;
//using System.Threading.Channels;
//using TatBlog.Core.Constants;
//using TatBlog.Core.Contracts;
//using TatBlog.Core.Entities;
//using TatBlog.Data.Contexts;
//using TatBlog.Data.Seeders;
//using TatBlog.Services.Blogs;
//using TatBlog.WinApp;



//namespace TatBlog.WinApp
//{
//    public class Menu
//    {
//        public void XuatMenu()
//        {
//            Console.WriteLine("---------Menu--------");
//            Console.WriteLine("0.Thoat");
//            Console.WriteLine("Phan I: ");
//            Console.WriteLine("1.Tim mot the theo ten dind danh");
//            Console.WriteLine("2.Lay tat ca cac the kem theo so bai viet chua the do");
//            Console.WriteLine("3.Xoa mot ma the cho truoc");
//            Console.WriteLine("4.Tim mot chuyen muc theo ten dinh danh");
//            Console.WriteLine("5.Tim mot chuyen muc theo ma so cho truoc");
//            Console.WriteLine("6.Them hoac cap nhat mot chuyen muc/ chu de");
//            Console.WriteLine("7.Xoa mot chuyen muc theo ma so cho truoc");
//            Console.WriteLine("8.Kiem tra ten dinh danh cua mot chuyen muc da ton tai hay chua");
//            Console.WriteLine("9.Lay va phan trang danh sach chuyen muc");
//            Console.WriteLine("10.Dem so luong bai viet trong N thang gan nhat");
//            Console.WriteLine("11.Tim mot bai viet theo ma so");
//            Console.WriteLine("12.Them hay cap nhat mot bai viet");
//            Console.WriteLine("13.Chuyen doi trang thai publish cua bai viet");
//            Console.WriteLine("14.Tim tat ca bai viet thoa man dieu kien");
//            Console.WriteLine("15.Dem so luong bai viet thoa man dieu kien");
//            Console.WriteLine("16.Tim va phan trang bai viet thoa man dieu kien PostQuery tra ve IPagedList<Post>");
//            Console.WriteLine("17.Tim va phan trang bai viet thoa man dieu kien PostQuery tra ve IPagedList<T>");
//            Console.WriteLine("Phan II:");
//            Console.WriteLine("18.Tim mot tac gia theo ma so");
//            Console.WriteLine("19.Tim mot tac gia theo ten dinh danh");
//            Console.WriteLine("20.Lấy và phân trang danh sách tác giả kèm theo số lượng bài viết của tác giả đó. Kết quả trả về kiểu IPagedList<AuthorItem>");
//            Console.WriteLine("21.Thêm hoặc cập nhật thông tin một tác giả. ");
//            Console.WriteLine("22. Tìm danh sách N tác giả có nhiều bài viết nhất. N là tham số đầu vào");
//        }

//        public int ChonMenu()
//        {

//            Console.WriteLine("Chon chuc nang:");
//            int n = Convert.ToInt32(Console.ReadLine());
//            return n;
//        }

//        public async void XyLyMenu(int n)
//        {
//            var context = new BlogDbContext();
//            var _memoryCache = new IMemoryCache
//            IBlogRepository blogRepo = new BlogRepository(context);
//            switch (n)
//            {
//                case 0:
//                    return;
//                    break;
//                case 1:
//                    Console.WriteLine("Tim mot the theo ten dind danh:");
//                    var tagf = await blogRepo.SeekTagWithUrlslugAync("ADuGoogle");
//                    Console.WriteLine(tagf.Name + "");
//                    break;

//                case 2:
//                    Console.WriteLine("Lay tat ca cac the kem theo so bai viet chua the do");
//                    var tagList = await blogRepo.GetListTagAndAmountOfPostInTagAsync();
//                    foreach (var tag in tagList)
//                    {
//                        Console.WriteLine("{0,-50}{1,-3}", tag.Name, tag.PostCount);
//                    }
//                    break;

//                case 3:
//                    Console.WriteLine("Xoa mot ma the cho truoc");
//                    await blogRepo.RemoveTagByIdAsync(7);
//                    break;

//                case 4:
//                    Console.WriteLine("Tim mot chuyen muc theo ten dinh danh");
//                    var category = await blogRepo.SeekCategoryAsync("netcore");
//                    Console.WriteLine(category.Name + "");
//                    break;

//                case 5:
//                    Console.WriteLine("Tim mot chuyen muc theo ma so cho truoc");
//                    var categoryA = await blogRepo.SeekCategoryByIdAsync(1);
//                    Console.WriteLine(categoryA.Name + "");
//                    break;

//                case 6:
//                    Console.WriteLine("Them hoac cap nhat mot chuyen muc/ chu de");
//                    Category category1 = new Category()
//                    {
//                        Name = "Du2",
//                        Description = "Du-Abc",
//                        UrlSlug = "DuAbc",
//                        Id = -1
//                    };
//                    await blogRepo.AddOrUpdateCategoryAsysc(category1);
//                    break;

//                case 7:
//                    Console.WriteLine("Xoa mot chuyen muc theo ma so cho truoc");
//                    await blogRepo.RemoveCategoryByIdAsync(1);
//                    Console.WriteLine("Da xoa");
//                    break;

//                case 8:
//                    Console.WriteLine("Kiem tra ten dinh danh cua mot chuyen muc da ton tai hay chua");
//                    Category category2 = new Category()
//                    {
//                        UrlSlug = "OOP"
//                    };
//                    await blogRepo.CheckExistCategoryAsync(category2);
//                    break;

//                case 9:
//                    Console.WriteLine("Lay va phan trang danh sach chuyen muc");
//                    var pagingParams = new PagingParams()
//                    {
//                        PageNumber = 1,
//                        PageSize = 5,
//                        SortColumn = "Id",
//                        SortOrder = "Asc"
//                    };


//                    var categoryPage = await blogRepo.GetPagingCategoryAsync(pagingParams);
//                    Console.WriteLine("{0,-5}{1,-50}{2,10}", "Id", "Name", "Count");

//                    foreach (var item in categoryPage)
//                    {
//                        Console.WriteLine("{0,-5}{1,-50}{2,10}", item.Id, item.Name, item.PostCount);
//                    }
//                    break;

//                case 10:
//                    Console.WriteLine("Dem so luong bai viet trong N thang gan nhat");

//                    break;


//                case 11:
//                    Console.WriteLine("Tim mot bai viet theo ma so");
//                    var postId = await blogRepo.SeekPostByIdAsync(2);
//                    Console.WriteLine(postId.Title + "");
//                    break;

//                case 12:
//                    Console.WriteLine("12.Them hay cap nhat mot bai viet");
//                    Post post = new Post()
//                    {
//                        Title = "React js",
//                        ShortDescription = "Du muon lam reactjs",
//                        Description = "Reacjs muon nam",
//                        Meta = "Du va cac ban phan hoc reactjs",
//                        UrlSlug = "react-js-remake-learn",
//                        Published = true,
//                        PostedDate = new DateTime(2023, 3, 6, 8, 39, 0),
//                        ModifiedDate = new DateTime(2023, 3, 6, 8, 39, 0),
//                        ViewCount = 15,
//                        Author = new(),
//                        Category = new(),
//                        Tags = new List<Tag>()
//                    };
//                    await blogRepo.AddOrUpdatePostAsysc(post);
//                    break;

//                case 13:
//                    Console.WriteLine("Chuyen doi trang thai publish cua bai viet");
//                    await blogRepo.ChangeStatusPublishAsync(2);
//                    break;

//                case 14:
//                    Console.WriteLine("Tim tat ca bai viet thoa man dieu kien");
//                    PostQuery query = new PostQuery()
//                    {
//                        PostId = 2
//                    };
//                    var listPosts = await blogRepo.SeekAllPostAsync(query);
//                    foreach (var item in listPosts)
//                    {
//                        Console.WriteLine(item.Title + "");
//                    }
//                    break;

//                case 15:
//                    Console.WriteLine("15.Dem so luong bai viet thoa man dieu kien");
//                    PostQuery query1 = new PostQuery()
//                    {
//                        PostId = 2
//                    };
//                    int count = await blogRepo.CountNumberPostAsync(query1);
//                    Console.WriteLine(count);
//                    break;

//                case 16:
//                    Console.WriteLine("Tim va phan trang bai viet thoa man dieu kien PostQuery tra ve IPagedList<Post>");
//                    var pagingParams1 = new PagingParams()
//                    {
//                        PageNumber = 1,
//                        PageSize = 5,
//                        SortColumn = "Id",
//                        SortOrder = "Desc"
//                    };
//                    PostQuery query2 = new PostQuery()
//                    {
//                        PostId = 2
//                    };
//                    await blogRepo.SeekPagingPostAsync(query2, pagingParams1);
//                    var categoryPaged = await blogRepo.GetPagingCategoryAsync(pagingParams1);
//                    Console.WriteLine("{0,-5}{1,-50}{2,10}", "Id", "Name", "Count");

//                    foreach (var item in categoryPaged)
//                    {
//                        Console.WriteLine("{0,-5}{1,-50}{2,10}", item.Id, item.Name, item.PostCount);
//                    }
//                    break;

//                case 17:
//                    Console.WriteLine("Tim va phan trang bai viet thoa man dieu kien PostQuery tra ve IPagedList<T>");
//                    break;
//            }
//        }
//    }

//}
