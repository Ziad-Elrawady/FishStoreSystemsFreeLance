using FishStoreSystem.BL.DTO;

namespace FishStoreSystem_BL.Interface
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllAsync();
        Task<CustomerDTO> GetByIdAsync(int id); 
        Task<CustomerDTO> CreateAsync(CustomerDTO dto);
        Task UpdateAsync(CustomerDTO dto);
        Task DeleteAsync(int id);
        Task<decimal> GetTotalDebtAsync(int customerId);  // <-- دالة مساعدة
        Task<IEnumerable<CustomerDTO>> GetOverdueCustomersAsync();
    }
}
