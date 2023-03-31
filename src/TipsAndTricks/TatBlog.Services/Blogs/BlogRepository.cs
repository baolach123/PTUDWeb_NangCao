using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Constants;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class BlogRepository : IBlogRepository
{
    private readonly BlogDbContext _context;
    private readonly IMemoryCache _memoryCache;

    public BlogRepository(BlogDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }


    public async Task<Post> GetPostAsync(
      int year,
      int month,
      string slug,
      CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author);

        if(year > 0)
        {
            postsQuery = postsQuery.Where(x=>x.PostedDate.Year == year);
        }


        if (month > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
        }

        if(!string.IsNullOrEmpty(slug))
        {
            postsQuery=postsQuery.Where(x=>x.UrlSlug == slug);
        }

        return await postsQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<Post>> GetPopularArticlesAsync(
        int numPosts,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(x => x.Author)
            .Include(x => x.Category)
            .OrderByDescending(x => x.ViewCount)
            .Take(numPosts)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<Author>> GetPopularAuthorsAsync(
        int numAuthor,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Author>()
            .Include(x => x.Posts)
            .OrderByDescending(x => x.Posts.Count)
            .Take(numAuthor)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> GetCachedCategoryIdAsync(int categoryid)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"category.by-id.{categoryid}",
            async (entryu) =>
            {
                entryu.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await GetCategoryIdAsync(categoryid);
            });


    }





    public async Task<IPagedList<CategoryItem>> GetPagedCategoryAsync(
        IPagingParams pagingParams,
        string name = null,
        CancellationToken cancellationToken = default)
    {
        var categoryQuery = _context.Set<Category>()
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published),
                ShowOnMenu = x.ShowOnMenu,
            });
        return await categoryQuery
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<Category> GetCategoryIdAsync(int id, bool p = true, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
           .Where(x => x.Id == id)
           .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsPostSlugExixtedAsync(
        int postID, string slug,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .AnyAsync(x => x.Id != postID && x.UrlSlug == slug, cancellationToken);
            
    }


    public async Task<bool> IsCategorySlugExixtedAsync(
    int categoryId, string slug,
    CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AnyAsync(x => x.Id != categoryId && x.UrlSlug == slug, cancellationToken);

    }

    public async Task IncreaseViewCountAsync(
        int postId,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<Post>()
            .Where(x => x.Id == postId)
            .ExecuteUpdateAsync(p =>
            p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken);
    }


    public async Task<IList<CategoryItem>> GetCategoriesAsync(
        bool showOnMenu = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> categories = _context.Set<Category>();

        if (showOnMenu)
        {
            categories = categories.Where(x => x.ShowOnMenu);
        }

        return await categories
            .OrderBy(x => x.Name)
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                ShowOnMenu = x.ShowOnMenu,
                PostCount = x.Posts.Count(p => p.Published)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsCategorySlugExistedAsync(
        int categoryId, string categorySlug,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AnyAsync(x => x.Id != categoryId && x.UrlSlug == categorySlug, cancellationToken);

    }

    public async Task<bool> AddOrUpdateAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {

        if (category.Id > 0)
        {
            _context.Categoties.Update(category);
            _memoryCache.Remove($"category.by-id.{category.Id}");

        }
        else
        {
            _context.Categoties.Add(category);

        }
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteCategoryIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _context.Set<Category>().FindAsync(id);

        if (category is null) return false;


        _context.Set<Category>().Remove(category);

        var rowCount = await _context.SaveChangesAsync(cancellationToken);

        return rowCount > 0;
    }

    public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var tagQuery = _context.Set<Tag>()
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.posts.Count(p => p.Published)
            });

        return await tagQuery
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<Tag> SeekTagWithUrlslugAync(string slugTag, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .Where(x=>x.UrlSlug== slugTag)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<TagItem>> GetListTagAndAmountOfPostInTagAsync(CancellationToken cancellationToken = default)
    {
        var listTag = _context.Set<Tag>()
            .Select(t => new TagItem()
            {               
                Name = t.Name,               
                PostCount=t.posts.Count()
            });
        return await listTag
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> SeekCategoryAsync(string slugCategory, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .Where(x=>x.UrlSlug == slugCategory)
            .FirstOrDefaultAsync (cancellationToken);
    }

    public async Task<Category> SeekCategoryByIdAsync(int categoryId, CancellationToken cancellation = default)
    {
        return await _context.Set<Category>()
            .Where(x => x.Id == categoryId)
            .FirstOrDefaultAsync(cancellation);
    }

    public async Task RemoveTagByIdAsync(int Id, CancellationToken cancellationToken = default)
    {
        _context.Database.ExecuteSqlRawAsync("DELETE FROM PostTags WHERE TagsId = "+Id);
    }

    public async Task AddOrUpdateCategoryAsysc(Category category, CancellationToken cancellationToken = default)
    {
        if (IsCategorySlugExixtedAsync(category.Id, category.UrlSlug).Result)
        { Console.WriteLine("da ton tai"); }
        else if (category.Id > 0)
        {
            await _context.Set<Category>()
                .Where(x => x.Id == category.Id)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(a=>a.Name, category.Name)
                    .SetProperty(a=>a.Description, category.Description)
                    .SetProperty(a=>a.ShowOnMenu,category.ShowOnMenu)
                    .SetProperty(a=>a.Posts, category.Posts), cancellationToken);
        }
        else
        {
            _context.Categoties.Add(category);
            _context.SaveChanges();
        }
    }

    public async Task RemoveCategoryByIdAsync(int Id, CancellationToken cancellationToken = default)
    {
        _context.Database.ExecuteSqlRawAsync("DELETE FROM Category WHERE Id = " + Id);
    }


    public async Task CheckExistCategoryAsync(Category category, CancellationToken cancellationToken = default)
    {
        if (IsCategorySlugExixtedAsync(category.Id, category.UrlSlug).Result)
        { Console.WriteLine("da ton tai"); }
    }


    public async Task<IPagedList<CategoryItem>> GetPagingCategoryAsync(
        IPagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        var categoryQuery = _context.Set<Category>()
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.Posts.Count(p => p.Published),
                ShowOnMenu = x.ShowOnMenu,
            });
        return await categoryQuery
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<Post> SeekPostByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return  await _context.Set<Post>()
            .Where(x => x.Id ==id)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task AddOrUpdatePostAsysc(Post postt, CancellationToken cancellationToken = default)
    {
        if (IsPostSlugExixtedAsync(postt.Id, postt.UrlSlug).Result)
        { Console.WriteLine("da ton tai"); }
        else if (postt.Id > 0)
        {
            await _context.Set<Post>()
                .Where(x => x.Id == postt.Id)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(a => a.Title, postt.Title)
                    .SetProperty(a => a.ShortDescription, postt.ShortDescription)
                    .SetProperty(a => a.Description, postt.Description)
                    .SetProperty(a=>a.Meta, postt.Meta)
                    .SetProperty(a=>a.UrlSlug, postt.UrlSlug)
                    .SetProperty(a=>a.ImageUrl, postt.ImageUrl)
                    .SetProperty(a=>a.ViewCount, postt.ViewCount)
                    .SetProperty(a=>a.Published, postt.Published)
                    .SetProperty(a=>a.PostedDate, postt.PostedDate)
                    .SetProperty(a => a.ModifiedDate, postt.ModifiedDate)                   
                    .SetProperty(a=>a.Category, postt.Category)
                    .SetProperty(a=>a.Author, postt.Author)
                    .SetProperty(a=>a.Tags, postt.Tags)
                    , cancellationToken);   
        }
        else
        {
            _context.Posts.Add(postt);
            _context.SaveChanges();
        }
    }


    public async Task ChangeStatusPublishAsync(int postId,CancellationToken cancellationToken=default)
    {
        await _context.Set<Post>()
            .Where(x => x.Id == postId)
            .ExecuteUpdateAsync(
            a => a
            .SetProperty(c => c.Published, c => !c.Published)
            );       
    }
     
    public async Task<IList<Post>> GetRandomNPostAsync(int n, CancellationToken cancellationToken=default)
    {
        return await _context.Set<Post>()
            .OrderBy(x => Guid.NewGuid())
            .Take(n)
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<Post>> SeekAllPostAsync(PostQuery postQuery,CancellationToken cancellationToken=default)
    {
        return await _context.Set<Post>()
            .Include(c => c.Category)
            .Include(c => c.Tags)
            .Include(c => c.Author)
            .WhereIf(postQuery.AuthorId > 0, p => p.AuthorId == postQuery.AuthorId)
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.AuthorSlug),p=>p.Author.UrlSlug==postQuery.AuthorSlug)
            .WhereIf(postQuery.PostId > 0, p => p.Id == postQuery.PostId)
            .WhereIf(postQuery.CategoryId > 0, p => p.CategotyId == postQuery.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.CategorySlug), p => p.Category.UrlSlug == postQuery.CategorySlug)
            .WhereIf(postQuery.PostedYear > 0, p => p.PostedDate.Year == postQuery.PostedYear)
            .WhereIf(postQuery.PostedMonth > 0, p => p.PostedDate.Month == postQuery.PostedMonth)
            .WhereIf(postQuery.TagId > 0, p => p.Tags.Any(x => x.Id == postQuery.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.TagSlug), p => p.Tags.Any(x => x.UrlSlug == postQuery.TagSlug))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountNumberPostAsync(PostQuery postQuery, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(c => c.Category)
            .Include(c => c.Tags)
            .Include(c => c.Author)
            .WhereIf(postQuery.AuthorId > 0, p => p.AuthorId == postQuery.AuthorId)         
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.AuthorSlug), p => p.Author.UrlSlug == postQuery.AuthorSlug)
            .WhereIf(postQuery.PostId > 0, p => p.Id == postQuery.PostId)
            .WhereIf(postQuery.CategoryId > 0, p => p.CategotyId == postQuery.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.CategorySlug), p => p.Category.UrlSlug == postQuery.CategorySlug)
            .WhereIf(postQuery.PostedYear > 0, p => p.PostedDate.Year == postQuery.PostedYear)
            .WhereIf(postQuery.PostedMonth > 0, p => p.PostedDate.Month == postQuery.PostedMonth)
            .WhereIf(postQuery.TagId > 0, p => p.Tags.Any(x => x.Id == postQuery.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.TagSlug), p => p.Tags.Any(x => x.UrlSlug == postQuery.TagSlug))
            .CountAsync(cancellationToken);
    }

    public async Task<IPagedList<Post>> SeekPagingPostAsync(PostQuery postQuery,
            IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(c => c.Category)
            .Include(c => c.Tags)
            .Include(c=>c.Author)
            .WhereIf(postQuery.AuthorId > 0, p => p.AuthorId == postQuery.AuthorId)
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.AuthorSlug), p => p.Author.UrlSlug == postQuery.AuthorSlug)
            .WhereIf(postQuery.PostId > 0, p => p.Id == postQuery.PostId)
            .WhereIf(postQuery.CategoryId > 0, p => p.CategotyId == postQuery.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.CategorySlug), p => p.Category.UrlSlug == postQuery.CategorySlug)
            .WhereIf(postQuery.PostedYear > 0, p => p.PostedDate.Year == postQuery.PostedYear)
            .WhereIf(postQuery.PostedMonth > 0, p => p.PostedDate.Month == postQuery.PostedMonth)
            .WhereIf(postQuery.TagId > 0, p => p.Tags.Any(x => x.Id == postQuery.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(postQuery.TagSlug), p => p.Tags.Any(x => x.UrlSlug == postQuery.TagSlug))
            .ToPagedListAsync(pagingParams, cancellationToken);
    }


    private IQueryable<Post> FilterPosts(PostQuery condition)
    {
        //var posts = _context.Set<Post>()
        //        .Include(c => c.Category)
        //        .Include(t => t.Tags)
        //        .Include(a => a.Author);
        //IQueryable<Post> postQuery = posts
        //    .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
        //    .WhereIf(!string.IsNullOrWhiteSpace(pq.AuthorSlug), p => p.Author.UrlSlug == pq.AuthorSlug)
        //    .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
        //    .WhereIf(pq.CategoryId > 0, p => p.CategotyId == pq.CategoryId)
        //    .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug.Equals(pq.CategorySlug))
        //    .WhereIf(!string.IsNullOrWhiteSpace(pq.PostSlug), p => p.UrlSlug == pq.PostSlug)
        //    .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
        //    .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
        //    .WhereIf(pq.PostedDay > 0, p => p.PostedDate.Day == pq.PostedDay)
        //    .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
        //    .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
        //    .WhereIf(pq.PublishedOnly, p => p.Published == pq.PublishedOnly)
        //    .WhereIf(!string.IsNullOrWhiteSpace(pq.KeyWord), p => p.Title.Contains(pq.KeyWord) ||
        //        p.ShortDescription.Contains(pq.KeyWord) ||
        //        p.Description.Contains(pq.KeyWord) ||
        //        p.Category.Name.Contains(pq.KeyWord) ||
        //        p.Tags.Any(t => t.Name.Contains(pq.KeyWord)));

        return _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags)
            .WhereIf(condition.PublishedOnly, x => x.Published)
            .WhereIf(condition.NotPublished, x => !x.Published)
            .WhereIf(condition.CategoryId > 0, x => x.CategotyId == condition.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(condition.CategorySlug), x => x.Category.UrlSlug == condition.CategorySlug)
            .WhereIf(condition.AuthorId > 0, x => x.AuthorId == condition.AuthorId)
            .WhereIf(!string.IsNullOrWhiteSpace(condition.AuthorSlug), x => x.Author.UrlSlug == condition.AuthorSlug)
            .WhereIf(!string.IsNullOrWhiteSpace(condition.TagSlug), x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug))
            .WhereIf(!string.IsNullOrWhiteSpace(condition.KeyWord), x => x.Title.Contains(condition.KeyWord) ||
                                                                         x.ShortDescription.Contains(condition.KeyWord) ||
                                                                         x.Description.Contains(condition.KeyWord) ||
                                                                         x.Category.Name.Contains(condition.KeyWord) ||
                                                                         x.Tags.Any(t => t.Name.Contains(condition.KeyWord)))
            .WhereIf(condition.Year > 0, x => x.PostedDate.Year == condition.Year)
            .WhereIf(condition.Month > 0, x => x.PostedDate.Month == condition.Month)
            .WhereIf(!string.IsNullOrWhiteSpace(condition.TitleSlug), x => x.UrlSlug == condition.TitleSlug);

        //return postQuery;
    }

    public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
            PostQuery condition,
            IPagingParams pagingParams,
            Func<IQueryable<Post>, IQueryable<T>> mapper)
    {
        var posts = FilterPosts(condition);
        var projectedPosts = mapper(posts);
        
        return await projectedPosts.ToPagedListAsync(pagingParams);
    }

    public async Task<IPagedList<Post>> GetPagedPostsAsync(
                PostQuery condition,
                int pageNumber = 1,
                int pageSize = 10,
                CancellationToken cancellationToken = default)
    {
        return await FilterPosts(condition).ToPagedListAsync(
            pageNumber, pageSize,
            nameof(Post.PostedDate), "DESC",
            cancellationToken);
    }


    public async Task<IList<AuthorItem>> GetAuthorsAsync(
            CancellationToken cancellationToken = default)
    {
        return await _context.Set<Author>()
            .OrderBy(a => a.FullName)
            .Select(a => new AuthorItem()
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                JoinedDate = a.JoinedDate,
                ImageUrl = a.ImageUrl,
                UrlSlug = a.UrlSlug,
                PostCount = a.Posts.Count(P => P.Published)
            })
            .ToListAsync(cancellationToken);
    }



    public async Task<Post> GetPostByIdAsync(
        int postId, bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        if (!includeDetails)
        {
            return await _context.Set<Post>().FindAsync(postId);
        }

        return await _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == postId, cancellationToken);
    }


    public async Task<Tag> GetTagAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .FirstOrDefaultAsync(x => x.UrlSlug == slug, cancellationToken);
    }

    public async Task<IList<TagItem>> GetTagsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Tag>()
            .OrderBy(x => x.Name)
            .Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                PostCount = x.posts.Count(p => p.Published)
            })
            .ToListAsync(cancellationToken);
    }


    public async Task<Post> CreateOrUpdatePostAsync(
        Post post, IEnumerable<string> tags,
        CancellationToken cancellationToken = default)
    {
        if (post.Id > 0)
        {
            await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
        }
        else
        {
            post.Tags = new List<Tag>();
        }

        var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => new
            {
                Name = x,
                Slug = GenerateSlug(x)
            })
            .GroupBy(x => x.Slug)
            .ToDictionary(g => g.Key, g => g.First().Name);


        foreach (var kv in validTags)
        {
            if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

            var tag = await GetTagAsync(kv.Key, cancellationToken) ?? new Tag()
            {
                Name = kv.Value,
                Description = kv.Value,
                UrlSlug = kv.Key
            };

            post.Tags.Add(tag);
        }

        post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

        if (post.Id > 0)
            _context.Update(post);
        else
            _context.Add(post);

        await _context.SaveChangesAsync(cancellationToken);

        return post;
    }

    private string GenerateSlug(string s)
    {
        return s.ToLower().Replace(".", "dot").Replace(" ", "-");
    }

    public async Task DeletePostsByIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        await _context.Database
            .ExecuteSqlRawAsync("DELETE FROM Posts WHERE Id= " + postId, cancellationToken);

        await _context.Set<Post>()
            .Where(t=>t.Id == postId)
            .ExecuteDeleteAsync(cancellationToken);
    }


    public async Task<bool> TogglePublishedFlagAsync(
        int postId, CancellationToken cancellationToken = default)
    {
        var post = await _context.Set<Post>().FindAsync(postId);

        if (post is null) return false;

        post.Published = !post.Published;
        await _context.SaveChangesAsync(cancellationToken);

        return post.Published;
    }

    public async Task<IList<PostItem>> ListMonth(
            int n,
            CancellationToken cancellationToken = default)
    {

        return await _context.Set<Post>()
        .GroupBy(x => new { x.PostedDate.Year, x.PostedDate.Month })
        .Select(g => new PostItem()
        {
            Year = g.Key.Year,
            Month = g.Key.Month,
            PostCount = g.Count(x => x.Published)
        })
        .OrderByDescending(x => x.Year)
        .ThenByDescending(x => x.Month)
        .ToListAsync(cancellationToken);
    }
}
