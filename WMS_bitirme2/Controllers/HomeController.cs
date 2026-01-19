using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include ve ToListAsync için gerekli
using WMS_bitirme2.Data;
using WMS_bitirme2.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WMS_bitirme2.Controllers
{
    public class HomeController : Controller
    {
        private readonly WMSDbContext _context;

        public HomeController(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // --- 1. KARTLAR ÝÇÝN SAYISAL VERÝLER ---
            ViewBag.UrunCesidi = await _context.Products.CountAsync();

            // Veritabaný boþsa hata vermesin diye (?? 0) kontrolü
            ViewBag.ToplamStok = await _context.Products.SumAsync(x => (int?)x.StokMiktari) ?? 0;

            ViewBag.BekleyenAlim = await _context.PurchaseOrders.CountAsync(x => x.Status == PurchaseOrderStatus.Hazirlaniyor);
            ViewBag.BekleyenSatis = await _context.SalesOrders.CountAsync(x => x.Status == SalesOrderStatus.Hazirlaniyor);

            // --- 2. KRÝTÝK STOK LÝSTESÝ (Tablo Ýçin) ---
            // Stoðu 20'den az olan ürünleri getir (Eþik deðeri isteðine göre deðiþtirebilirsin)
            ViewBag.KritikUrunler = await _context.Products
                                            .Where(x => x.StokMiktari < 20)
                                            .OrderBy(x => x.StokMiktari) // En az olan en üstte
                                            .Take(5) // Sadece ilk 5 tanesini göster
                                            .ToListAsync();

            // --- 3. SON HAREKETLER (Akýþ Ýçin) ---
            // Stok hareket geçmiþinden en son yapýlanlarý çek
            ViewBag.SonHareketler = await _context.StockMovements
                                            .Include(x => x.Product) // Ürün adýný görmek için Include
                                            .OrderByDescending(x => x.Tarih) // En yeni en üstte
                                            .Take(6) // Son 6 iþlem
                                            .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}