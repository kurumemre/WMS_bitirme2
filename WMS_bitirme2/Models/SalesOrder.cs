using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class SalesOrder
    {
        public int Id { get; set; }

        [Display(Name = "Sipariş Kodu")]
        public string OrderCode { get; set; } // Örn: SATIS-2025-001

        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }
        // DİKKAT: Burada Supplier değil Customer var 👇
        public Customer? Customer { get; set; }

        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Durum")]
        public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Hazirlaniyor;

        [Display(Name = "Notlar")]
        public string? Notes { get; set; }

        // Siparişin detayları (Satırlar)
        public List<SalesOrderItem>? Items { get; set; }
    }
}