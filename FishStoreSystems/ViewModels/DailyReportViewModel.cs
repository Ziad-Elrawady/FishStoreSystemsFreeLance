using FishStoreSystem.BL.DTO;

namespace FishStoreSystem.ViewModels
{
    public class DailyReportViewModel
    {
        public DateTime Date { get; set; }

        // 1- يدخله المستخدم
        public decimal TotalManualSales { get; set; }

        // 2- الخوارج يدخله المستخدم
        public decimal Expenses { get; set; }

        // 3- المبالغ على الزبائن تلقائي
        public decimal Receivables { get; set; }

        // 4- المدفوعات المستلمة تلقائي
        public decimal PaymentsReceived { get; set; }

        // القيم المحسوبة
        public decimal TotalCash => TotalManualSales + PaymentsReceived - Expenses;

        public List<CustomerDTO> OverdueCustomers { get; set; } = new();
        public List<InvoiceDTO> InvoicesOfDay { get; set; } = new();
    }
}
