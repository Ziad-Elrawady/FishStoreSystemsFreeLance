namespace FishStoreSystems.ViewModels
{
    public class WeeklyDayViewModel
    {
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public decimal PaymentsReceived { get; set; }
        public decimal Expenses { get; set; }
        public decimal Receivables { get; set; }
    }
}
