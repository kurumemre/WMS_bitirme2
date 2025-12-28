namespace WMS_bitirme2.Models
{
    // Siparişin o anki durumu
    public enum PurchaseOrderStatus
    {
        Hazirlaniyor = 0, // Sipariş oluşturuldu ama henüz mal gelmedi
        Tamamlandi = 1,   // Mallar depoya girdi, stok arttı
        Iptal = 2         // Sipariş iptal edildi
    }
}