using Mapster;
using TatBlog.Core.Entities;
using TatBlog.Core.DTO;
using TatBlog.WebApp.Areas.Admin.Models;
using TatBlog.Core.Constants;

namespace TatBlog.WebApp.Mapster
{
    public class MapsterConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Post, PostItem>()
                .Map(dest => dest.Category, src => src.Category.Name)
                .Map(dest => dest.Tags, src => src.Tags.Select(x => x.Name));

            config.NewConfig<PostFilterModel, PostQuery>()
                .Map(dest => dest.PublishedOnly, src => !src.NotPublished)
                .Map(dest => dest.NotPublished, src => src.NotPublished);

            config.NewConfig<PostEditModel, Post>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.ImageUrl);

            config.NewConfig<Post, PostEditModel>()
                .Map(dest => dest.SelectedTags, src => string.Join("\r\n", src.Tags.Select(x => x.Name)))
                .Ignore(dest => dest.CategoryList)
                .Ignore(dest => dest.AuthorList)
                .Ignore(dest => dest.ImageFile);
        }
    }
}
