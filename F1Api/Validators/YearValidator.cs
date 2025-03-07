using FluentValidation;
using F1Api.Models;
using System;

namespace F1Api.Validators
{
    public class YearValidator : AbstractValidator<YearRequest>
    {
        public YearValidator()
        {
            // Set minimum to 1950
            RuleFor(request => request.Year)
                .GreaterThanOrEqualTo(1950)
                .WithMessage("Year must be at least 1950");

        }
    }
}