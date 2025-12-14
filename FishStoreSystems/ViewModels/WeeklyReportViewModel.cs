using FishStoreSystem.BL.DTO;
using FishStoreSystem_DAL.Entities;

namespace FishStoreSystem.ViewModels
{
    public class WeeklyReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<WeeklyItem> Items { get; set; } = new();
    }
}
