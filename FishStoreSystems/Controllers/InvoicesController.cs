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
            if (ModelState.IsValid)
            {
                var invoiceDto = new InvoiceDTO
                {
                    CustomerId = model.CustomerId,
                    DueDate = model.DueDate,
                    Items = model.Items.Select(i => new InvoiceItemDTO
                    {
                        ItemName = i.ItemName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.Quantity * i.UnitPrice   // ✔ الحساب هنا
                    }).ToList()
                };

                invoiceDto.TotalAmount = invoiceDto.Items.Sum(i => i.TotalPrice);

                await _invoiceService.CreateAsync(invoiceDto);

                return RedirectToAction(nameof(Index));
            }

            var customers = await _customerService.GetAllAsync();
            model.CustomersList = new SelectList(customers, "Id", "Name");
            return View(model);
        }



        // POST: /Invoices/AddPayment
        [HttpPost]
        public async Task<IActionResult> AddPayment(int invoiceId, decimal amount, string method = "كاش")
        {
            if (amount > 0)
            {
                await _invoiceService.AddPaymentAsync(invoiceId, amount, method);
            }
            return RedirectToAction(nameof(Details), new { id = invoiceId });
        }

      

        // GET: /Invoices/Print/5
        public async Task<IActionResult> Print(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            return View(invoice); // نستخدم View جديدة للطباعة
        }


    }
}
