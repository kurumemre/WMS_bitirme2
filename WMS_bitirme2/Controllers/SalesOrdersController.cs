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
    public class SalesOrdersController : Controller
    {
        private readonly WMSDbContext _context;

        public SalesOrdersController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: SalesOrders
        public async Task<IActionResult> Index()
        {
            var wMSDbContext = _context.SalesOrders.Include(s => s.Customer);
            return View(await wMSDbContext.ToListAsync());
        }

        // GET: SalesOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .Include(s => s.Items).ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // GET: SalesOrders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            return View();
        }

        // POST: SalesOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderCode,CustomerId,OrderDate,Status,Notes")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salesOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // GET: SalesOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders.FindAsync(id);
            if (salesOrder == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // POST: SalesOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderCode,CustomerId,OrderDate,Status,Notes")] SalesOrder salesOrder)
        {
            if (id != salesOrder.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. ESKİ DURUMU ÖĞREN
                    var eskiSiparis = await _context.SalesOrders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    var siparisDetaylari = _context.SalesOrderItems.Where(x => x.SalesOrderId == id).ToList();

                    // SENARYO A: Satış Yapıldı (Stoktan DÜŞ -)
                    // Hazırlanıyor -> Tamamlandı
                    if (eskiSiparis.Status != SalesOrderStatus.Tamamlandi && salesOrder.Status == SalesOrderStatus.Tamamlandi)
                    {
                        foreach (var kalem in siparisDetaylari)
                        {
                            var urun = await _context.Products.FindAsync(kalem.ProductId);
                            if (urun != null)
                            {
                                urun.StokMiktari -= kalem.Quantity; // AZALT 📉
                                _context.Update(urun);
                            }
                        }
                    }
                    // SENARYO B: İptal/İade (Stoku GERİ YÜKLE +)
                    // Tamamlandı -> Hazırlanıyor/İptal
                    else if (eskiSiparis.Status == SalesOrderStatus.Tamamlandi && salesOrder.Status != SalesOrderStatus.Tamamlandi)
                    {
                        foreach (var kalem in siparisDetaylari)
                        {
                            var urun = await _context.Products.FindAsync(kalem.ProductId);
                            if (urun != null)
                            {
                                urun.StokMiktari += kalem.Quantity; // ARTTIR (İade al) 📈
                                _context.Update(urun);
                            }
                        }
                    }

                    _context.Update(salesOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderExists(salesOrder.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
            return View(salesOrder);
        }

        // GET: SalesOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // POST: SalesOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesOrder = await _context.SalesOrders.FindAsync(id);
            if (salesOrder != null)
            {
                _context.SalesOrders.Remove(salesOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderExists(int id)
        {
            return _context.SalesOrders.Any(e => e.Id == id);
        }
    }
}
