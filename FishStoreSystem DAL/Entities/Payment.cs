using System.ComponentModel.DataAnnotations;

namespace FishStoreSystem.DAL.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }

        [Display(Name = "تاريخ الدفع")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Display(Name = "المبلغ")]
        public decimal Amount { get; set; }

        [Display(Name = "طريقة الدفع")]
        public string PaymentMethod { get; set; } = "كاش";

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public Invoice? Invoice { get; set; }
    }
}
