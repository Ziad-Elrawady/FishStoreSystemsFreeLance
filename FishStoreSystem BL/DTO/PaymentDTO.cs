namespace FishStoreSystem.BL.DTO
{
    public class PaymentDTO
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
