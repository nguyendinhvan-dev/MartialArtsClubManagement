using FluentValidation;
using MartialArtsClubManagement.API.DTOs;

namespace MartialArtsClubManagement.API.Validators
{
    public class KhoaHocDtoValidator : AbstractValidator<KhoaHocDto>
    {
        public KhoaHocDtoValidator()
        {
            RuleFor(x => x.TenKhoaHoc).NotEmpty().MaximumLength(100);
            RuleFor(x => x.HocPhi).GreaterThanOrEqualTo(0);
            // Add other business rules as needed
        }
    }
}
