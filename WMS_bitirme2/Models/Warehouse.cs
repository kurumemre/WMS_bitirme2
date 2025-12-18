namespace WMS_bitirme2.Models
{
    public class Warehouse
    {
        public int Id { get; set; }           // Depo ID'si
        public string Ad { get; set; }        // Örn: Gebze Depo
        public string Sehir { get; set; }     // Örn: Kocaeli
        public string Adres { get; set; }     // Açık adres

        // YENİ EKLENEN KISIM: Bir deponun rafları olur
        public ICollection<Shelf> Shelves { get; set; }
    }
}
