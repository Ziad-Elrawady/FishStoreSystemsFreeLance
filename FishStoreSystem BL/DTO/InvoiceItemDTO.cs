namespace FishStoreSystem.BL.DTO
{
    public class InvoiceItemDTO
    {
        public int Id { get; set; }
        public string? ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
