using FluentValidation;
using Microsoft.AspNetCore.Rewrite;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations
{
    public class AuthorValidator : AbstractValidator<AuthorEditModel>
    {
        public AuthorValidator() 
        {
            RuleFor(a => a.FullName)
                .NotEmpty()
                .WithMessage("Ten tac gia khong duoc de trong")
                .MaximumLength(100)
                .WithMessage("Ten Tac gia toi da 100 ky tu");
            
            RuleFor(a => a.UrlSlug)
                .NotEmpty()
                .WithMessage("UrlSlug khong duoc de trong")
                .MaximumLength(100)
                .WithMessage("UrlSlug toi da 100 ky tu");

            RuleFor(a => a.JoinedDate)
                .GreaterThan(DateTime.MinValue)
                .WithMessage("Ngay tham gia khong hop le");

            RuleFor(a => a.Email)
                .NotEmpty()
                .WithMessage("Email khong duoc de trong")
                .MaximumLength(100)
                .WithMessage("Email toi da 100 ky tu");

            RuleFor(a => a.Notes)
                .MaximumLength(500)
                .WithMessage("Ghi chu toi da 500 ky tu");
        }
    }
}
