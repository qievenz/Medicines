using FluentValidation;
using Medicines.Core.DTOs.Ingestion;
using System.Text.RegularExpressions;

namespace Medicines.Application.Validators
{
    public class MedicineCsvDtoValidator : AbstractValidator<MedicineCsvDto>
    {
        public MedicineCsvDtoValidator()
        {
            RuleFor(x => x.medicine_code).NotEmpty().WithMessage("Code is required.");
            RuleFor(x => x.medicine_name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.laboratory).NotEmpty().WithMessage("Laboratory is required.");
            RuleFor(x => x.active_ingredient).NotEmpty().WithMessage("ActiveIngredient is required.");

            RuleFor(x => x.expiration_date)
                .NotEmpty().WithMessage("ExpirationDate is required.")
                .Must(BeAValidFutureDate).WithMessage("ExpirationDate must be a valid date in the future.");

            RuleFor(x => x.concentration)
                .Must(BeAValidConcentrationFormat).When(x => !string.IsNullOrEmpty(x.concentration))
                .WithMessage("Concentration format is invalid. Must be number + unit (e.g., '500mg').");
        }

        private bool BeAValidFutureDate(string dateString)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
            {
                return date >= DateTime.Today;
            }
            return false;
        }

        private bool BeAValidConcentrationFormat(string concentration)
        {
            return Regex.IsMatch(concentration, @"^\d+(\.\d+)?(mg|ml|UI|mcg|milligramos|ml/dl|unidad|g|kg|L|µg|ug)$", RegexOptions.IgnoreCase);
        }
    }
}
