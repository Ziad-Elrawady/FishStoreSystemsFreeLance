using FishStoreSystem.BL.DTO;
using FishStoreSystem.ViewModels;
using FishStoreSystem_BL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FishStoreSystem.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IInvoiceService _invoiceService;

        public CustomersController(ICustomerService customerService, IInvoiceService invoiceService)
        {
            _customerService = customerService;
            _invoiceService = invoiceService;
        }

        // GET: /Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllAsync();
            return View(customers);
        }

        // GET: /Customers/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // 1. جيب بيانات الزبون (DTO من BL)
            var customerDto = await _customerService.GetByIdAsync(id);
            if (customerDto == null) return NotFound();

            // 2. جيب كل الفواتير ونقّي المفتوحة (DTO من BL)
            var allInvoices = await _invoiceService.GetAllAsync();
            var openInvoices = allInvoices.Where(i => i.CustomerId == id && i.Status != "مغلقة").ToList();

            // 3. احسب الدين الإجمالي (من BL)
            var totalDebt = await _customerService.GetTotalDebtAsync(id);

            // 4. املأ الـ ViewModel في الـ UI
            var viewModel = new CustomerDetailsViewModel
            {
                Customer = customerDto,
                OpenInvoices = openInvoices,
                TotalDebt = totalDebt
            };

            return View(viewModel);
        }

        // GET: /Customers/Create
        public IActionResult Create() => View();

        // POST: /Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerDTO dto)
        {
            if (ModelState.IsValid)
            {
                await _customerService.CreateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: /Customers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: /Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerDTO dto)
        {
            if (ModelState.IsValid)
            {
                await _customerService.UpdateAsync(dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // POST: /Customers/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
