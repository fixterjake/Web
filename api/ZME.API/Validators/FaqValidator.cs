using FluentValidation;
using ZME.API.Shared.Models;

namespace ZME.API.Validators;

public class FaqValidator : AbstractValidator<Faq>
{
    public FaqValidator()
    {
        RuleFor(x => x.Question).NotEmpty();
        RuleFor(x => x.Answer).NotEmpty();
    }
}
