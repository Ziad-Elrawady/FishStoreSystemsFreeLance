using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishStoreSystem_DAL.Entities
{
    public class Weekly
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // إجماليات الأسبوع
        public decimal TotalSales { get; set; }
        public decimal TotalPaymentsReceived { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalReceivables { get; set; }

        public List<WeeklyItem> Items { get; set; } = new();
    }
}
