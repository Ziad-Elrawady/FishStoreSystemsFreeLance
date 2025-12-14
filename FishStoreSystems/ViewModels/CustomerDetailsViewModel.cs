using System.Collections.Generic;
using FishStoreSystem.BL.DTO;

namespace FishStoreSystem.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public CustomerDTO? Customer { get; set; }
        public List<InvoiceDTO>? OpenInvoices { get; set; }
        public decimal TotalDebt { get; set; }
    }
}