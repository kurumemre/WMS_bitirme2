using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WMS_bitirme2.Data;
using WMS_bitirme2.Models;

namespace WMS_bitirme2.Controllers
{
    public class TransfersController : Controller
    {
        private readonly WMSDbContext _context;

        public TransfersController(WMSDbContext context)
        {
            _context = context;
        }

        // GET: Transfer Sayfasını Aç
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad");
            return View();
        }

        // POST: Transferi Gerçekleştir
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransferViewModel model)
        {
            // Dropdownları dolduran metodu en başta tanımlayalım ki her hatada çağırabilelim
            void DropdownlariDoldur()
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad", model.ProductId);
                ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. AYNI DEPOYA TRANSFER OLMAZ
                    if (model.SourceWarehouseId == model.TargetWarehouseId)
                    {
                        ModelState.AddModelError("", "HATA: Kaynak ve Hedef depo aynı olamaz!");
                        DropdownlariDoldur(); // ⚠️ Sayfa yeniden açılacağı için listeyi doldur
                        return View(model);
                    }

                    // 2. ÜRÜNÜ BUL VE KONTROL ET
                    var urun = await _context.Products.FindAsync(model.ProductId);

                    // Eğer ürün veritabanında bulunamazsa (Silinmiş olabilir)
                    if (urun == null)
                    {
                        ModelState.AddModelError("", "HATA: Seçilen ürün bulunamadı!");
                        DropdownlariDoldur();
                        return View(model);
                    }

                    // 3. STOK KONTROLÜ (ÇÖKMEYİ ENGELLEYEN KISIM) 🚨
                    if (urun.StokMiktari < model.Quantity)
                    {
                        // Çökmek yerine kullanıcıya mesaj gösteriyoruz
                        ModelState.AddModelError("Quantity", $"YETERSİZ STOK: Depoda sadece {urun.StokMiktari} adet ürün var, siz {model.Quantity} adet istediniz.");
                        DropdownlariDoldur(); // ⚠️ Listeyi tekrar doldurmazsak sayfa çöker!
                        return View(model);
                    }

                    // 4. İŞLEMİ YAP VE LOGLA

                    // A) Kaynak Depodan Çıkış Logu
                    var cikisHareketi = new StockMovement
                    {
                        ProductId = model.ProductId,
                        ShelfId = null,
                        Miktar = model.Quantity,
                        HareketTipi = MovementType.Cikis,
                        Tarih = DateTime.Now
                    };
                    _context.Add(cikisHareketi);

                    // B) Hedef Depoya Giriş Logu
                    var girisHareketi = new StockMovement
                    {
                        ProductId = model.ProductId,
                        ShelfId = null,
                        Miktar = model.Quantity,
                        HareketTipi = MovementType.Giris, // Dikkat: Burası 'Giris' olmalı
                        Tarih = DateTime.Now
                    };
                    _context.Add(girisHareketi);

                    // Veritabanına kaydet
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Transfer başarıyla tamamlandı!";
                    return RedirectToAction("Index", "StockMovements");
                }
                catch (Exception ex)
                {
                    // Beklenmedik bir veritabanı hatası olursa buraya düşer
                    ModelState.AddModelError("", "Sistem Hatası: " + ex.Message);
                    DropdownlariDoldur();
                    return View(model);
                }
            }

            // Model geçerli değilse (boş alan varsa)
            DropdownlariDoldur();
            return View(model);
        }

        private void RefreshDropdowns()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Ad");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Ad");
        }
    }
}