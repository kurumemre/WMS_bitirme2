namespace WMS_bitirme2.Models
{
    public enum SalesOrderStatus
    {
        Hazirlaniyor = 0, // Sipariş alındı, depoda hazırlanıyor
        Tamamlandi = 1,   // Faturalandı ve stoktan düştü (Sevkedildi)
        Iptal = 2         // Müşteri vazgeçti
    }
}