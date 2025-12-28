using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class SalesOrderItem
    {
        public int Id { get; set; }

        // Hangi Satış Siparişi?
        public int SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }

        // Hangi Ürün?
        [Display(Name = "Ürün")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Display(Name = "Miktar")]
        [Range(1, 10000, ErrorMessage = "En az 1 adet girmelisiniz.")]
        public int Quantity { get; set; }

        [Display(Name = "Birim Fiyat")]
        public decimal UnitPrice { get; set; }

        // Toplam Tutar (Miktar * Fiyat)
        public decimal LineTotal => Quantity * UnitPrice;
    }
}