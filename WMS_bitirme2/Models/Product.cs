namespace WMS_bitirme2.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string BarkodNo { get; set; }  // Ürün barkodu
        public string Ad { get; set; }        // Ürün adı
        public string Tanim { get; set; }     // Açıklama
        public decimal Fiyat { get; set; }    // Fiyatı
        public int StokMiktari { get; set; }  // Kaç tane var?
    }
}
