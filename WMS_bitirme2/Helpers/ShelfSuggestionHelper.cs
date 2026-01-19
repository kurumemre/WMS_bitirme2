using WMS_bitirme2.Data;
using WMS_bitirme2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WMS_bitirme2.Helpers
{
    public class ShelfSuggestionHelper
    {
        private readonly WMSDbContext _context;

        public ShelfSuggestionHelper(WMSDbContext context)
        {
            _context = context;
        }

        // Bize en uygun rafı öneren metot
        public int OneriGetir(int urunId, int adet, int depoId)
        {
            // 1. ADIM: AYNI ÜRÜNÜN OLDUĞU RAFLARI BUL (KURAL 1)
            // Bu ürünün daha önce girdiği rafları bulalım.
            // ÖNEMLİ DÜZELTME: ShelfId null olmayanları alıyoruz (.Value diyerek int'e çeviriyoruz)
            var urunluRaflar = _context.StockMovements
                .Include(x => x.Shelf) // Depo kontrolü için Rafı dahil et
                .Where(x => x.ProductId == urunId
                         && x.ShelfId != null // ✅ DÜZELTME: Rafsız hareketleri (Satış vb.) yoksay
                         && x.Shelf.WarehouseId == depoId)
                .Select(x => x.ShelfId.Value) // ✅ DÜZELTME: "Nullable int"i "Normal int"e çevir
                .Distinct()
                .ToList();

            foreach (var rafId in urunluRaflar)
            {
                if (SigarMi(rafId, adet))
                {
                    return rafId; // Bulduk! Buraya koyabilirsin.
                }
            }

            // 2. ADIM: EĞER BULAMADIYSAK, BOŞ RAFLARA BAK (KURAL 2)
            // Hiç hareket görmemiş veya stoğu sıfıra inmiş raflar
            var tumRaflar = _context.Shelves.Where(x => x.WarehouseId == depoId).ToList();

            foreach (var raf in tumRaflar)
            {
                // Rafın içindeki toplam ürün sayısını hesapla
                int mevcutDoluluk = StokSay(raf.Id);

                if (mevcutDoluluk == 0 && raf.Kapasite >= adet) // Raf boşsa ve kapasite yetiyorsa
                {
                    return raf.Id;
                }
            }

            // 3. ADIM: HİÇBİR YER YOKSA
            return 0; // "0" demek "Yer bulamadım" demek.
        }

        // Yardımcı Metot: Bir rafın içine o kadar mal sığar mı?
        private bool SigarMi(int rafId, int eklenecekAdet)
        {
            var raf = _context.Shelves.Find(rafId);
            if (raf == null) return false;

            int mevcutStok = StokSay(rafId);

            // Kapasite kontrolü (Örn: Kapasite 100, Mevcut 80, Gelen 30 -> Sığmaz)
            return (raf.Kapasite - mevcutStok) >= eklenecekAdet;
        }

        // Yardımcı Metot: Rafın içindeki güncel stok sayısını hesaplar
        private int StokSay(int rafId)
        {
            // Girişleri topla
            var girisler = _context.StockMovements
                .Where(x => x.ShelfId == rafId && x.HareketTipi == MovementType.Giris)
                .Sum(x => x.Miktar);

            // Çıkışları topla
            var cikislar = _context.StockMovements
                .Where(x => x.ShelfId == rafId && x.HareketTipi == MovementType.Cikis)
                .Sum(x => x.Miktar);

            return girisler - cikislar;
        }
    }
}