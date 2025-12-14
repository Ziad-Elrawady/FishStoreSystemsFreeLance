using System.ComponentModel.DataAnnotations;

namespace FishStoreSystem.BL.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }

        [Display(Name = "الاسم")]
        public string? Name { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        [Display(Name = "حد التنبيه")]
        public decimal DebtWarningLimit { get; set; } = 1000;

        [Display(Name = "الحالة")]
        public string? Status { get; set; }
        public decimal TotalDebt { get; set; }
    }

}
