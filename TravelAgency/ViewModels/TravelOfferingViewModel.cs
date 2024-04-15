using FluentValidation;
using TravelAgency.Models;

namespace TravelAgency.ViewModels
{
    public class TravelOfferingViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Destination { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
    }
    public class TravelOfferingValidator : AbstractValidator<TravelOffering>
    {
        public TravelOfferingValidator()
        {
            RuleFor(offer => offer.StartDate)
                .NotEmpty().WithMessage("Data rozpoczęcia jest wymagana.")
                .LessThan(offer => offer.EndDate).WithMessage("Data rozpoczęcia musi być wcześniejsza niż data zakończenia.");

            RuleFor(offer => offer.EndDate)
                .NotEmpty().WithMessage("Data zakończenia jest wymagana.");

            RuleFor(offer => offer.Price)
                .NotEmpty().WithMessage("Cena jest wymagana.")
                .GreaterThan(0).WithMessage("Cena musi być większa od zera.");
            RuleFor(offer => offer.Name)
                .NotEmpty().WithMessage("Nazwa jest wymagana.")
                .MaximumLength(100).WithMessage("Nazwa nie może być dłuższa niż 100 znaków.");
            RuleFor(offer => offer.Destination)
                .NotEmpty().WithMessage("Miejsce docelowe jest wymagane.")
                .MaximumLength(100).WithMessage("Miejsce docelowe nie może być dłuższe niż 100 znaków.");
            RuleFor(offer => offer.Description)
                .MaximumLength(500).WithMessage("Opis nie może być dłuższy niż 500 znaków.");
        }
    }
}
