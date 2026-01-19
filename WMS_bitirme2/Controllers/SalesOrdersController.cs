using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMS_bitirme2.Data;
using WMS_bitirme2.Models;

namespace WMS_bitirme2.Controllers
{
    [Authorize]
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

            // ✅ EKSİK OLAN BU SATIRI EKLE:
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad");

            return View();
        }

        // POST: SalesOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // ✅ DÜZELTME 1: Bind içine WarehouseId eklendi
        public async Task<IActionResult> Create([Bind("Id,OrderCode,CustomerId,OrderDate,Status,Notes,WarehouseId")] SalesOrder salesOrder)
        {
            // Navigation property çakışmasını önle
            salesOrder.Warehouse = null;

            if (ModelState.IsValid)
            {
                _context.Add(salesOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ✅ DÜZELTME 2: Dropdownlar tekrar dolduruluyor (Hata olursa sayfa boş gelmesin)
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad", salesOrder.WarehouseId); // EKLENDİ
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderCode,CustomerId,OrderDate,Status,Notes,WarehouseId")] SalesOrder salesOrder)
        {
            if (id != salesOrder.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. ESKİ DURUMU ÖĞREN
                    var eskiSiparis = await _context.SalesOrders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                    var siparisDetaylari = _context.SalesOrderItems.Where(x => x.SalesOrderId == id).ToList();

                    // ---------------------------------------------------------
                    // SENARYO A: Satış Tamamlanıyor (Stoktan DÜŞ -)
                    // Hazırlanıyor -> Tamamlandı
                    // ---------------------------------------------------------
                    if (eskiSiparis.Status != SalesOrderStatus.Tamamlandi && salesOrder.Status == SalesOrderStatus.Tamamlandi)
                    {
                        // 🔥 KRİTİK KONTROL: Önce Stok Yeterli mi diye bak!
                        foreach (var kalem in siparisDetaylari)
                        {
                            var urunKontrol = await _context.Products.FindAsync(kalem.ProductId);
                            if (urunKontrol != null && urunKontrol.StokMiktari < kalem.Quantity)
                            {
                                // HATA FIRLAT VE DURDUR!
                                ModelState.AddModelError("", $"HATA: '{urunKontrol.Ad}' ürünü için stok yetersiz! (Mevcut: {urunKontrol.StokMiktari}, İstenen: {kalem.Quantity})");

                                // Dropdownları doldur ve sayfayı geri gönder
                                ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
                                ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad", salesOrder.WarehouseId);
                                return View(salesOrder);
                            }
                        }

                        // Stok Yeterliyse Düşüşü Yap
                        foreach (var kalem in siparisDetaylari)
                        {
                            var urun = await _context.Products.FindAsync(kalem.ProductId);
                            if (urun != null)
                            {
                                urun.StokMiktari -= kalem.Quantity; // AZALT 📉
                                _context.Update(urun);

                                // ✅ YENİ: STOK HAREKETİ KAYDET (ÇIKIŞ)
                                var hareket = new StockMovement
                                {
                                    ProductId = kalem.ProductId,
                                    ShelfId = null, // Satışta raf seçimi şimdilik yok
                                    Miktar = kalem.Quantity,
                                    HareketTipi = MovementType.Cikis,
                                    Tarih = DateTime.Now
                                };
                                _context.Add(hareket);
                            }
                        }
                    }

                    // ---------------------------------------------------------
                    // SENARYO B: İptal/İade (Stoku GERİ YÜKLE +)
                    // Tamamlandı -> Hazırlanıyor/İptal
                    // ---------------------------------------------------------
                    else if (eskiSiparis.Status == SalesOrderStatus.Tamamlandi && salesOrder.Status != SalesOrderStatus.Tamamlandi)
                    {
                        foreach (var kalem in siparisDetaylari)
                        {
                            var urun = await _context.Products.FindAsync(kalem.ProductId);
                            if (urun != null)
                            {
                                urun.StokMiktari += kalem.Quantity; // ARTTIR (İade al) 📈
                                _context.Update(urun);

                                // ✅ YENİ: STOK HAREKETİ KAYDET (GİRİŞ - İADE)
                                var hareket = new StockMovement
                                {
                                    ProductId = kalem.ProductId,
                                    ShelfId = null,
                                    Miktar = kalem.Quantity,
                                    HareketTipi = MovementType.Giris, // Mal geri geldi
                                    Tarih = DateTime.Now
                                };
                                _context.Add(hareket);
                            }
                        }
                    }
                    // ---------------------------------------------------------

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

            // Hata durumunda dropdownları doldur
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", salesOrder.CustomerId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad", salesOrder.WarehouseId);
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
