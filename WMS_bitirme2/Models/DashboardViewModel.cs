using System.Collections.Generic;

namespace WMS_bitirme2.Models
{
    public class DashboardViewModel
    {
        public int ToplamUrunSayisi { get; set; }
        public int ToplamDepoSayisi { get; set; }
        public int ToplamRafSayisi { get; set; }

        // Tabloda göstermek için Son 5 Hareket
        public List<StockMovement> SonHareketler { get; set; }
    }
}