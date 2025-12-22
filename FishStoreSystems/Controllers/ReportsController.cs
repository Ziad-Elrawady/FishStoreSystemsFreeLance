using FishStoreSystem.BL.DTO;
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
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _context;
        public ReportsController(
            IInvoiceService invoiceService,
            ICustomerService customerService,
            IPaymentService paymentService,
            AppDbContext context)
        {
            _invoiceService = invoiceService;
            _customerService = customerService;
            _paymentService = paymentService;
            _context = context;
        }

        // ================= DAILY REPORT =================
        public async Task<IActionResult> Daily(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;

            // فواتير اليوم
            var invoicesOfDay = await _context.Invoices
                .Include(i => i.Payments)
                .Include(i => i.Customer)
                .Where(i => i.InvoiceDate.Date == targetDate.Date)
                .ToListAsync();

            // 🔥 المدفوعات المستلمة (من جدول Payments مباشرة)
            var paymentsReceived = await _context.Payments
                .Where(p => p.PaymentDate.Date == targetDate.Date)
                .SumAsync(p => p.Amount);

            // المبالغ المتبقية من فواتير اليوم
            var receivables = invoicesOfDay
                .Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount));

            // مبيعات صاحب المحل (يدخلها يدوي)
            var dailySales = await _context.DailySales
                .Where(o => o.Date.Date == targetDate.Date)
                .SumAsync(o => o.Amount);

            // الخوارج اليومية
            var expenses = await _context.DailyExpenses
                .Where(e => e.Date.Date == targetDate.Date)
                .SumAsync(e => e.Amount);

            var model = new DailyReportViewModel
            {
                Date = targetDate,

                // 👇 نفس اللي في الـ UI
                PaymentsReceived = paymentsReceived,
                Receivables = receivables,
                TotalManualSales = dailySales,
                Expenses = expenses,

                // عرض فواتير اليوم
                InvoicesOfDay = invoicesOfDay.Select(i =>
                {
                    var paid = i.Payments.Sum(p => p.Amount);

                    return new InvoiceDTO
                    {
                        Id = i.Id,
                        CustomerName = i.Customer.Name,
                        InvoiceDate = i.InvoiceDate,
                        TotalAmount = i.TotalAmount,
                        PaidAmount = paid,
                        RemainingAmount = i.TotalAmount - paid
                    };
                }).ToList()
            };

            return View(model);
        }


        public async Task<IActionResult> Weekly(DateTime? start)
        {
            var startDate = start ?? GetWeekStart(DateTime.Today);
            var endDate = startDate.AddDays(6);

            var items = new List<WeeklyItem>();

            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                // فواتير اليوم
                var dayInvoices = await _context.Invoices
                    .Include(i => i.Payments)
                    .Where(i => i.InvoiceDate.Date == day.Date)
                    .ToListAsync();

                // التحصيل الفعلي لليوم
                var paymentsReceived = await _context.Payments
                    .Where(p => p.PaymentDate.Date == day.Date)
                    .SumAsync(p => p.Amount);

                // المبيعات اليدوية
                var manualSales = await _context.DailySales
                    .Where(s => s.Date.Date == day.Date)
                    .SumAsync(s => s.Amount);

                // الخوارج
                var expenses = await _context.DailyExpenses
                    .Where(e => e.Date.Date == day.Date)
                    .SumAsync(e => e.Amount);

                // المتبقي على الزبائن (فواتير اليوم فقط)
                var receivables = dayInvoices
                    .Sum(i => i.TotalAmount - i.Payments.Sum(p => p.Amount));

                items.Add(new WeeklyItem
                {
                    Date = day,
                    Sales = manualSales,
                    PaymentsReceived = paymentsReceived,
                    Expenses = expenses,
                    Receivables = receivables
                    // NetProfit هيتحسب تلقائي
                });
            }

            return View(new WeeklyReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Items = items
            });
        }


        // ================== HELPER ==================
        private DateTime GetWeekStart(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Saturday)) % 7;
            return date.AddDays(-diff).Date;
        }
        public async Task<IActionResult> WeeklyArchive()
        {
            // 🔹 نجيب أول تاريخ فاتورة حقيقي
            var invoiceDates = await _context.Invoices
                .Select(i => i.InvoiceDate.Date)
                .Distinct()
                .ToListAsync();

            if (!invoiceDates.Any())
                return View(new List<WeeklyArchiveViewModel>());

            var firstWeekStart = GetWeekStart(invoiceDates.Min());
            var lastWeekStart = GetWeekStart(DateTime.Today);

            var reports = new List<WeeklyArchiveViewModel>();

            for (var weekStart = firstWeekStart;
                 weekStart <= lastWeekStart;
                 weekStart = weekStart.AddDays(7))
            {
                var days = new List<WeeklyDayViewModel>();

                for (int i = 0; i < 7; i++)
                {
                    var day = weekStart.AddDays(i);

                    var dayInvoices = await _context.Invoices
                        .Include(i2 => i2.Payments)
                        .Where(i2 => i2.InvoiceDate.Date == day.Date)
                        .ToListAsync();

                    var paymentsReceived = await _context.Payments
                        .Where(p => p.PaymentDate.Date == day.Date)
                        .SumAsync(p => p.Amount);

                    var manualSales = await _context.DailySales
                        .Where(s => s.Date.Date == day.Date)
                        .SumAsync(s => s.Amount);

                    var expenses = await _context.DailyExpenses
                        .Where(e => e.Date.Date == day.Date)
                        .SumAsync(e => e.Amount);

                    var receivables = dayInvoices
                        .Sum(inv => inv.TotalAmount - inv.Payments.Sum(p => p.Amount));

                    days.Add(new WeeklyDayViewModel
                    {
                        Date = day,
                        Sales = manualSales,
                        PaymentsReceived = paymentsReceived,
                        Expenses = expenses,
                        Receivables = receivables
                    });
                }

                reports.Add(new WeeklyArchiveViewModel
                {
                    StartDate = weekStart,
                    EndDate = weekStart.AddDays(6),
                    Days = days
                });
            }

            // 🔥 دي مهمة عشان تشوف كل الأسابيع
            return View(reports.OrderByDescending(r => r.StartDate).ToList());
        }


        public async Task<IActionResult> DailyPrint(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;

            // فواتير اليوم
            var dailyInvoices = await _invoiceService.GetAllAsync();
            var invoicesOfDay = dailyInvoices
                .Where(i => i.InvoiceDate.Date == targetDate.Date)
                .ToList();

            // ✅ المدفوعات (الحل النهائي)
            var paymentsReceived = await _context.Payments
                .Where(p => p.PaymentDate.Date == targetDate.Date)
                .SumAsync(p => p.Amount);

            // المبيعات اليدوية
            var totalManualSales = await _context.DailySales
                .Where(d => d.Date.Date == targetDate.Date)
                .SumAsync(d => d.Amount);

            // الخوارج
            var expenses = await _context.DailyExpenses
                .Where(e => e.Date.Date == targetDate.Date)
                .SumAsync(e => e.Amount);

            // المبالغ على الزبائن
            var receivables = invoicesOfDay
                .Sum(i => i.TotalAmount - i.PaidAmount);

            var model = new DailyReportViewModel
            {
                Date = targetDate,
                TotalManualSales = totalManualSales,
                PaymentsReceived = paymentsReceived,
                Expenses = expenses,
                Receivables = receivables,
                InvoicesOfDay = invoicesOfDay
            };

            return View("DailyPrint", model);
        }




        public async Task<IActionResult> WeeklyPrint(DateTime start)
        {
            var startDate = GetWeekStart(start);
            var endDate = startDate.AddDays(6);

            var invoices = await _invoiceService.GetAllAsync();
            var payments = await _paymentService.GetAllAsync();

            var items = new List<WeeklyItem>();

            for (var day = startDate; day <= endDate; day = day.AddDays(1))
            {
                var invoicesOfDay = invoices
                    .Where(i => i.InvoiceDate.Date == day.Date)
                    .ToList();

                var paymentsReceived = payments
                    .Where(p => p.PaymentDate.Date == day.Date)
                    .Sum(p => p.Amount);

                var manualSales = await _context.DailySales
                    .Where(d => d.Date.Date == day.Date)
                    .SumAsync(d => d.Amount);

                var expenses = await _context.DailyExpenses
                    .Where(e => e.Date.Date == day.Date)
                    .SumAsync(e => e.Amount);

                var receivables = invoicesOfDay
                    .Sum(i => i.TotalAmount - i.PaidAmount);

                items.Add(new WeeklyItem
                {
                    Date = day,
                    Sales = manualSales,
                    PaymentsReceived = paymentsReceived,
                    Expenses = expenses,
                    Receivables = receivables
                    // NetProfit بيتحسب أوتوماتيك
                });
            }


            return View("WeeklyPrint", new WeeklyReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Items = items
            });
        }




    }
}
