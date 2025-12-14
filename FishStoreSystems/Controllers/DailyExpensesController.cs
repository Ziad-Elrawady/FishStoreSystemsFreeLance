using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FishStoreSystem_DAL;
using FishStoreSystem_DAL.Entities;

namespace FishStoreSystem.Controllers
{
    public class DailyExpensesController : Controller
    {
        private readonly AppDbContext _context;
        public DailyExpensesController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index(DateTime? date)
        {
            var q = _context.DailyExpenses.AsQueryable();
            if (date.HasValue) q = q.Where(s => s.Date.Date == date.Value.Date);
            var list = await q.OrderByDescending(s => s.Date).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.DailyExpenses.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            var model = new DailyExpense { Date = DateTime.Today };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailyExpense model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.DailyExpenses.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.DailyExpenses.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DailyExpense model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.DailyExpenses.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.DailyExpenses.FindAsync(id);
            if (item != null)
            {
                _context.DailyExpenses.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
