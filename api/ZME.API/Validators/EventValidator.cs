using FluentValidation;
using ZME.API.Shared.Models;

namespace ZME.API.Validators;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Host).NotEmpty();
    }
}
