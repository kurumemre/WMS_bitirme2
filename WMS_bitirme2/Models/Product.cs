using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WMS_bitirme2.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Barkod No")]
        public string BarkodNo { get; set; }

        [Required(ErrorMessage = "Ürün adı boş geçilemez.")]
        [Display(Name = "Ürün Adı")]
        public string Ad { get; set; }

        [Display(Name = "Açıklama")]
        public string Tanim { get; set; }

        [Display(Name = "Fiyat")]
        public decimal Fiyat { get; set; }

        [Display(Name = "Stok Miktarı")]
        public int StokMiktari { get; set; }

        // --- DÜZELTİLEN YERLER (Soru İşaretlerine Dikkat!) ---

        [Display(Name = "Kategori")]
        public int? CategoryId { get; set; }
        // 👇 Buraya ? ekledik
        public Category? Category { get; set; }

        [Display(Name = "Marka")]
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; } // Bu zaten doğruydu

        [Display(Name = "Birim")]
        public int? UnitId { get; set; }
        // 👇 Buraya ? ekledik
        public Unit? Unit { get; set; }
    }
}