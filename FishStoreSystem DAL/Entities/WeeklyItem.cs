using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishStoreSystem_DAL.Entities
{
    public class WeeklyItem
    {
        public int Id { get; set; }
        public int WeeklyReportId { get; set; }
        public DateTime Date { get; set; }
        public decimal Sales { get; set; }
        public decimal PaymentsReceived { get; set; }
        public decimal Expenses { get; set; }
        public decimal Receivables { get; set; }

        public Weekly? WeeklyReport { get; set; }
        public decimal NetProfit => Sales + PaymentsReceived - Expenses;


    }
}
