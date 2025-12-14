using System.ComponentModel.DataAnnotations;

namespace FishStoreSystem.DAL.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        [Display(Name = "الزبون")]
        public int CustomerId { get; set; }

        [Display(Name = "تاريخ الفاتورة")]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Display(Name = "تاريخ الاستحقاق")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Display(Name = "المبلغ الإجمالي")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "المبلغ المدفوع")]
        public decimal PaidAmount { get; set; } = 0;

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "جديدة";

        public Customer? Customer { get; set; }
        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [Display(Name = "المتبقي")]
        public decimal RemainingAmount => TotalAmount - PaidAmount;
    }
}
