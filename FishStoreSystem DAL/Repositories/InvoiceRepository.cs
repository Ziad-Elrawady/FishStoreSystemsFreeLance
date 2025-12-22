using FishStoreSystem.DAL.Entities;
using FishStoreSystem_DAL.Interface;
using Microsoft.EntityFrameworkCore;

namespace FishStoreSystem_DAL.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .ToListAsync();
        }


        public async Task<Invoice> GetByIdAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Items)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id);
#pragma warning restore CS8603 // Possible null reference return.
        }

        public async Task<Invoice> AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task AddPaymentAsync(int invoiceId, decimal amount, string method)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) throw new KeyNotFoundException("الفاتورة غير موجودة");

            invoice.PaidAmount += amount;
            invoice.Status = invoice.PaidAmount >= invoice.TotalAmount ? "مغلقة" : "جزئية";

            _context.Payments.Add(new Payment
            {
                InvoiceId = invoiceId,
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now
            });


            await _context.SaveChangesAsync();
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task DeletePaymentAsync(Payment payment)
        {
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(Invoice invoice)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }

    }
}
