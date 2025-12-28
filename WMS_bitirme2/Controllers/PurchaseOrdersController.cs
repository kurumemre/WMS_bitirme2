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
    public class PurchaseOrdersController : Controller
    {
        private readonly WMSDbContext _context;

        public PurchaseOrdersController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var wMSDbContext = _context.PurchaseOrders.Include(p => p.Supplier);
            return View(await wMSDbContext.ToListAsync());
        }

        // GET: PurchaseOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)       // Tedarikçi ismini görmek için
                .Include(p => p.Items)          // Siparişin satırlarını (Detayları) görmek için
                .ThenInclude(i => i.Product)    // Satırdaki Ürünün ismini görmek için
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchaseOrder == null) return NotFound();

            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderCode,SupplierId,OrderDate,Status,Notes")] PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Email", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: PurchaseOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderCode,SupplierId,OrderDate,Status,Notes")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. ESKİ DURUMU ÖĞREN
                    var eskiSiparis = await _context.PurchaseOrders
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(x => x.Id == id);

                    // Ürünleri hafızaya al (Hem eklerken hem çıkarırken lazım olacak)
                    var siparisDetaylari = _context.PurchaseOrderItems
                                           .Where(x => x.PurchaseOrderId == id)
                                           .ToList();

                    // ---------------------------------------------------------
                    // SENARYO A: Mal Kabul Yapılıyor (Stok ARTIR +)
                    // Hazırlanıyor -> Tamamlandı
                    // ---------------------------------------------------------
                    if (eskiSiparis.Status != PurchaseOrderStatus.Tamamlandi &&
                        purchaseOrder.Status == PurchaseOrderStatus.Tamamlandi)
                    {
                        foreach (var kalem in siparisDetaylari)
                        {
                            var urun = await _context.Products.FindAsync(kalem.ProductId);
                            if (urun != null)
                            {
                                urun.StokMiktari += kalem.Quantity; // ARTIR
                                _context.Update(urun);
                            }
                        }
                    }

                    // ---------------------------------------------------------
                    // SENARYO B: İşlemden Vazgeçiliyor (Stok AZALT -)
                    // Tamamlandı -> Hazırlanıyor VEYA Tamamlandı -> İptal
                    // ---------------------------------------------------------
                    else if (eskiSiparis.Status == PurchaseOrderStatus.Tamamlandi &&
                             purchaseOrder.Status != PurchaseOrderStatus.Tamamlandi)
                    {
                        foreach (var kalem in siparisDetaylari)
                        {
                            var urun = await _context.Products.FindAsync(kalem.ProductId);
                            if (urun != null)
                            {
                                urun.StokMiktari -= kalem.Quantity; // AZALT (Geri Al)
                                _context.Update(urun);
                            }
                        }
                    }
                    // ---------------------------------------------------------

                    // Siparişin kendisini güncelle
                    _context.Update(purchaseOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(purchaseOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrders.Any(e => e.Id == id);
        }
    }
}
