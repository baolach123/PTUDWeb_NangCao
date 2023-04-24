using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Entities;
using TatBlog.Core.DTO;
using TatBlog.Core.Contracts;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Xml.Linq;
using TatBlog.Core.Constants;

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
        int year, int month,
        string slug, CancellationToken cancellationToken = default)
    {
        IQueryable<Post> postsQuery = _context.Set<Post>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .Include(x => x.Tags);

        if (year > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
        }

        if (month > 0)
        {
            postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
        }

        return await postsQuery.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IList<Post>> GetPopularArticlesAsync(
        int numPosts, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(x => x.Author)
            .Include(x => x.Category)
            .OrderByDescending(p => p.ViewCount)
            .Take(numPosts)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsPostSlugExistedAsync(
        int postId, string slug, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .AnyAsync(x => x.Id != postId && x.UrlSlug == slug,
            cancellationToken);
    }

    public async Task IncreaseViewCountAsync(
        int postId, CancellationToken cancellationToken = default)
    {
        await _context.Set<Post>()
            .Where(x => x.Id == postId)
            .ExecuteUpdateAsync(p => p.SetProperty(
                x => x.ViewCount, x => x.ViewCount + 1),
                cancellationToken);
    }

    public async Task<IList<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
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

    public async Task<IPagedList<TagItem>> GetPagedTagsAsync(
        IPagingParams pagingParams, CancellationToken cancellationToken = default)
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

        return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
    }



    public async Task<Category> FindCategoryByUrlAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Category> categoriesQuery = _context.Set<Category>();
        {
            if (!string.IsNullOrWhiteSpace(slug))
            {
                categoriesQuery = categoriesQuery.Where(x => x.UrlSlug == slug);
            }
        }
        return await categoriesQuery.FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<Category> FindCategoryByIDAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Category> CreateOrUpdateCategoryAsync(
        Category category, CancellationToken cancellationToken = default)
    {
        if (category.Id > 0)
        {
            _context.Set<Category>().Update(category);
        }
        else
        {
            _context.Set<Category>().Add(category);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return category;
    }

    public async Task<bool> DeleteCategoryByIdAsync(
     int categoryId,
     CancellationToken cancellationToken = default)
    {
        var category = await _context.Set<Category>().FindAsync(categoryId);
        if (category is null) return false;
        _context.Set<Category>().Remove(category);
        var rowsCount = await _context.SaveChangesAsync(cancellationToken);
        return rowsCount > 0;
    }

    public async Task<bool> ToggleShowOnMenuFlagAsync(
      int categoryId,
      CancellationToken cancellationToken = default)
    {
        var category = await _context.Set<Category>().FindAsync(categoryId);
        if (category is null) return false;
        category.ShowOnMenu = !category.ShowOnMenu;
        await _context.SaveChangesAsync(cancellationToken);
        return category.ShowOnMenu;
    }


    public async Task<bool> IsCategorySlugExistedAsync(
    int categoryId,
    string slug,
    CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AnyAsync(x => x.Id != categoryId && x.UrlSlug == slug, cancellationToken);
    }

    public async Task<IPagedList<CategoryItem>> GetPagedCategoryAsync(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var categoriesQuery = _context.Set<Category>()
            .Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                UrlSlug = x.UrlSlug,
                Description = x.Description,
                ShowOnMenu = x.ShowOnMenu,
                PostCount = x.Posts.Count(p => p.Published)
            });
        return await categoriesQuery.ToPagedListAsync(
            pageNumber, pageSize,
            nameof(Category.Name), "DESC",
            cancellationToken);
    }

    public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
          IPagingParams pagingParams,
          string name = null,
          CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(name),
                x => x.Name.Contains(name))
            .Select(a => new CategoryItem()
            {
                Id = a.Id,
                Name = a.Name,
                Description = a.Description,
                UrlSlug = a.UrlSlug,
                ShowOnMenu = a.ShowOnMenu,
                PostCount = a.Posts.Count(p => p.Published)
            })
            .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public async Task<Category> GetCachedCategoryByIdAsync(int categoryId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"category.by-id.{categoryId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await FindCategoryByIDAsync(categoryId);
            });
    }

    public async Task<Category> GetCachedCategoryBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"category.by-slug.{slug}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await FindCategoryByUrlAsync(slug, cancellationToken);
            });
    }

    public async Task<bool> AddOrUpdateCategoryAsync(
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

    public async Task<bool> RemovePostsByIdAsync(
  int postId,
  CancellationToken cancellationToken = default)
    {
        var post = await _context.Set<Post>().FindAsync(postId);
        if (post is null) return false;
        _context.Set<Post>().Remove(post);
        var rowsCount = await _context.SaveChangesAsync(cancellationToken);
        return rowsCount > 0;
    }

    public async Task<bool> TogglePuslishedFlagAsync(
      int postId,
      CancellationToken cancellationToken = default)
    {
        var post = await _context.Set<Post>().FindAsync(postId);
        if (post is null) return false;
        post.Published = !post.Published;
        await _context.SaveChangesAsync(cancellationToken);
        return post.Published;
    }

    public async Task<bool> SetImageUrlAsync(
       int postId, string imageUrl,
       CancellationToken cancellationToken = default)
    {
        return await _context.Posts
            .Where(x => x.Id == postId)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(a => a.ImageUrl, a => imageUrl),
                cancellationToken) > 0;
    }

    public async Task<IList<PostItem>> CountPostsMonthAsync(
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

    public async Task<Post> FindPostByIDAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddOrUpdatePostAsync(
       Post postName,
       CancellationToken cancellationToken = default)
    {
        if (IsPostSlugExistedAsync(postName.Id, postName.UrlSlug).Result)
            Console.WriteLine("Error: Existed Slug");
        else

           if (postName.Id > 0)
            await _context.Set<Post>()
            .Where(p => p.Id == postName.Id)
            .ExecuteUpdateAsync(p => p
                .SetProperty(x => x.Title, x => postName.Title)
                .SetProperty(x => x.UrlSlug, x => postName.UrlSlug)
                .SetProperty(x => x.ShortDescription, x => postName.ShortDescription)
                .SetProperty(x => x.Description, x => postName.Description)

                .SetProperty(x => x.ModifiedDate, x => postName.ModifiedDate)
                .SetProperty(x => x.CategotyId, x => postName.CategotyId)
                 .SetProperty(x => x.Meta, x => postName.Meta)
                .SetProperty(x => x.ImageUrl, x => postName.ImageUrl)
                .SetProperty(x => x.ViewCount, x => postName.ViewCount)
                .SetProperty(x => x.Published, x => postName.Published)
                .SetProperty(x => x.PostedDate, x => postName.PostedDate)

                .SetProperty(x => x.Author, x => postName.Author)
                .SetProperty(x => x.AuthorId, x => postName.AuthorId)
                .SetProperty(x => x.Category, x => postName.Category)
                .SetProperty(x => x.Tags, x => postName.Tags),
                cancellationToken);
        else
        {
            _context.AddRange(postName);
            _context.SaveChanges();
        }
    }

    public async Task<bool> AddOrUpdatePostsAsync(
           Post post,
           CancellationToken cancellationToken = default)
    {
        if (post.Id > 0)
        {
            _context.Posts.Update(post);
            _memoryCache.Remove($"post.by-id.{post.Id}");
        }
        else
        {
            _context.Posts.Add(post);
        }

        return await _context.SaveChangesAsync(cancellationToken) > 0;
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

    public async Task PublishedAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<Post>()
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p
                    .SetProperty(p => p.Published, p => !p.Published),
                cancellationToken);
    }

    public async Task<IList<Post>> GetRandomPostsAsync(
        int n,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .OrderBy(x => Guid.NewGuid())
            .Take(n)
            .ToListAsync();
    }

    public async Task<IList<Post>> GetAllPostsByPostQuery(
        PostQuery pquery, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(c => c.Category)
            .Include(c => c.Tags)
            .WhereIf(pquery.AuthorId > 0, p => p.AuthorId == pquery.AuthorId)
            .WhereIf(pquery.PostId > 0, p => p.Id == pquery.PostId)
            .WhereIf(pquery.CategoryId > 0, p => p.CategotyId == pquery.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(pquery.CategorySlug), p => p.Category.UrlSlug ==
            pquery.CategorySlug)
            .WhereIf(pquery.PostedYear > 0, p => p.PostedDate.Year == pquery.PostedYear)
            .WhereIf(pquery.PostedMonth > 0, p => p.PostedDate.Month == pquery.PostedMonth)
            .WhereIf(pquery.TagId > 0, p => p.Tags.Any(x => x.Id == pquery.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(pquery.TagSlug), p => p.Tags.Any(x => x.UrlSlug ==
            pquery.TagSlug))
            .ToListAsync(cancellationToken);
    }

    public async Task<IList<Post>> FindAllPostsWithPostQueryAsync(PostQuery pq, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
                .Include(c => c.Category)
                .Include(t => t.Tags)
                .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
                .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
                .WhereIf(pq.CategoryId > 0, p => p.Id == pq.CategoryId)
                .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
                .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
                .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
                .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
                .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
                .ToListAsync(cancellationToken);
    }

    public async Task<int> CountPostsWithPostQueryAsync(PostQuery pq,
             CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .Include(c => c.Category)
            .Include(t => t.Tags)
            .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
            .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
            .WhereIf(pq.CategoryId > 0, p => p.Id == pq.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
            .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
            .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
            .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
            .CountAsync(cancellationToken);
    }

    public async Task<IPagedList<Post>> GetPagedsPostAsync(
        PostQuery pq, IPagingParams pagingParams,
        CancellationToken cancellationToken = default)
    {
        return await FilterPost(pq)
                .ToPagedListAsync(pagingParams, cancellationToken);
    }

    public IQueryable<Post> FilterPost(PostQuery pq)
    {
        var query = _context.Set<Post>()
            .Include(c => c.Category)
            .Include(t => t.Tags)
            .Include(a => a.Author);
        return query
            .WhereIf(pq.AuthorId > 0, p => p.AuthorId == pq.AuthorId)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.AuthorSlug), p => p.Author.UrlSlug == pq.AuthorSlug)
            .WhereIf(pq.PostId > 0, p => p.Id == pq.PostId)
            .WhereIf(pq.CategoryId > 0, p => p.CategotyId == pq.CategoryId)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.CategorySlug), p => p.Category.UrlSlug == pq.CategorySlug)
            .WhereIf(pq.PostedYear > 0, p => p.PostedDate.Year == pq.PostedYear)
            .WhereIf(pq.PostedMonth > 0, p => p.PostedDate.Month == pq.PostedMonth)
            .WhereIf(pq.TagId > 0, p => p.Tags.Any(x => x.Id == pq.TagId))
            .WhereIf(!string.IsNullOrWhiteSpace(pq.TagSlug), p => p.Tags.Any(x => x.UrlSlug == pq.TagSlug))
            .WhereIf(pq.PublishedOnly, p => p.Published == pq.PublishedOnly)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.PostSlug), p => p.UrlSlug == pq.PostSlug)
            .WhereIf(!string.IsNullOrWhiteSpace(pq.KeyWord), p => p.Title.Contains(pq.KeyWord) ||
                    p.ShortDescription.Contains(pq.KeyWord) ||
                    p.Description.Contains(pq.KeyWord) ||
                    p.Category.Name.Contains(pq.KeyWord) ||
                    p.Tags.Any(t => t.Name.Contains(pq.KeyWord)));
    }

    public async Task<IPagedList<T>> GetPagedPostsAsync<T>(
   PostQuery condition,
   IPagingParams pagingParams,
   Func<IQueryable<Post>, IQueryable<T>> mapper)
    {
        var posts = FilterPost(condition);
        var projectedPosts = mapper(posts);

        return await projectedPosts.ToPagedListAsync(pagingParams);
    }

    public async Task<IPagedList<Post>> GetPagedPostAsync(
            PostQuery pq,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
    {
        return await FilterPost(pq)
            .ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Post.PostedDate), "DESC",
                cancellationToken);
    }

    public async Task<IPagedList<T>> GetPagedPostQueryAsync<T>(
            PostQuery pq,
            Func<IQueryable<Post>, IQueryable<T>> mapper,
            CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Post> GetCachedPostByIdAsync(int postId)
    {
        return await _memoryCache.GetOrCreateAsync(
            $"post.by-id.{postId}",
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                return await FindPostByIDAsync(postId);
            });
    }

    public async Task<int> CountPostAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountUnPublicPostAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Post>()
            .CountAsync(x => !x.Published, cancellationToken);
    }

    public async Task<int> CountCategoryAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .CountAsync(cancellationToken);
    }

    public Task IsCategorySlugExistedAsync(Category category2)
    {
        throw new NotImplementedException();
    }
}