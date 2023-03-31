using FluentValidation;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations;

public class CategoryValidator : AbstractValidator<CategoryEditModel>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Ten chu de khong duoc de trong")
            .MaximumLength(100)
            .WithMessage("Ten chu de toi da 100 ky tu");


        RuleFor(c => c.UrlSlug)
            .NotEmpty()
            .WithMessage("UrlSlug khong duoc de trong")
            .MaximumLength(100)
            .WithMessage("UrlSlug toi da 100 ky tu");


        RuleFor(c => c.Description)
            .MaximumLength(1000)
            .WithMessage("noi dung toi da 1000 ly tu");


    }
}