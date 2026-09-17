using System.Reflection.Metadata;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Shared;
using CoworkingBooking.Shared.Classes;
using FluentValidation;

namespace CoworkingBooking.Application.Workspace.Validators
{
    public class CreateWorkspaceValidator: AbstractValidator<CreateWorkspaceRequestDTO>
    {
        public CreateWorkspaceValidator()
        {
            RuleFor(workspace => workspace.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(workspace => workspace.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(workspace => workspace.Type)
                .IsInEnum();

            RuleFor(workspace => workspace.Coordinates)
                .NotEmpty()
                .Must(value => value.GetType() == typeof(Coordinates))
                .Must(CoordinatesValidator.IsValidCoordinates)
                .WithMessage("Invalid Coordinates values");
                

            RuleFor(workspace => workspace.PricePerHour)
                .GreaterThanOrEqualTo(0);
        }

        
    }
}