using Microsoft.AspNetCore.Mvc;
using WMS_bitirme2.Data;
using System.Linq;

namespace WMS_bitirme2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsApiController : ControllerBase
    {
        private readonly WMSDbContext _context;

        public BrandsApiController(WMSDbContext context)
        {
            _context = context;
        }

        // URL Şöyle Olacak: /api/brandsapi/getbrands/5
        [HttpGet("GetBrands/{categoryId}")]
        public IActionResult GetBrands(int categoryId)
        {
            // Veritabanından o kategoriye ait markaları bul
            var markalar = _context.Brands
                .Where(x => x.CategoryId == categoryId)
                // DİKKAT: Burası önemli! Tüm veriyi değil, sadece lazım olanı alıyoruz.
                // Buna "Data Projection" denir, performansı uçurur.
                .Select(x => new {
                    x.Id,
                    x.Ad
                })
                .ToList();

            return Ok(markalar); // JSON olarak geri döndür (RESTful Cevap)
        }
    }
}