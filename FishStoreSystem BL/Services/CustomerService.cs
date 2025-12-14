using AutoMapper;
using FishStoreSystem.BL.DTO;
using FishStoreSystem.DAL.Entities;
using FishStoreSystem_BL.Interface;
using FishStoreSystem_DAL.Interface;

namespace FishStoreSystem_BL.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers.Select(c => new CustomerDTO
            {
                Id = c.Id,
                Name = c.Name,
                Phone = c.Phone,
                CreditLimit = c.CreditLimit,
                Status = c.Status,
                TotalDebt = c.Invoices.Sum(i => i.TotalAmount - i.PaidAmount)
            });
        }

        public async Task<CustomerDTO> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task<CustomerDTO> CreateAsync(CustomerDTO dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            var created = await _repository.AddAsync(customer);
            return _mapper.Map<CustomerDTO>(created);
        }

        public async Task UpdateAsync(CustomerDTO dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            await _repository.UpdateAsync(customer);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<decimal> GetTotalDebtAsync(int customerId)
        {
            var customer = await _repository.GetByIdAsync(customerId);
            return customer?.Invoices.Sum(i => i.RemainingAmount) ?? 0;
        }

        public async Task<IEnumerable<CustomerDTO>> GetOverdueCustomersAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers
                .Where(c => c.Invoices.Any(i => i.DueDate < DateTime.Now && i.Status != "مغلقة"))
                .Select(c => new CustomerDTO {
                    Id = c.Id,
                    Name = c.Name,
                    Phone = c.Phone,
                    CreditLimit = c.CreditLimit,
                    Status = "متأخر",
                    TotalDebt = c.Invoices.Sum(i => i.TotalAmount - i.PaidAmount)
                })
                .ToList();
        }
    }
}
