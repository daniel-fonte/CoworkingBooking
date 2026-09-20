using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Shared.Enums;
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

            RuleFor(availability => availability.Frequency)
                .NotEmpty()
                .IsInEnum();

            RuleFor(availability => availability.Timezone)
                .NotEmpty();

            RuleFor(availability => availability.Until)
                    .NotEmpty()
                    .Must(BeAValidIsoString);

            When(availability => availability.Frequency == Frequency.DAILY, () =>
            {
                RuleFor(availability => availability.ByDay)
                    .Null();
                
                RuleFor(availability => availability.ByMonth)
                    .Null(); 
            });

            When(availability => availability.Frequency == Frequency.WEEKLY, () =>
            {
                RuleFor(availability => availability.ByDay)
                    .NotEmpty();

                RuleForEach(availability => availability.ByDay)
                    .IsInEnum();

                When(availability => availability.ByMonth != null, () =>
                {
                    RuleFor(availability => availability.ByMonth)
                        .NotEmpty();

                    RuleForEach(availability => availability.ByMonth)
                        .NotEmpty()
                        .InclusiveBetween(1, 12);
                });
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