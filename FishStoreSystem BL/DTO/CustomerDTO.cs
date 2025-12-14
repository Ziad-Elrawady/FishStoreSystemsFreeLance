namespace FishStoreSystem.BL.DTO
{
    public class CustomerDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public decimal? CreditLimit { get; set; }
        public string? Status { get; set; }
        public decimal TotalDebt { get; set; }
    }

}
