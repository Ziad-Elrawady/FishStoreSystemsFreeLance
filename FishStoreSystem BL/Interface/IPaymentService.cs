using FishStoreSystem.BL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishStoreSystem_BL.Interface
{
    public interface IPaymentService
    {
        Task<List<PaymentDTO>> GetAllAsync();
    }
}
