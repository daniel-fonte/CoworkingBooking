using CoworkingBooking.Application.Workspace.Dtos;
using FluentValidation;
using System.Globalization;

namespace CoworkingBooking.Application.Workspace.Validators
{
    public class UpdateWorkspaceAvailabilityValidator: AbstractValidator<UpdateWorkspaceAvailabilityRequestDTO>
    {
        public UpdateWorkspaceAvailabilityValidator()
        {
            RuleFor(availability => availability.StartAt)
                .NotEmpty()
                .Must(BeAValidIsoString);

            RuleFor(availability => availability.EndAt)
                .NotEmpty()
                .Must(BeAValidIsoString);

            RuleFor(availability => availability.Until)
                .NotEmpty()
                .Must(BeAValidIsoString);

            RuleFor(availability => availability.Timezone)
                .NotEmpty();

            When(availability => availability.ByDay != null, () =>
            {
                RuleFor(availability => availability.ByDay)
                    .NotEmpty();

                RuleForEach(availability => availability.ByDay)
                    .IsInEnum();
            });

            When(availability => availability.ByMonth != null, () =>
            {
                RuleFor(availability => availability.ByMonth)
                    .NotEmpty();

                RuleForEach(availability => availability.ByMonth)
                    .NotEmpty()
                    .InclusiveBetween(1, 12);
            });
                
        }

        private bool BeAValidIsoString(string? dateString)
        {
            return DateTime.TryParse(
                dateString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out _
            );
        }
    }
}