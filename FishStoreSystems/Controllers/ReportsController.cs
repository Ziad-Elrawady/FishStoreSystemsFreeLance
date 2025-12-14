using FishStoreSystem.ViewModels;
using FishStoreSystem_BL.Interface;
using FishStoreSystem_DAL;
using FishStoreSystem_DAL.Entities;
using FishStoreSystems.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishStoreSystem.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;
        private readonly AppDbContext _context;

        public ReportsController(IInvoiceService invoiceService, ICustomerService customerService, AppDbContext context)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _context = context;
        }

        // ================= DAILY REPORT =================
        public async Task<IActionResult> Daily(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;

            var allInvoices = await _invoiceService.GetAllAsync();

            // فواتير اليوم
            var invoicesOfDay = allInvoices
                .Where(i => i.InvoiceDate.Date == targetDate.Date)
                .ToList();

            // المدفوعات حسب تاريخ الدفع (سواء الفاتورة كانت في يوم تاني)
            var paymentsReceived = allInvoices
                .SelectMany(i => i.Payments)
                .Where(p => p.PaymentDate.Date == targetDate.Date)
                .Sum(p => p.Amount);

            // المبالغ المتبقية من فواتير اليوم فقط
            var receivables = invoicesOfDay
                .Where(i => i.PaidAmount < i.TotalAmount)
                .Sum(i => i.TotalAmount - i.PaidAmount);

            // مبيعات صاحب المحل (يدخلها هو يدوي)
            var DailySales = await _context.DailySales
                .Where(o => o.Date.Date == targetDate.Date)
                .SumAsync(o => o.Amount);

            // الخوارج اليومية
            var expenses = await _context.DailyExpenses
                .Where(e => e.Date.Date == targetDate.Date)
                .SumAsync(e => e.Amount);

            var model = new DailyReportViewModel
            {
                Date = targetDate,
                InvoicesOfDay = invoicesOfDay,
                PaymentsReceived = paymentsReceived,
                Receivables = receivables,
                TotalManualSales = DailySales,
                Expenses = expenses
            };

            return View(model);
        }

        // ================= WEEKLY REPORT =================
        public async Task<IActionResult> Weekly(DateTime? start)
        {
            var startDate = start ?? DateTime.Today ; // يبدأ من النهارده
            var endDate = startDate.AddDays(6);

            var allInvoices = await _invoiceService.GetAllAsync();

            var weekItems = new List<WeeklyItem>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var invoicesOfDay = allInvoices
                    .Where(i => i.InvoiceDate.Date == date.Date)
                    .ToList();

                var sales = await _context.DailySales
                    .Where(o => o.Date.Date == date.Date)
                    .SumAsync(o => o.Amount);

                var paymentsReceived = allInvoices
                    .SelectMany(i => i.Payments)
                    .Where(p => p.PaymentDate.Date == date.Date)
                    .Sum(p => p.Amount);

                var expenses = await _context.DailyExpenses
                    .Where(e => e.Date.Date == date.Date)
                    .SumAsync(e => e.Amount);

                var receivables = invoicesOfDay
                    .Where(i => i.PaidAmount < i.TotalAmount)
                    .Sum(i => i.TotalAmount - i.PaidAmount);

                weekItems.Add(new WeeklyItem
                {
                    Date = date,
                    Sales = sales,
                    PaymentsReceived = paymentsReceived,
                    Expenses = expenses,
                    Receivables = receivables
                });
            }

            // حفظ الأسبوع لو مش محفوظ
            var exists = await _context.Weeklys
                .AnyAsync(w => w.StartDate == startDate && w.EndDate == endDate);

            if (!exists)
            {
                var weekly = new Weekly
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Items = weekItems
                };

                _context.Weeklys.Add(weekly);
                await _context.SaveChangesAsync();
            }

            var model = new WeeklyReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Items = weekItems
            };

            return View(model);
        }

        // ================= WEEKLY ARCHIVE =================
        public async Task<IActionResult> WeeklyArchive()
        {
            var weeklyEntities = await _context.Weeklys
                .Include(w => w.Items)
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();

            var model = weeklyEntities.Select(w => new WeeklyArchiveViewModel
            {
                StartDate = w.StartDate,
                EndDate = w.EndDate,
                Days = w.Items.OrderBy(i => i.Date)
                    .Select(i => new WeeklyDayViewModel
                    {
                        Date = i.Date,
                        Sales = i.Sales,
                        PaymentsReceived = i.PaymentsReceived,
                        Expenses = i.Expenses,
                        Receivables = i.Receivables
                    }).ToList()
            }).ToList();

            return View(model);
        }
        public async Task<IActionResult> DailyPrint(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;

            var allInvoices = await _invoiceService.GetAllAsync();
            var overdueCustomers = await _customerService.GetOverdueCustomersAsync();

            var dailyInvoices = allInvoices
                .Where(i => i.InvoiceDate.Date == targetDate.Date)
                .ToList();

            var receivables = dailyInvoices
                .Where(i => i.PaidAmount < i.TotalAmount)
                .Sum(i => i.RemainingAmount);

            var paymentsReceived = dailyInvoices
                .SelectMany(i => i.Payments)
                .Where(p => p.PaymentDate.Date == targetDate.Date)
                .Sum(p => p.Amount);

            var model = new DailyReportViewModel
            {
                Date = targetDate,
                Receivables = receivables,
                PaymentsReceived = paymentsReceived,
                OverdueCustomers = overdueCustomers.ToList(),
                InvoicesOfDay = dailyInvoices
            };

            return View("DailyPrint", model); // استدعاء View منفصل للطباعة
        }
        public async Task<IActionResult> WeeklyPrint(DateTime? start)
        {
            var startDate = start ?? DateTime.Today;
            var endDate = startDate.AddDays(6);

            var allInvoices = await _invoiceService.GetAllAsync();

            var weekItems = new List<WeeklyItem>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var invoicesOfDay = allInvoices
                    .Where(i => i.InvoiceDate.Date == date.Date)
                    .ToList();

                var sales = invoicesOfDay.Sum(i => i.TotalAmount);
                var paymentsReceived = invoicesOfDay
                    .SelectMany(i => i.Payments)
                    .Where(p => p.PaymentDate.Date == date.Date)
                    .Sum(p => p.Amount);
                var receivables = invoicesOfDay
                    .Where(i => i.PaidAmount < i.TotalAmount)
                    .Sum(i => i.RemainingAmount);

                decimal expenses = 0; // يمكن إدخالها من جدول الخوارج

                weekItems.Add(new WeeklyItem
                {
                    Date = date,
                    Sales = sales,
                    PaymentsReceived = paymentsReceived,
                    Receivables = receivables,
                    Expenses = expenses
                });
            }

            var model = new WeeklyReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Items = weekItems
            };

            return View("WeeklyPrint", model); // استدعاء View منفصل للطباعة
        }

    }
}
