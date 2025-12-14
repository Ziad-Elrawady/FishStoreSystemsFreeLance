using System.ComponentModel.DataAnnotations;

namespace FishStoreSystem.DAL.Entities
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }

        [Required(ErrorMessage = "اسم الصنف مطلوب")]
        [Display(Name = "اسم الصنف")]
        public string? ItemName { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Display(Name = "الكمية")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "سعر الصنف مطلوب")]
        [Display(Name = "(سعرالصنف (الكيلو ")]
        public decimal UnitPrice { get; set; }

        public Invoice? Invoice { get; set; }

        [Display(Name = "الإجمالي")]
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
