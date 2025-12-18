using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_bitirme2.Data;
using WMS_bitirme2.Models;

namespace WMS_bitirme2.Controllers
{
    public class StockMovementsController : Controller
    {
        private readonly WMSDbContext _context;

        public StockMovementsController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: StockMovements
        public async Task<IActionResult> Index()
        {
            // Include komutları: "Hareketleri getirirken, Ürün ve Raf detaylarını da yanına al"
            var wMSDbContext = _context.StockMovements
                .Include(s => s.Product)
                .Include(s => s.Shelf);

            return View(await wMSDbContext.ToListAsync());
        }

        // GET: StockMovements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockMovement = await _context.StockMovements
                .Include(s => s.Product)
                .Include(s => s.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockMovement == null)
            {
                return NotFound();
            }

            return View(stockMovement);
        }

        // GET: StockMovements/Create
        public IActionResult Create()
        {
            // Ürün Adı görünsün
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad");
            // Raf Kodu görünsün
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "Id", "Kod");
            return View();
        }

        // POST: StockMovements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,ShelfId,Miktar,HareketTipi,Tarih")] StockMovement stockMovement)
        {
            // 1. Validasyon Hatasını Önle (Product ve Shelf nesneleri boş gelebilir)
            ModelState.Remove("Product");
            ModelState.Remove("Shelf");

            if (ModelState.IsValid)
            {
                _context.Add(stockMovement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 2. Dropdown'da İsimlerin Görünmesini Sağla
            // Product için 'Id' yerine 'Ad' göster
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", stockMovement.ProductId);
            // Shelf için 'Id' yerine 'Kod' göster
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "Id", "Kod", stockMovement.ShelfId);

            return View(stockMovement);
        }

        // GET: StockMovements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockMovement = await _context.StockMovements.FindAsync(id);
            if (stockMovement == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockMovement.ProductId);
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "Id", "Id", stockMovement.ShelfId);
            return View(stockMovement);
        }

        // POST: StockMovements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ShelfId,Miktar,HareketTipi,Tarih")] StockMovement stockMovement)
        {
            if (id != stockMovement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockMovement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockMovementExists(stockMovement.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", stockMovement.ProductId);
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "Id", "Id", stockMovement.ShelfId);
            return View(stockMovement);
        }

        // GET: StockMovements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockMovement = await _context.StockMovements
                .Include(s => s.Product)
                .Include(s => s.Shelf)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockMovement == null)
            {
                return NotFound();
            }

            return View(stockMovement);
        }

        // POST: StockMovements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockMovement = await _context.StockMovements.FindAsync(id);
            if (stockMovement != null)
            {
                _context.StockMovements.Remove(stockMovement);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockMovementExists(int id)
        {
            return _context.StockMovements.Any(e => e.Id == id);
        }
    }
}
