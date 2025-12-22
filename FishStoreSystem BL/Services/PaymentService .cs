using FishStoreSystem.BL.DTO;
using FishStoreSystem_BL.Interface;
using FishStoreSystem_DAL.Interface;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;

    public PaymentService(IPaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PaymentDTO>> GetAllAsync()
    {
        var payments = await _repository.GetAllAsync();

        return payments.Select(p => new PaymentDTO
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            Amount = p.Amount,
            PaymentDate = p.PaymentDate,
            PaymentMethod = p.PaymentMethod   // ✅ الصح
        }).ToList();

    }
}
