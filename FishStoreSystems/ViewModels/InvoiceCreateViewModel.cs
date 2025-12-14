using Microsoft.AspNetCore.Mvc.Rendering;
using FishStoreSystem.BL.DTO;

namespace FishStoreSystem.ViewModels
{
    public class InvoiceCreateViewModel
    {
        public int CustomerId { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
        public List<InvoiceItemDTO> Items { get; set; } = new();
        public SelectList? CustomersList { get; set; }
    }
}
