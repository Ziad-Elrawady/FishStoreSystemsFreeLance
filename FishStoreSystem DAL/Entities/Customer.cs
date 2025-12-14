using System.ComponentModel.DataAnnotations;

namespace FishStoreSystem.DAL.Entities
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الزبون مطلوب")]
        [Display(Name = "الاسم")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        [Display(Name = "حد الائتمان")]
        public decimal? CreditLimit { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "جديد";

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
