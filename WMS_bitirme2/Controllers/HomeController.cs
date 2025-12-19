using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include için gerekli
using System.Linq;
using System.Threading.Tasks;
using WMS_bitirme2.Data; // DbContext için
using WMS_bitirme2.Models; // Modeller için

namespace WMS_bitirme2.Controllers
{
    public class HomeController : Controller
    {
        // Veritabaný baðlantýsýný buraya çaðýrýyoruz
        private readonly WMSDbContext _context;

        public HomeController(WMSDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Verileri topluyoruz
            var model = new DashboardViewModel
            {
                ToplamUrunSayisi = await _context.Products.CountAsync(),
                ToplamDepoSayisi = await _context.Warehouses.CountAsync(),
                ToplamRafSayisi = await _context.Shelves.CountAsync(),

                // Son 5 hareketi getir (Tarihe göre tersten sýrala)
                SonHareketler = await _context.StockMovements
                                      .Include(x => x.Product)
                                      .Include(x => x.Shelf)
                                      .OrderByDescending(x => x.Tarih)
                                      .Take(5)
                                      .ToListAsync()
            };

            return View(model);
        }
    }
}