using FluentValidation;
using WealthOS.Application.Properties.DTOs.Requests;

namespace WealthOS.Application.Properties.Validators;

public sealed class PropertyAddressRequestValidator : AbstractValidator<PropertyAddressRequest>
{
    public PropertyAddressRequestValidator()
    {
        RuleFor(request => request.Line1).MaximumLength(256);
        RuleFor(request => request.Line2).MaximumLength(256);
        RuleFor(request => request.Locality).MaximumLength(128);
        RuleFor(request => request.City).MaximumLength(128);
        RuleFor(request => request.State).MaximumLength(128);
        RuleFor(request => request.PostalCode).MaximumLength(32);
        RuleFor(request => request.Country).MaximumLength(128);
        RuleFor(request => request.FullAddress).MaximumLength(512);
        RuleFor(request => request.GoogleMapsUrl).MaximumLength(1024);

        RuleFor(request => request.Latitude)
            .InclusiveBetween(-90m, 90m)
            .When(request => request.Latitude.HasValue);

        RuleFor(request => request.Longitude)
            .InclusiveBetween(-180m, 180m)
            .When(request => request.Longitude.HasValue);
    }
}

public sealed class PropertyOwnerRequestValidator : AbstractValidator<PropertyOwnerRequest>
{
    public PropertyOwnerRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.OwnershipPercentage)
            .InclusiveBetween(0.01m, 100m);

        RuleFor(request => request.OwnershipType)
            .IsInEnum();
    }
}

public sealed class CreatePropertyRequestValidator : AbstractValidator<CreatePropertyRequest>
{
    public CreatePropertyRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Type).IsInEnum();
        RuleFor(request => request.OwnershipType).IsInEnum();
        RuleFor(request => request.Status).IsInEnum();

        RuleFor(request => request.PurchasePrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.CurrentMarketValue)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.Area)
            .GreaterThan(0)
            .When(request => request.Area.HasValue);

        RuleFor(request => request.BuiltUpArea)
            .GreaterThan(0)
            .When(request => request.BuiltUpArea.HasValue);

        RuleFor(request => request.Floor).MaximumLength(64);
        RuleFor(request => request.Facing).MaximumLength(64);
        RuleFor(request => request.Description).MaximumLength(4000);
        RuleFor(request => request.Notes).MaximumLength(4000);

        RuleFor(request => request.Bedrooms)
            .InclusiveBetween(0, 100)
            .When(request => request.Bedrooms.HasValue);

        RuleFor(request => request.Bathrooms)
            .InclusiveBetween(0, 100)
            .When(request => request.Bathrooms.HasValue);

        RuleFor(request => request.Parking)
            .InclusiveBetween(0, 100)
            .When(request => request.Parking.HasValue);

        RuleFor(request => request.Address)
            .SetValidator(new PropertyAddressRequestValidator()!)
            .When(request => request.Address is not null);

        RuleForEach(request => request.Owners)
            .SetValidator(new PropertyOwnerRequestValidator())
            .When(request => request.Owners is not null);

        RuleFor(request => request.Owners)
            .Must(owners => owners is null || owners.Sum(owner => owner.OwnershipPercentage) <= 100.01m)
            .WithMessage("Total ownership percentage cannot exceed 100.")
            .When(request => request.Owners is not null && request.Owners.Count > 0);
    }
}

public sealed class UpdatePropertyRequestValidator : AbstractValidator<UpdatePropertyRequest>
{
    public UpdatePropertyRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(request => request.Type).IsInEnum();
        RuleFor(request => request.OwnershipType).IsInEnum();
        RuleFor(request => request.Status).IsInEnum();

        RuleFor(request => request.PurchasePrice)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.CurrentMarketValue)
            .GreaterThanOrEqualTo(0);

        RuleFor(request => request.CurrencyCode)
            .NotEmpty()
            .Length(3);

        RuleFor(request => request.Area)
            .GreaterThan(0)
            .When(request => request.Area.HasValue);

        RuleFor(request => request.BuiltUpArea)
            .GreaterThan(0)
            .When(request => request.BuiltUpArea.HasValue);

        RuleFor(request => request.Floor).MaximumLength(64);
        RuleFor(request => request.Facing).MaximumLength(64);
        RuleFor(request => request.Description).MaximumLength(4000);
        RuleFor(request => request.Notes).MaximumLength(4000);

        RuleFor(request => request.Bedrooms)
            .InclusiveBetween(0, 100)
            .When(request => request.Bedrooms.HasValue);

        RuleFor(request => request.Bathrooms)
            .InclusiveBetween(0, 100)
            .When(request => request.Bathrooms.HasValue);

        RuleFor(request => request.Parking)
            .InclusiveBetween(0, 100)
            .When(request => request.Parking.HasValue);

        RuleFor(request => request.Address)
            .SetValidator(new PropertyAddressRequestValidator()!)
            .When(request => request.Address is not null);

        RuleForEach(request => request.Owners)
            .SetValidator(new PropertyOwnerRequestValidator())
            .When(request => request.Owners is not null);

        RuleFor(request => request.Owners)
            .Must(owners => owners is null || owners.Sum(owner => owner.OwnershipPercentage) <= 100.01m)
            .WithMessage("Total ownership percentage cannot exceed 100.")
            .When(request => request.Owners is not null && request.Owners.Count > 0);
    }
}

public sealed class GetAllPropertiesQueryValidator
    : AbstractValidator<Queries.GetAllPropertiesQuery>
{
    public GetAllPropertiesQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThanOrEqualTo(1);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Search).MaximumLength(200);
        RuleFor(query => query.Status).IsInEnum().When(query => query.Status.HasValue);
        RuleFor(query => query.Type).IsInEnum().When(query => query.Type.HasValue);
    }
}
