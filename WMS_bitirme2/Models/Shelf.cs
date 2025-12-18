namespace WMS_bitirme2.Models
{
    public class Shelf
    {
        public int Id { get; set; }

        public string Kod { get; set; } // Örn: A-101, B-05
        public int Kapasite { get; set; } // Bu raf kaç ürün alır?

        // --- İLİŞKİ AYARLARI ---
        // Hangi depoya ait?
        public int WarehouseId { get; set; }

        // Bu kod sayesinde rafın hangi depoda olduğunu C# tarafında görebileceğiz
        public Warehouse Warehouse { get; set; }
    }
}