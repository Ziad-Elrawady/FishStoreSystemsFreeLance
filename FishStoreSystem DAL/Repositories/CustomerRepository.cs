using FishStoreSystem.DAL.Entities;
using FishStoreSystem_DAL.Interface;
using Microsoft.EntityFrameworkCore;

namespace FishStoreSystem_DAL.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.Invoices)
                .ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Customers
                .Include(c => c.Invoices)
                .ThenInclude(i => i.Payments)
                .Include(c => c.Invoices)
                .ThenInclude(i => i.Items)
                .FirstOrDefaultAsync(c => c.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Customer>> GetOverdueCustomersAsync()
        {
            return await _context.Customers
                .Include(c => c.Invoices)
                .Where(c => c.Invoices.Any(i => i.DueDate < DateTime.Now && i.Status != "مغلقة"))
                .ToListAsync();
        }
    }
}
