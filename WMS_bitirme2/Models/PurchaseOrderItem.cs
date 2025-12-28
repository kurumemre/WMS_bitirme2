using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }

        // Hangi Siparişe ait?
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        // Hangi Ürün?
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        // Kaç tane?
        [Display(Name = "Miktar")]
        [Range(1, 10000, ErrorMessage = "En az 1 adet girmelisiniz.")]
        public int Quantity { get; set; }

        // Kaça alındı? (Alış Fiyatı)
        // Ürünün kendi fiyatından farklı olabilir (indirim vs.)
        [Display(Name = "Birim Fiyat")]
        public decimal UnitPrice { get; set; }

        // Toplam Tutar (Miktar * Fiyat) - Veritabanında tutmaya gerek yok, hesaplanır
        // Ama ekranda göstermek için property ekleyebiliriz
        public decimal LineTotal => Quantity * UnitPrice;
    }
}