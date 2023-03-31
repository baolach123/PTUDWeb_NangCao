using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Models;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.InteropServices;
using TatBlog.Core.Collections;
using TatBlog.Core.Constants;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using TatBlog.WebApi.Filters;

namespace TatBlog.WebApi.Endpoints
{
    public static class AuthorEndpoints
    {
        public static WebApplication MapAuthorEndPoints(
            this WebApplication app)
        {
            var routeGroupBuilter = app.MapGroup("/api/authors");
            routeGroupBuilter.MapGet("/", GetAuthors)
                .WithName("GetAuthors")
                .Produces<PaginationResult<AuthorItem>>();

            routeGroupBuilter.MapGet("/{id:int}", GetAuthorDetails)
                .WithName("GetAuthorById")
                .Produces<AuthorItem>()
                .Produces (404);

            routeGroupBuilter.MapGet(
                "/{slug:regex(^[a-z0-9-]+$)}/posts",
                GetPostsByAuthorSlug)
                .WithName("GetPostsByAuthorSlug")
                .Produces<PaginationResult<PostDto>>();

            routeGroupBuilter.MapPost("/", AddAuthor)
                .WithName("AddAuthor")
                .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                .Produces(201)
                .Produces(400)
                .Produces(409);

            routeGroupBuilter.MapPost("/{id:int}/avatar", SetAuthorPicture)
                .WithName("SetAuthorPicture")
                .Accepts<IFormFile>("multipart/form-data")
                .Produces<string>()
                .Produces(400);
               
            
            routeGroupBuilter.MapPut("/{id:int}", UpdateAuthor)
                .WithName("UpdateAuthor")
                .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
                .Produces(204)
                .Produces(400)
                .Produces(409);

            routeGroupBuilter.MapDelete("/{id:int}", DeleteAuthor)
                .WithName("DeleteAuthor")
                .Produces(204)
                .Produces(404);

            return app;
        }

        public static async Task<IResult> GetAuthors(           
            [AsParameters] AuthorFilterModel model,
            IAuthorRepository authorRepository)
        {
            var authorsList = await authorRepository.GetPagedAuthorsAsync(model, model.Name);
            var paginationResult = new PaginationResult<AuthorItem>(authorsList);
            return Results.Ok(paginationResult);
        }

        private static async Task<IResult> GetAuthorDetails(
            int id,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            var author = await authorRepository.GetCachedAuthorByIdAsync(id);
            return author ==null
                ?Results.NotFound($"Khong tim thay tac gia co ma so {id}")
                : Results.Ok(mapper.Map<AuthorItem>(author)); 
        }


        private static async Task<IResult> GetPostsByAuthorId(
            int id,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                AuthorId = id,
                PublishedOnly = true
            };
             var postList = await blogRepository.GetPagedPostsAsync(
                 postQuery,pagingModel,
                 posts=>posts.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(postList);

            return Results.Ok(paginationResult);
        }
        
        
        
        private static async Task<IResult> GetPostsByAuthorSlug(
            [FromRoute] string slug,
            [AsParameters] PagingModel pagingModel,
            IBlogRepository blogRepository)
        {
            var postQuery = new PostQuery()
            {
                AuthorSlug = slug,
                PublishedOnly = true
            };
             var postList = await blogRepository.GetPagedPostsAsync(
                 postQuery,pagingModel,
                 posts=>posts.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(postList);

            return Results.Ok(paginationResult);
        }

        private static  async Task<IResult> AddAuthor(
            AuthorEditModel model,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {

            if(await authorRepository
                .IsAuthorSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict(
                    $"Slug'{model.UrlSlug}' da duoc su dung");
            }

            var author = mapper.Map<Author>(model);
            await authorRepository.AddOrUpdateAsync(author);

            return Results.CreatedAtRoute(
                "GetAuthorById", new { author.Id},
                mapper.Map<AuthorItem>(author));
        }


        private static async Task<IResult> SetAuthorPicture(
             int id, IFormFile imageFile,
             IAuthorRepository authorRepository,
             IMediaManager mediaManager)
        {
            var imageUrl= await mediaManager.SaveFileAsync(
                imageFile.OpenReadStream(),
                imageFile.FileName, imageFile.ContentType );

            if(string.IsNullOrWhiteSpace(imageUrl))
            {
                return Results.BadRequest("Khong luu duoc tap tin");
            }

            await  authorRepository.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(imageUrl);
        }

        private static async Task<IResult> UpdateAuthor(
            int id,
            AuthorEditModel model,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            if (await authorRepository
                .IsAuthorSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict(
                    $"Slug'{model.UrlSlug}' da duoc su dung");
            }

            var author = mapper.Map<Author>(model);
            author.Id = id;

            return await authorRepository.DeleteAuthorAsync(id)
                ? Results.NoContent()
                : Results.NotFound();
        }

        private static async Task<IResult> DeleteAuthor(
            int id, IAuthorRepository authorRepository)
        {
            return await authorRepository.DeleteAuthorAsync(id)
                ? Results.NoContent()
                : Results.NotFound($"Could not find author with id {id}");
        }




    }
}
