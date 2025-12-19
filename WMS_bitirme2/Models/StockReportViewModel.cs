namespace WMS_bitirme2.Models
{
    public class StockReportViewModel
    {
        public string UrunAdi { get; set; }
        public string RafKodu { get; set; }
        public string DepoAdi { get; set; }
        public int ToplamGiris { get; set; }
        public int ToplamCikis { get; set; }

        // En önemli kısım burası: Kalan
        public int MevcutStok { get; set; }
    }
}