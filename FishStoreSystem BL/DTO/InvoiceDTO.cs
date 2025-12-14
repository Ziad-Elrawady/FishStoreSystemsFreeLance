namespace FishStoreSystem.BL.DTO
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string? Status { get; set; }
        public List<InvoiceItemDTO> Items { get; set; } = new();
        public List<PaymentDTO> Payments { get; set; } = new();
    }
}
