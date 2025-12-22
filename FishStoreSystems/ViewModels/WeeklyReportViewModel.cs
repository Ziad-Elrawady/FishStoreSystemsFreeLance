using FishStoreSystem_DAL.Entities;


namespace FishStoreSystems.ViewModels
{
    public class WeeklyReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<WeeklyItem> Items { get; set; } = new();

        public decimal TotalNetProfit => Items.Sum(i => i.NetProfit);
        public decimal TotalPaymentsReceived => Items.Sum(i => i.PaymentsReceived);
        public decimal TotalExpenses => Items.Sum(i => i.Expenses);
        public decimal TotalReceivables => Items.Sum(i => i.Receivables);
    }
}

