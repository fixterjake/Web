using FluentValidation;
using ZME.API.Shared.Models;

namespace ZME.API.Validators;

public class EventRegistrationValidator : AbstractValidator<EventRegistration>
{
    public EventRegistrationValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.EventPositionId).NotEmpty();
        RuleFor(x => x.Start).NotEmpty();
        RuleFor(x => x.End).NotEmpty();
    }
}
