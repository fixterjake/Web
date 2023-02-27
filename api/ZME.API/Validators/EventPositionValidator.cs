using FluentValidation;
using ZME.API.Shared.Models;

namespace ZME.API.Validators;

public class EventPositionValidator : AbstractValidator<EventPosition>
{
    public EventPositionValidator()
    {
        RuleFor(x => x.EventId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.MinRating).NotEmpty();
    }
}
