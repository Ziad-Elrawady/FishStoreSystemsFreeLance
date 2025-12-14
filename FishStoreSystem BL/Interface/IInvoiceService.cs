using FishStoreSystem.BL.DTO;

namespace FishStoreSystem_BL.Interface
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDTO>> GetAllAsync();
        Task<InvoiceDTO> GetByIdAsync(int id);
        Task<InvoiceDTO> CreateAsync(InvoiceDTO dto);
        Task AddPaymentAsync(int invoiceId, decimal amount, string method);
    }
}
