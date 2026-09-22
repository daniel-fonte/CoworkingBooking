using CoworkingBooking.Application.WorkspaceCalendar.Dtos;
using CoworkingBooking.Shared.Utils;
using FluentValidation;

namespace CoworkingBooking.Application.WorkspaceCalendar.Validators
{
    public class CreateWorkspaceCalendarBookingValidator : AbstractValidator<CreateWorkspaceCalendarBookingRequestDTO>
    {
        public CreateWorkspaceCalendarBookingValidator()
        {
            RuleFor(workspaceCalendarBooking => workspaceCalendarBooking.StartAt)
                .NotEmpty()
                .Must(DatesUtils.BeAValidIsoString);

            RuleFor(workspaceCalendarBooking => workspaceCalendarBooking.EndAt)
                .NotEmpty()
                .Must(DatesUtils.BeAValidIsoString);
        }
    }
}