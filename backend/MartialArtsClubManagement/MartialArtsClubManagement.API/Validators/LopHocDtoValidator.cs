using FluentValidation;
using MartialArtsClubManagement.API.DTOs;

namespace MartialArtsClubManagement.API.Validators
{
    public class LopHocDtoValidator : AbstractValidator<LopHocDto>
    {
        public LopHocDtoValidator()
        {
            RuleFor(x => x.TenLop).NotEmpty().MaximumLength(100);
            RuleFor(x => x.HocPhi).GreaterThanOrEqualTo(0);
            // Add other business rules as needed
        }
    }
}
