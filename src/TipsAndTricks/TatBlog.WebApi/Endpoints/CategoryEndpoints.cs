using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.InteropServices;
using TatBlog.Core.Collections;
using TatBlog.Core.Constants;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class CategoryEndpoints
{
    public static WebApplication MapCategoryEndpoints(
        this WebApplication app)
    {

        var routeGroupBuilder = app.MapGroup("/api/categories");


        routeGroupBuilder.MapGet("/", GetCategories)
            .WithName("GetCategories")
            .Produces<ApiResponse<PaginationResult<CategoryItem>>>();


        routeGroupBuilder.MapGet("/{id:int}", GetCategoryDetails)
            .WithName("GetCategoryById")
            .Produces<ApiResponse<CategoryItem>>()
            .Produces(404);


        routeGroupBuilder.MapGet(
            "/{slug:regex(^[a-z0-9_-]+$)}/posts",
            GetPostsByCategorySlug)
            .WithName("GetPostsByCategorySlug")
            .Produces<ApiResponse<PaginationResult<CategoryDto>>>();

        routeGroupBuilder.MapPost("/", AddCategory)
            .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
            .WithName("AddNewCategory")
            .Produces(401)
            .Produces<ApiResponse<CategoryItem>>()
            .Produces(201)
            .Produces(400)
            .Produces(409);

        routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
            .WithName("UpdateAnCategory")
            .Produces(401)
            .Produces<ApiResponse<string>>();
        //.AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
        //.Produces(204)
        //.Produces(400)
        //.Produces(409);

        routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
            .WithName("DeleteAnCategory")
            .Produces(401)
            .Produces<ApiResponse<string>>();
        //.Produces(204)
        //.Produces(404);

        return app;
    }

    private static async Task<IResult> GetCategories(
        [AsParameters] CategoryFilterModel model,
        IBlogRepository blogRepository)
    {
        var categoriesList = await blogRepository
            .GetPagedCategoryAsync(model, model.Name);

        var paginationResult =
            new PaginationResult<CategoryItem>(categoriesList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> GetCategoryDetails(
        int id,
        IBlogRepository blogRepository,
        IMapper mapper)
    {
        var category = await blogRepository.GetCachedCategoryIdAsync(id);
        return category == null
            ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
                $"Khong tim thay category co ma {id}"))

                : Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category)));
    }

    private static async Task<IResult> GetPostsByCategorySlug(
        [FromRoute] string slug,
        [AsParameters] PagingModel pagingModel,
        IBlogRepository blogRepository)
    {
        var postQuery = new PostQuery()
        {
            CategorySlug = slug,
            PublishedOnly = true,
        };
        var categoriesList = await blogRepository.GetPagedPostsAsync(
            postQuery, pagingModel,
            categories => categories.ProjectToType<CategoryDto>());
        var paginationResult = new PaginationResult<CategoryDto>(categoriesList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

    private static async Task<IResult> AddCategory(
        CategoryEditModel model,
        IBlogRepository blogRepository,
        IMapper mapper)
    {
        if (await blogRepository
                .IsCategorySlugExistedAsync(0, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                $"Slug '{model.UrlSlug}' da duoc dung"));
        }

        var category = mapper.Map<Category>(model);
        await blogRepository.AddOrUpdateAsync(category);

        return Results.Ok(ApiResponse.Success(
            mapper.Map<CategoryItem>(category), HttpStatusCode.Created));
    }


    private static async Task<IResult> UpdateCategory(
        int id, CategoryEditModel model,
        IValidator<CategoryEditModel> validator,
        IBlogRepository blogRepository,
        IMapper mapper)
    {
        var validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            return Results.Ok(ApiResponse.Fail(
                HttpStatusCode.BadRequest, validationResult));
        }

        if (await blogRepository
                .IsCategorySlugExistedAsync(id, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict,
                $"Slug '{model.UrlSlug}' Da duoc dung"));

        }

        var category = mapper.Map<Category>(model);
        category.Id = id;

        return await blogRepository.AddOrUpdateAsync(category)
            ? Results.Ok(ApiResponse.Success("Author is update",
                      HttpStatusCode.NoContent))
            : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
                      "Could not find author"));

    }


    private static async Task<IResult> DeleteCategory(
        int id, 
        IBlogRepository blogRepository)
    {
        return await blogRepository.DeleteCategoryIdAsync(id)
            ? Results.Ok(ApiResponse.Success("Author is delered",
                      HttpStatusCode.NoContent))
            : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound,
                      "Could not find author"));
    }

}