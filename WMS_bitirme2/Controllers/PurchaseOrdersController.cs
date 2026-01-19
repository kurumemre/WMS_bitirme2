using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_bitirme2.Data;
using WMS_bitirme2.Models;
using Microsoft.AspNetCore.Authorization;

namespace WMS_bitirme2.Controllers
{
    [Authorize]
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
                .Include(p => p.Supplier)
                .Include(p => p.Items)            // Ürünleri getir
                    .ThenInclude(i => i.Product)  // Ürün detaylarını getir
                .Include(p => p.Items)            // Tekrar Items üzerinden...
                    .ThenInclude(i => i.Shelf)    // ✅ BU SATIR ŞART: Raf bilgisini getir!
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchaseOrder == null) return NotFound();

            return View(purchaseOrder);
        }

        // GET: PurchaseOrders/Create
        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad");
            return View();
        }

        // POST: PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderCode,SupplierId,OrderDate,Status,Notes,WarehouseId")] PurchaseOrder purchaseOrder)
        {
            // Bind içine WarehouseId eklendiği için artık veri doğru gelecek.

            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Eğer hata olursa dropdownları tekrar dolduruyoruz
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad", purchaseOrder.WarehouseId); // Burayı da düzelttim
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderCode,SupplierId,OrderDate,Status,Notes,WarehouseId")] PurchaseOrder purchaseOrder)
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
                                urun.StokMiktari += kalem.Quantity;
                                _context.Update(urun);

                                // ✅ YENİ: STOK HAREKETİ KAYDET (GİRİŞ)
                                var hareket = new StockMovement
                                {
                                    ProductId = kalem.ProductId,
                                    ShelfId = kalem.ShelfId, // Alımda raf bellidir
                                    Miktar = kalem.Quantity,
                                    HareketTipi = MovementType.Giris,
                                    Tarih = DateTime.Now
                                };
                                _context.Add(hareket); // Logu veritabanına ekle
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
                                urun.StokMiktari -= kalem.Quantity;
                                _context.Update(urun);

                                // ✅ YENİ: STOK HAREKETİ KAYDET (ÇIKIŞ - DÜZELTME)
                                var hareket = new StockMovement
                                {
                                    ProductId = kalem.ProductId,
                                    ShelfId = kalem.ShelfId,
                                    Miktar = kalem.Quantity,
                                    HareketTipi = MovementType.Cikis, // Giren malı geri çıktık
                                    Tarih = DateTime.Now
                                };
                                _context.Add(hareket);
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
                    if (!PurchaseOrderExists(purchaseOrder.Id))
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

            // Hata durumunda dropdownları doldur (Buraya WarehouseId'yi de ekledim, eksikti)
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad", purchaseOrder.WarehouseId); // EKLENDİ
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
