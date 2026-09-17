using System.Data;
using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Core.Workspace.Enums;
using FluentValidation;

namespace CoworkingBooking.Application.Workspace.Validators
{
    public class UpdateWorkspaceStatusValidator : AbstractValidator<UpdateWorkspaceStatusRequestDTO>
    {
        public UpdateWorkspaceStatusValidator()
        {
            RuleFor(x => x.status)
                .NotEmpty()
                .IsInEnum();
        }
    }
}