using FishStoreSystem.DAL.Entities;

namespace FishStoreSystem_DAL.Interface
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice> GetByIdAsync(int id);
        Task<Invoice> AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task AddPaymentAsync(int invoiceId, decimal amount, string method);
    }
}
