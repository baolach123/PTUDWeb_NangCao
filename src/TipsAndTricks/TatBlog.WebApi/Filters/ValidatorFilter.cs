using FluentValidation;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Filters
{
    public class ValidatorFilter<T> : IEndpointFilter where T : class
    {
        public readonly IValidator<T> _validator;

        public ValidatorFilter(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async ValueTask<object> InvokeAsync(
            EndpointFilterInvocationContext context,
            EndpointFilterDelegate next)
        {
            var model = context.Arguments
                .SingleOrDefault(x=>x?.GetType()==typeof(T)) as T;
            if (model == null)
            {
                return Results.BadRequest(
                    new ValidationFailureResponse(new[]
                    {
                        "could not create model object"
                    }));
            }
                var validationResults = await _validator.ValidateAsync(model);

            if(!validationResults.IsValid)
            {
                return Results.BadRequest(
                    validationResults.Errors.ToResponse() );
            }

            return await next(context);
        }
    }
}
