using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeders
{
    public class DataSeeder : IDataSeeder
    {
        private readonly BlogDbContext _dbContext;

        public DataSeeder(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Initialize()
        {
            _dbContext.Database.EnsureCreated();

            if (_dbContext.Posts.Any()) return;
            var authors = AddAuthors();
            var categories = AddCategories();
            var tags = AddTags();
            var posts = AddPosts(authors, categories, tags);
        }



        // Nhap thong tin tac gia
        private IList<Author> AddAuthors()
        {
            var authors = new List<Author>()
            {
                new()
                {
                    FullName = "Jason Mouth",
                    UrlSlug = "jason-mouth",
                    Email = "json@mail.com",
                    JoinedDate = new DateTime(2022, 10, 21)
                },
                new()
                {
                    FullName = "Jessica Wonder",
                    UrlSlug = "Jessica-wonder",
                    Email = "jessica665@motip.com",
                    JoinedDate = new DateTime(2020, 4, 19)
                },
                new()
                {
                    FullName = "BuiVanDu",
                    UrlSlug = "Bui-Van-Du",
                    Email = "buivandu2002@gmail.com",
                    JoinedDate = new DateTime(2023, 3, 2)
                },
                new()
                {
                    FullName = "DangNgocThang",
                    UrlSlug = "Dang-Ngoc-Thang",
                    Email = "DangNgocThang@gmail.com",
                    JoinedDate = new DateTime(2023, 3, 9)
                },
                new()
                {
                    FullName = "DoanCaoNhatHa",
                    UrlSlug = "doan-cao-nhat-ha",
                    Email = "DoanCaoNhatHa@gmail.com",
                    JoinedDate = new DateTime(2023, 3, 20)
                },
                new()
                {
                    FullName = "NguyenTuanKietIdol",
                    UrlSlug = "nguyen-tuan-kiet-idol",
                    Email = "nguyentuankietidol@gmail.com",
                    JoinedDate = new DateTime(2023, 2, 10)
                },


            };
            //add vao db
            foreach (var author in authors)
            {
                if (!_dbContext.Author.Any(a => a.UrlSlug == author.UrlSlug))
                    _dbContext.Author.Add(author);
            }
           // _dbContext.Author.AddRange(authors);
            _dbContext.SaveChanges();
            return authors;
        }



        //nhap thong tin category
        private IList<Category> AddCategories() {
            var categories = new List<Category>()
        {
            new(){Name=".NET Core", Description=".NET Core", UrlSlug="NETCore"},
            new(){Name="Architecture", Description="Architecture", UrlSlug="Architecture"},
            new(){Name="Messaging", Description="Messaging", UrlSlug="Messaging"},
            new(){Name="OOP", Description="Object-Oriented Programing", UrlSlug="OOP"},
            new(){Name="Design Patterns", Description="Design Patterns", UrlSlug="DesignPatterns"},
            new(){Name="DuInfo", Description="Information Of Du", UrlSlug="PersonalInformationOfDu"}
        };
            foreach (var category in categories)
            {
                if (!_dbContext.Categoties.Any(a => a.UrlSlug == category.UrlSlug))
                    _dbContext.Categoties.Add(category);
            }   
            //_dbContext.AddRange(categories);
            _dbContext.SaveChanges();
            return categories;
        
        }


        //nhap thong tin tag
        private IList<Tag> AddTags() {
            var tags = new List<Tag>()
        {
            new(){Name="Google", Description="Google applications",UrlSlug="ADuGoogle"},
            new(){Name="ASP.NET MVC", Description="ASP.NET MVC ",UrlSlug="ADuASP.NET"},
            new(){Name="Razor Page", Description="Razor Page",UrlSlug="ADuRazorPage"},
            new(){Name="Blazor", Description="Blazor",UrlSlug="ADuBlazor"},
            new(){Name="Deep Learning", Description="Deep Learning",UrlSlug="ADuDeepLearning"},
            new(){Name="Neural Network", Description="Neural Network",UrlSlug="ADuNeuralNetwork"},
            new(){Name="BuiVanDu", Description="BuiVanDu", UrlSlug="ADuBuiVanDu"}
        };

            foreach (var tag in tags)
            {
                if (!_dbContext.Tags.Any(t => t.UrlSlug == tag.UrlSlug))
                {  
                    _dbContext.Tags.Add(tag);
                }
            }

            //_dbContext.AddRange(tags);
            _dbContext.SaveChanges();
            return tags;
                
        }




        //nhap thong tin post
        private IList<Post> AddPosts(
              IList<Author> authors,
              IList<Category> Categories,
              IList<Tag> tags
            )
        {
            var posts = new List<Post>()
        {
            new(){
            Title ="ASP.NET Core Diagnostic Scenarios",
            ShortDescription="David and friends has a great repository",
            Description="Here is a few great DONT and DO examples",
            Meta="David and friends has a frear repository",
            UrlSlug="aspnet-core-diagnostic-scenarios",
            Published=true,
            PostedDate=new DateTime(2021, 9, 30, 10, 20, 0),
            ModifiedDate=null,
            ViewCount=10,
            Author=authors[0],
            Category = Categories[0],
            Tags= new List<Tag>()
            {
                tags[0]           
            }
            },
            new(){
            Title ="Anh du qua la dep trai",
            ShortDescription="Du 20 tuoi, cao 1m66, nang 1 ta 1",
            Description="Tai sao cuoc doi co the san sinh ra duoc mot nguoi dep trai nhu Du",
            Meta="Cai nay thi khong hieu de lam gi",
            UrlSlug="buivandu-cdvt-vippro123",
            Published=true,
            PostedDate=new DateTime(2023, 3, 2, 17, 9, 20),
            ModifiedDate=null,
            ViewCount=10,
            Author=authors[2],
            Category = Categories[0],
            Tags= new List<Tag>()
            {
                tags[0]
            }
            },
             new(){
            Title ="Google qwertyuiio",
            ShortDescription="shienshisha",
            Description="Decreofasfisha",
            Meta="Deltaable",
            UrlSlug="abc-123-asd-jkl",
            Published=true,
            PostedDate=new DateTime(2022, 10, 20, 8, 3, 0),
            ModifiedDate=null,
            ViewCount=5,
            Author=authors[2],
            Category = Categories[1],
            Tags= new List<Tag>()
            {
                tags[6]               
            }
            },
             new(){
            Title ="Hom Nay Phai Chay DeadLine",
            ShortDescription="Chay deadline nhieu khong tot cho suc khoe",
            Description="cac ban hay nen de deadline tu chay",
            Meta="meta1",
            UrlSlug="chay-deadline-cham-chi",
            Published=true,
            PostedDate=new DateTime(2021, 9, 10, 2, 10, 3),
            ModifiedDate=null,
            ViewCount=3,
            Author=authors[5],
            Category = Categories[4],
            Tags= new List<Tag>()
            {
                tags[3]
            }
            },
             new(){
            Title ="Cung nhau hoc hanh",
            ShortDescription="hoc cham chi len",
            Description="hay hoc cham chi",
            Meta="phien ban moi",
            UrlSlug="hoc-hanh-cham-chi",
            Published=true,
            PostedDate=new DateTime(2023, 2, 10, 10, 5, 0),
            ModifiedDate=null,
            ViewCount=2,
            Author=authors[3],
            Category = Categories[1],
            Tags= new List<Tag>()
            {
                tags[3]
            }
            },
        };
            foreach ( var post in posts )
            {
                if(!_dbContext.Posts.Any(p=>p.UrlSlug==post.UrlSlug))
                    _dbContext.Posts.Add(post);
            }
           // _dbContext.AddRange(posts); 
            _dbContext.SaveChanges();
            return posts;
        }
      
             
        
    }
}
