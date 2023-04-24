using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.Constants;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;
using TatBlog.WebApi.Models.Posts;

namespace TatBlog.WebApi.Endpoints
{
    public static class PostEndpoints
    {
        public static WebApplication MapPostEndpoints(
       this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/posts");
            routeGroupBuilder.MapGet("/", GetPosts)
                .WithName("GetPosts")
                .Produces<ApiResponse<PaginationResult<PostDTO>>>();

            routeGroupBuilder.MapGet("/featured/{limit:int}", GetListPostFeatured)
              .WithName("GetPostFeatured")
              .Produces<ApiResponse<IList<PostDTO>>>();

            routeGroupBuilder.MapGet("/random/{limit:int}", GetListPostRandom)
              .WithName("GetPostRandom")
              .Produces<ApiResponse<IList<PostDTO>>>();

            routeGroupBuilder.MapGet("/archives/{limit:int}", GetListPostArchives)
              .WithName("GetPostArchives")
              .Produces<ApiResponse<IList<PostItem>>>();

            routeGroupBuilder.MapGet("/{id:int}", GetPostById)
                .WithName("GetPostById")
                .Produces<ApiResponse<PostDetail>>();

            routeGroupBuilder.MapGet("/{id:int}/comments", GetPostComment)
                .WithName("GetPostComment")
                .Produces<ApiResponse<IList<PostDTO>>>();

            routeGroupBuilder.MapGet(
                "/byslug/{slug:regex(^[a-z0-9 -]+$)}", GetPostsBySlug)
                .WithName("GetPostsBySlug")
                .Produces<ApiResponse<PostDetail>>();

            routeGroupBuilder.MapPost("/", AddPost)
                .WithName("AddNewPost")
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
                .Produces(401)
                .Produces<ApiResponse<PostDetail>>();
          
            routeGroupBuilder.MapPost("/{id:int}/picture", SetPostPicture)
                .WithName("SetPostPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<ApiResponse<string>>();
           
            routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
                .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
               .WithName("UpdateAnPost")
               .Produces(401)
                .Produces<ApiResponse<string>>();
      
            routeGroupBuilder.MapDelete("/{id:int}", RemovePost)
                .WithName("RemovePost")
                .Produces(401)
                .Produces<ApiResponse<string>>();

            return app;
        }

        
        public static async Task<IResult> GetPosts(
            [AsParameters] PostFilterModel model,
            IBlogRepository bolgRepository,
            IMapper mapper)
        {
            var postQuery = mapper.Map<PostQuery>(model);
            var postList = await bolgRepository
                .GetPagedPostsAsync(postQuery, model, posts => posts.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        public static async Task<IResult> GetListPostFeatured(
           int luotDoc,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.GetPopularArticlesAsync(luotDoc);
            return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"khong thay bai viet co nhieu luot doc nhat"))
                   : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostDTO>>(post)));
        }

        public static async Task<IResult> GetListPostRandom(
           int lim,
           IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.GetRandomPostsAsync(lim);
            return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"khong tim thay bai viet nao"))
                : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostDTO>>(post)));
        }

        public static async Task<IResult> GetListPostArchives(
           int soThang,
            IBlogRepository blogRepository,
            IMapper mapper)
        {
            var post = await blogRepository.CountPostsMonthAsync(soThang);
            return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"khong tim thay bai viet trong {soThang} thang gan nhat"))
                : Results.Ok(ApiResponse.Success(mapper.Map<IList<PostItem>>(post)));
        }

        public static async Task<IResult> GetPostComment(
           int id,
            IBlogRepository blogRepository)
        {           
            return Results.Ok(ApiResponse.Success(""));
        }

        
        public static async Task<IResult> GetPostById(
            int id,
            IMapper mper,
            IBlogRepository blogRepository)
        {
            var posts = await blogRepository.GetPostByIdAsync(id);

            return posts != null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"khong tim thay post id: {id}"))
               : Results.Ok(ApiResponse.Success(mper.Map<PostDetail>(posts)));
        }

        
        private static async Task<IResult> GetPostsBySlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                PostSlug = slug,
                PublishedOnly = true
            };

            var postsList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                postsList => postsList.ProjectToType<PostDTO>());

            var paginationResult = new PaginationResult<PostDTO>(postsList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }

        
        private static async Task<IResult> AddPost(
            PostEditModel model,
            IBlogRepository blogRepository,
            IMapper mapper)

        {
            if (await blogRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                    $"Slug '{model.UrlSlug}' da su dung"));
            }

            var post = mapper.Map<Post>(model);

            post.PostedDate = DateTime.Now;

            await blogRepository.AddOrUpdatePostsAsync(post);

            return Results.Ok(ApiResponse.Success(
                mapper.Map<PostQuery>(post), HttpStatusCode.Created));
        }

  
        private static async Task<IResult> SetPostPicture(
            int id, IFormFile imageFile,
            IBlogRepository blogRepository,
            IMediaManager mediaM)
        {
            var imageUrl = await mediaM.SaveFileAsync(

            imageFile.OpenReadStream(),
            imageFile.FileName, imageFile.ContentType);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, "khong luu duoc"));
            }

            await blogRepository.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(ApiResponse.Success(imageUrl));

        }

        private static async Task<IResult> UpdatePost(
            int id, PostEditModel model,
            IBlogRepository blogRepository,
           IMapper mapper)
        {          
            if (await blogRepository
                   .IsPostSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                 $"Slug '{model.UrlSlug}' Da su dung"));
            }

            var post = mapper.Map<Post>(model);
            post.Id = id;
            post.Category = null;
            post.Author = null;

            return await blogRepository.AddOrUpdatePostsAsync(post)
                  ? Results.Ok(ApiResponse.Success("da cap nhap", HttpStatusCode.NoContent))
                   : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "khong tim thay"));
        }

        private static async Task<IResult> RemovePost(
            int id, IBlogRepository blogRepository)
        {
            return await blogRepository.RemovePostsByIdAsync(id)
                 ? Results.Ok(ApiResponse.Success("da xoa", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "khong the xoa"));
        }
    }
}