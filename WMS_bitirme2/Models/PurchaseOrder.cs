using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }

        [Display(Name = "Sipariş Kodu")]
        public string OrderCode { get; set; } // Örn: PO-2023-001 (Otomatik üreteceğiz)

        [Display(Name = "Tedarikçi")]
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Durum")]
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Hazirlaniyor;

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        // Bu siparişin içindeki ürünler 
        public List<PurchaseOrderItem>? Items { get; set; }
    }
}