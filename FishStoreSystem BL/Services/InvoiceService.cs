using AutoMapper;
using FishStoreSystem.BL.DTO;
using FishStoreSystem.DAL.Entities;
using FishStoreSystem_BL.Interface;
using FishStoreSystem_DAL.Interface;

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
            return invoices.Select(i => new InvoiceDTO
            {
                Id = i.Id,
                CustomerId = i.CustomerId,
                CustomerName = i.Customer?.Name,
                InvoiceDate = i.InvoiceDate,
                DueDate = i.DueDate,
                TotalAmount = i.TotalAmount,
                PaidAmount = i.PaidAmount,
                RemainingAmount = i.RemainingAmount,
                Status = i.Status,
                Items = i.Items.Select(item => new InvoiceItemDTO
                {
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                }).ToList(),
                Payments = i.Payments.Select(p => new PaymentDTO
                {
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod
                }).ToList()
            });
        }

        public async Task<InvoiceDTO> GetByIdAsync(int id)
        {
            var invoice = await _repository.GetByIdAsync(id);
            if (invoice == null) return null;

            return new InvoiceDTO
            {
                Id = invoice.Id,
                CustomerId = invoice.CustomerId,
                CustomerName = invoice.Customer?.Name,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = invoice.PaidAmount,
                RemainingAmount = invoice.RemainingAmount,
                Status = invoice.Status,
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
            await _repository.AddPaymentAsync(invoiceId, amount, method);
        }
    }
}
