namespace FishStoreSystems.ViewModels
{
    public class WeeklyArchiveViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<WeeklyDayViewModel> Days { get; set; } = new();

        // مجموع الأسبوع
        public decimal TotalSales => Days.Sum(d => d.Sales);
        public decimal TotalPaymentsReceived => Days.Sum(d => d.PaymentsReceived);
        public decimal TotalExpenses => Days.Sum(d => d.Expenses);
        public decimal TotalReceivables => Days.Sum(d => d.Receivables);
    }
}
