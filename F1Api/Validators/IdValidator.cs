using FluentValidation;
using F1Api.Models;

namespace F1Api.Validators
{
    public class IdValidator : AbstractValidator<IdRequest>
    {
        public IdValidator()
        {
            RuleFor(request => request.Id)
                .GreaterThan(0)
                .WithMessage("ID must be greater than 0");
        }
    }
}