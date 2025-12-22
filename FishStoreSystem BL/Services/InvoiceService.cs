using AutoMapper;
using FishStoreSystem.BL.DTO;
using FishStoreSystem.DAL.Entities;
using FishStoreSystem_BL.Interface;
using FishStoreSystem_DAL.Interface;
using FishStoreSystem_DAL.Repositories;

namespace FishStoreSystem_BL.Services
{
    public class InvoiceService: IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IMapper _mapper;

        public InvoiceService(IInvoiceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InvoiceDTO>> GetAllAsync()
        {
            var invoices = await _repository.GetAllAsync();

            return invoices.Select(i =>
            {
                var paid = i.Payments.Sum(p => p.Amount);
                var remaining = i.TotalAmount - paid;

                return new InvoiceDTO
                {
                    Id = i.Id,
                    CustomerId = i.CustomerId,
                    CustomerName = i.Customer?.Name,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = paid,
                    RemainingAmount = remaining,
                    Status = remaining <= 0 ? "مدفوعة" : "غير مدفوعة"
                };
            });
        }


        public async Task<InvoiceDTO> GetByIdAsync(int id)
        {
            var invoice = await _repository.GetByIdAsync(id);
            if (invoice == null) return null;

            var paid = invoice.Payments?.Sum(p => p.Amount) ?? 0;
            var remaining = invoice.TotalAmount - paid;

            return new InvoiceDTO
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer?.Name,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,

                // 🔥 الحساب الصح
                PaidAmount = paid,
                RemainingAmount = remaining,
                Status = remaining <= 0 ? "مدفوعة" : "غير مدفوعة",

                Items = invoice.Items.Select(item => new InvoiceItemDTO
                {
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList(),

                Payments = invoice.Payments.Select(p => new PaymentDTO
                {
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod
                }).ToList()
            };
        }


        public async Task<InvoiceDTO> CreateAsync(InvoiceDTO dto)
        {
            var invoice = new Invoice
            {
                CustomerId = dto.CustomerId,
                DueDate = dto.DueDate,
                InvoiceDate = DateTime.Now,
                PaidAmount = 0,
                Status = "مفتوحة",
                Items = dto.Items.Select(i => new InvoiceItem
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                }).ToList()
            };

            invoice.TotalAmount = invoice.Items.Sum(i => i.TotalPrice);

            var created = await _repository.AddAsync(invoice);
            return await GetByIdAsync(created.Id);
        }


        public async Task AddPaymentAsync(int invoiceId, decimal amount, string method)
        {
            var invoice = await _repository.GetByIdAsync(invoiceId);
            if (invoice == null) return;

            var remaining = invoice.TotalAmount - invoice.PaidAmount;

            // لو الفاتورة مدفوعة بالكامل، نسمح بسجل دفع (تصحيح)
            if (remaining <= 0)
            {
                invoice.Payments.Add(new Payment
                {
                    InvoiceId = invoice.Id,
                    Amount = amount,
                    PaymentMethod = method,
                    PaymentDate = DateTime.Now
                });

                await _repository.UpdateAsync(invoice);
                return;
            }

            if (amount > remaining)
                amount = remaining;

            // تسجيل الدفعة
            invoice.Payments.Add(new Payment
            {
                InvoiceId = invoice.Id,
                Amount = amount,
                PaymentMethod = method,
                PaymentDate = DateTime.Now
            });

            invoice.PaidAmount += amount;

            invoice.Status = invoice.PaidAmount >= invoice.TotalAmount
                ? "مدفوعة"
                : "جزئي";

            await _repository.UpdateAsync(invoice);
        }

        public async Task DeletePaymentAsync(int paymentId)
        {
            var payment = await _repository.GetPaymentByIdAsync(paymentId);
            if (payment == null) return;

            await _repository.DeletePaymentAsync(payment);
        }

        public async Task DeleteInvoiceAsync(int invoiceId)
        {
            var invoice = await _repository.GetByIdAsync(invoiceId);
            if (invoice == null) return;

            await _repository.DeleteInvoiceAsync(invoice);
        }



    }
}
