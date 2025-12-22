using FishStoreSystem.BL.DTO;
using FishStoreSystem.ViewModels;
using FishStoreSystem_BL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FishStoreSystem.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ICustomerService _customerService;

        public InvoicesController(IInvoiceService invoiceService, ICustomerService customerService)
        { 
            _invoiceService = invoiceService;
            _customerService = customerService;
        }

        // GET: /Invoices
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return View(invoices);
        }

        // GET: /Invoices/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // GET: /Invoices/Create
        public async Task<IActionResult> Create()
        {
            var customers = await _customerService.GetAllAsync();
            var model = new InvoiceCreateViewModel
            {
                CustomersList = new SelectList(customers, "Id", "Name"),
                DueDate = DateTime.Now.AddDays(7)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceCreateViewModel model)
        {
            // 🔴 Validation أساسي
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "لا يمكن إنشاء فاتورة بدون أصناف");
            }

            if (!ModelState.IsValid)
                goto Reload;

            // فلترة الأصناف الصح بس
            var validItems = model.Items
                .Where(i => i.Quantity > 0 && i.UnitPrice > 0)
                .ToList();

            if (!validItems.Any())
            {
                ModelState.AddModelError("", "الأصناف المدخلة غير صحيحة");
                goto Reload;
            }

            var invoiceDto = new InvoiceDTO
            {
                CustomerId = model.CustomerId,

                // 🔥 دي كانت ناقصة وبتكسر الإضافة
                InvoiceDate = DateTime.Now,

                DueDate = model.DueDate,

                Items = validItems.Select(i => new InvoiceItemDTO
                {
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.Quantity * i.UnitPrice
                }).ToList()
            };

            invoiceDto.TotalAmount = invoiceDto.Items.Sum(i => i.TotalPrice);

            await _invoiceService.CreateAsync(invoiceDto);

            return RedirectToAction(nameof(Index));

        Reload:
            var customers = await _customerService.GetAllAsync();
            model.CustomersList = new SelectList(customers, "Id", "Name");
            return View(model);
        }




        // POST: /Invoices/AddPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPayment(int invoiceId, decimal amount, string method)
        {
            if (invoiceId <= 0)
                return BadRequest();

            if (amount <= 0)
            {
                TempData["Error"] = "المبلغ لازم يكون أكبر من صفر";
                return RedirectToAction(nameof(Details), new { id = invoiceId });
            }

            if (string.IsNullOrWhiteSpace(method))
                method = "كاش";

            await _invoiceService.AddPaymentAsync(invoiceId, amount, method);

            return RedirectToAction(nameof(Index));
        }




        // GET: /Invoices/Print/5
        public async Task<IActionResult> Print(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            return View(invoice); // نستخدم View جديدة للطباعة
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePayment(int paymentId, int invoiceId)
        {
            await _invoiceService.DeletePaymentAsync(paymentId);
            return RedirectToAction("Details", "Customers", new { id = invoiceId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            await _invoiceService.DeleteInvoiceAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
