using FishStoreSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishStoreSystem_DAL.Interface
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetAllAsync();
        Task AddAsync(Payment payment);
    }
}
