using Microsoft.AspNetCore.Mvc;
using WMS_bitirme2.Data;
using WMS_bitirme2.Models;
using System.Linq;

namespace WMS_bitirme2.Controllers
{
    public class HomeController : Controller
    {
        private readonly WMSDbContext _context;

        // Constructor'da veritabanýný baðlýyoruz
        public HomeController(WMSDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Ýstatistikleri hesaplayýp çantaya (ViewBag) atýyoruz

            // 1. Toplam Ürün Çeþidi
            ViewBag.UrunCesidi = _context.Products.Count();

            // 2. Toplam Stok Adedi (Tüm ürünlerin stoklarýnýn toplamý)
            // Eðer veritabaný boþsa hata vermesin diye (int?) nullable yaptýk
            ViewBag.ToplamStok = _context.Products.Sum(x => (int?)x.StokMiktari) ?? 0;

            // 3. Bekleyen (Hazýrlanýyor) Satýn Alma Sipariþleri
            ViewBag.BekleyenAlim = _context.PurchaseOrders.Count(x => x.Status == PurchaseOrderStatus.Hazirlaniyor);

            // 4. Bekleyen (Hazýrlanýyor) Satýþ Sipariþleri
            ViewBag.BekleyenSatis = _context.SalesOrders.Count(x => x.Status == SalesOrderStatus.Hazirlaniyor);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}