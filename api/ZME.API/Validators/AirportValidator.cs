using FluentValidation;
using ZME.API.Shared.Models;

namespace ZME.API.Validators;

public class AirportValidator : AbstractValidator<Airport>
{
    public AirportValidator()
    {
        RuleFor(x => x.Icao).NotEmpty().Length(4);
        RuleFor(x => x.Name).NotEmpty();
    }
}
