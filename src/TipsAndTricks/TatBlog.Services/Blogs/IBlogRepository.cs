using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Constants;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
    public interface IBlogRepository
    {
        public Task<Post> GetPostAsync(
            int year,
            int month,
            string slug,
            CancellationToken cancellationToken = default);

        public Task<IList<Post>> GetPopularArticlesAsync(
            int numPosts,
            CancellationToken cancellationToken = default);

        public Task<bool> IsPostSlugExistedAsync(
            int postId, string slug,
            CancellationToken cancellationToken = default);

        public Task IncreaseViewCountAsync(
            int postId,
            CancellationToken cancellationToken = default);

        public Task<IList<CategoryItem>> GetCategoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default);

        public Task<IPagedList<TagItem>> GetPagedTagsAsync(
            IPagingParams pagingParams, CancellationToken cancellationToken = default);

        Task<Category> FindCategoryByUrlAsync(string slug, CancellationToken cancellationToken = default);

        Task<Category> FindCategoryByIDAsync(int id, CancellationToken cancellationToken = default);

        Task<Category> CreateOrUpdateCategoryAsync(
        Category category, CancellationToken cancellationToken = default);

        Task<bool> AddOrUpdateCategoryAsync(
     Category category,
     CancellationToken cancellationToken = default);

        Task<bool> DeleteCategoryByIdAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<bool> ToggleShowOnMenuFlagAsync(int categoryId, CancellationToken cancellationToken = default);

        Task<bool> IsCategorySlugExistedAsync(int categoryId, string slug, CancellationToken cancellationToken = default);

        Task<IPagedList<CategoryItem>> GetPagedCategoryAsync(
         int pageNumber = 1,
         int pageSize = 10,
         CancellationToken cancellationToken = default);

        Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
          IPagingParams pagingParams,
          string name = null,
          CancellationToken cancellationToken = default);

        Task<Category> GetCachedCategoryByIdAsync(int categoryId);

        Task<Category> GetCachedCategoryBySlugAsync(
        string slug, CancellationToken cancellationToken = default);

        Task<bool> RemovePostsByIdAsync(int postId, CancellationToken cancellationToken = default);

        Task<bool> TogglePuslishedFlagAsync(int postId, CancellationToken cancellationToken = default);

        Task<IList<PostItem>> CountPostsMonthAsync(
        int n, CancellationToken cancellationToken = default);

        Task<Post> GetPostByIdAsync(
        int postId, bool includeDetails = false,
        CancellationToken cancellationToken = default);

        Task<bool> SetImageUrlAsync(
       int postId, string imageUrl,
       CancellationToken cancellationToken = default);

        Task<bool> AddOrUpdatePostsAsync(
          Post post,
          CancellationToken cancellationToken = default);

        Task<Post> FindPostByIDAsync(int id, CancellationToken cancellationToken = default);

        Task AddOrUpdatePostAsync(Post postsName, CancellationToken cancellationToken = default);

        Task<Post> CreateOrUpdatePostAsync(
         Post post, IEnumerable<string> tags,
         CancellationToken cancellationToken = default);

        Task PublishedAsync(
        int id,
        CancellationToken cancellationToken = default);

        Task<IList<Post>> GetRandomPostsAsync(
            int n,
            CancellationToken cancellationToken = default);

        Task<IList<Post>> GetAllPostsByPostQuery(
        PostQuery pquery, CancellationToken cancellationToken = default);

        Task<IList<Post>> FindAllPostsWithPostQueryAsync(
            PostQuery pq,
            CancellationToken cancellationToken = default);

        Task<int> CountPostsWithPostQueryAsync(
            PostQuery pq,
            CancellationToken cancellationToken = default);

        Task<IPagedList<Post>> GetPagedsPostAsync(PostQuery pq,
            IPagingParams pagingParams,
            CancellationToken cancellationToken = default);

        IQueryable<Post> FilterPost(PostQuery pq);

        Task<IPagedList<T>> GetPagedPostsAsync<T>(
           PostQuery condition,
           IPagingParams pagingParams,
           Func<IQueryable<Post>, IQueryable<T>> mapper);

        Task<IPagedList<Post>> GetPagedPostAsync(
                PostQuery pq,
                int pageNumber = 1,
                int pageSize = 10,
                CancellationToken cancellationToken = default);

        Task<IPagedList<T>> GetPagedPostQueryAsync<T>(
           PostQuery pq,
           Func<IQueryable<Post>, IQueryable<T>> mapper,
           CancellationToken cancellationToken = default);

        Task<Post> GetCachedPostByIdAsync(int postId);
        Task IsCategorySlugExistedAsync(Category category2);      
    }
}