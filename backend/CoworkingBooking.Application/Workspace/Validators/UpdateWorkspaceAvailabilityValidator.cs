using CoworkingBooking.Application.Workspace.Dtos;
using CoworkingBooking.Shared.Enums;
using CoworkingBooking.Shared.Utils;
using FluentValidation;

namespace CoworkingBooking.Application.Workspace.Validators
{
    public class UpdateWorkspaceAvailabilityValidator: AbstractValidator<UpdateWorkspaceAvailabilityRequestDTO>
    {
        public UpdateWorkspaceAvailabilityValidator()
        {
            RuleFor(availability => availability.StartAt)
                .NotEmpty()
                .Must(DatesUtils.BeAValidIsoString);

            RuleFor(availability => availability.EndAt)
                .NotEmpty()
                .Must(DatesUtils.BeAValidIsoString);

            RuleFor(availability => availability.Frequency)
                .NotEmpty()
                .IsInEnum();

            RuleFor(availability => availability.Timezone)
                .NotEmpty();

            RuleFor(availability => availability.Until)
                    .NotEmpty()
                    .Must(DatesUtils.BeAValidIsoString);

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
    }
}