using AutoMapper;
using TravelAgency.Models;
using TravelAgency.ViewModels;

public class TravelOfferingsProfile : Profile
{
    public TravelOfferingsProfile()
    {
        CreateMap<TravelOfferingViewModel, TravelOffering>();
        CreateMap<TravelOffering, TravelOfferingViewModel>();
    }
}
