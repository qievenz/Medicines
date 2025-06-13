using FluentValidation;
using Medicines.Core.DTOs.Ingestion;
using System.Text.RegularExpressions;

namespace Medicines.Application.Validators
{
    public class MedicineJsonDtoValidator : AbstractValidator<MedicineJsonDto>
    {
        public MedicineJsonDtoValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Code is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Laboratory).NotEmpty().WithMessage("Laboratory is required.");
            RuleFor(x => x.ActiveIngredient).NotEmpty().WithMessage("ActiveIngredient is required.");

            RuleFor(x => x.ExpirationDate)
                .NotEmpty().WithMessage("ExpirationDate is required.")
                .Must(BeAValidFutureDate).WithMessage("ExpirationDate must be a valid date in the future.");

            RuleFor(x => x.Concentration)
                .Must(BeAValidConcentrationFormat).When(x => !string.IsNullOrEmpty(x.Concentration))
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
