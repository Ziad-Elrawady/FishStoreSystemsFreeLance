using AutoMapper;
using FishStoreSystem.BL.DTO;
using FishStoreSystem.DAL.Entities;

namespace FishStoreSystem_BL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.TotalDebt, opt => opt.MapFrom(src => src.Invoices.Sum(i => i.TotalAmount - i.PaidAmount)))
                .ReverseMap();  // <-- أضف ReverseMap

            CreateMap<Invoice, InvoiceDTO>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ReverseMap();  // <-- أضف ReverseMap لكن بدون Customer

            CreateMap<InvoiceItem, InvoiceItemDTO>().ReverseMap();  // <-- ReverseMap يفيد كتير

            CreateMap<Payment, PaymentDTO>().ReverseMap();
        }
    }
}
