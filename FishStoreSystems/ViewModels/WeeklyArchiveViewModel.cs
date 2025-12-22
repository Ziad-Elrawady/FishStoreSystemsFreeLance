namespace FishStoreSystems.ViewModels
{
    public class WeeklyArchiveViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<WeeklyDayViewModel> Days { get; set; } = new();

        public decimal TotalNetProfit => Days.Sum(d => d.NetProfit);
        public decimal TotalPaymentsReceived => Days.Sum(d => d.PaymentsReceived);
        public decimal TotalExpenses => Days.Sum(d => d.Expenses);
        public decimal TotalReceivables => Days.Sum(d => d.Receivables);
    }
}

