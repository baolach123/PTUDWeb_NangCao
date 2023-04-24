using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.Constants;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;
using TatBlog.WebApp.Validations;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
    public class PostsController : Controller
    {
        
        private readonly ILogger<PostsController> _logger;
        private readonly IBlogRepository _blogRepository;
        private readonly IMediaManager _mediaManager;
        private readonly IMapper _mapper;
        private readonly IValidator<PostEditModel> _postValidator;
        private readonly AuthorRepository _authorRepository;
        public PostsController(
            IBlogRepository blogRepository,
            IMapper mapper, IMediaManager mediaManager, 
            ILogger<PostsController> logger,
            IValidator<PostEditModel> postValidator)
        {
            _postValidator = postValidator;
            _mediaManager = mediaManager;
            _blogRepository = blogRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index(PostFilterModel model,
            [FromQuery(Name ="p")]int pageNumber=1,
            [FromQuery(Name ="ps")]int pageSize=10)
        {



            var postQuery = _mapper.Map<PostQuery>(model);




            ViewBag.PostsList=await _blogRepository
                .GetAllPostsByPostQuery(postQuery);

            await PopulatePostFilterModelAsync(model);
            return View(model);
        }

        public async Task<IActionResult> Delete(int id = 0)
        {
            await _blogRepository.RemovePostsByIdAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Toggle(int id = 0)
        {
            await _blogRepository.TogglePuslishedFlagAsync(id);
            return RedirectToAction(nameof(Index));
        }


        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
        {
            var authors = await _authorRepository.GetAuthorsAsync();
            var categories = await _blogRepository.GetCategoriesAsync();

            model.AuthorList = authors.Select(c => new SelectListItem()
            {
                Text = c.FullName,
                Value = c.Id.ToString()
            });


            model.CategoryList=categories.Select(c => new SelectListItem()
            { Text = c.Name,
            Value = c.Id.ToString() });
        }

        private async Task PopulatePostEditModelAsync(PostEditModel model)
        {
            var authors = await _authorRepository.GetAuthorsAsync();
            var categories = await _blogRepository.GetCategoriesAsync();

            model.AuthorList = authors.Select(c => new SelectListItem()
            {
                Text = c.FullName,
                Value = c.Id.ToString()
            });


            model.CategoryList = categories.Select(c => new SelectListItem()
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }


        [HttpGet]

        public async Task<IActionResult> Edit(int id = 0)
        {
            var post = id > 0
                ? await _blogRepository.GetPostByIdAsync(id, true)
                : null;

            var model = post == null
                ? new PostEditModel()
                : _mapper.Map<PostEditModel>(post);

            await PopulatePostEditModelAsync(model);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(
          
            PostEditModel model)
        {
            var validationResult = await _postValidator.ValidateAsync(model);

            if(!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }

            if (!ModelState.IsValid)
            {
                await PopulatePostEditModelAsync(model);
                //return View(model);
            }

            var post = model.Id>0
                ?await _blogRepository.GetPostByIdAsync(model.Id)
                : null;

            if(post == null)
            {
                post = _mapper.Map<Post>(model);
                post.CategotyId = model.CategoryId;
                post.Id = 0;
                post.PostedDate=DateTime.Now;
            }
            else
            {
                _mapper.Map(model, post);
                post.Category = null;
                post.ModifiedDate = DateTime.Now;
            }

            if(model.ImageFile?.Length > 0)
            {
                var newImagePath=await _mediaManager.SaveFileAsync(
                    model.ImageFile.OpenReadStream(),
                    model.ImageFile.FileName,
                    model.ImageFile.ContentType);


                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    await _mediaManager.DeleteFileAsync(post.ImageUrl);
                    post.ImageUrl=newImagePath;
                }
            }


            await _blogRepository.CreateOrUpdatePostAsync(post, model.GetSelectedTags());

            return RedirectToAction(nameof(Index));
            
        }

        [HttpPost]

        public async Task<IActionResult> VerifyPostSlug(
            int id, string urlSlug)
        {
            var slugExisted= await _blogRepository
                .IsPostSlugExistedAsync(id, urlSlug);

            return slugExisted
                ? Json($"Slug '{urlSlug}' đã được sử dụng")
                : Json(true);
        }
        
        public async Task<IActionResult> Filrate(PostFilterModel model,
            [FromQuery(Name = "p")] int pageNumber = 1,
            [FromQuery(Name = "ps")] int pageSize = 10)
        {
            var postQuery = _mapper.Map<PostQuery>(model);

            ViewBag.PostsList = await _blogRepository
                .GetPagedPostAsync(postQuery, pageNumber, pageSize);

            await PopulatePostFilterModelAsync(model);

            return View("Index", model);
        }
    }  
}
