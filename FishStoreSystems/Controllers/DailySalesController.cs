using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FishStoreSystem_DAL;
using FishStoreSystem_DAL.Entities;

namespace FishStoreSystem.Controllers
{
    public class DailySalesController : Controller
    {
        private readonly AppDbContext _context;
        public DailySalesController(AppDbContext context) => _context = context;

        // Index
        public async Task<IActionResult> Index(DateTime? date)
        {
            var q = _context.DailySales.AsQueryable();
            if (date.HasValue) q = q.Where(s => s.Date.Date == date.Value.Date);
            var list = await q.OrderByDescending(s => s.Date).ToListAsync();
            return View(list);
        }

        // Details
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.DailySales.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // Create - GET
        public IActionResult Create()
        {
            var model = new DailySale { Date = DateTime.Today };
            return View(model);
        }

        // Create - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailySale model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.DailySales.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Edit - GET
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.DailySales.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // Edit - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DailySale model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Delete - GET
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.DailySales.FindAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // Delete - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.DailySales.FindAsync(id);
            if (item != null)
            {
                _context.DailySales.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
