using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class Unit
    {
        //(Birim) (Adet, Koli, Kg, Litre gibi ayrımlar için)
        public int Id { get; set; }

        [Required]
        [Display(Name = "Birim Adı")]
        public string Ad { get; set; } // Örn: Adet, Koli, Paket

        [Display(Name = "Kısaltma")]
        public string Kisaltma { get; set; } // Örn: ad., kg, lt.

        public List<Product>? Products { get; set; }
    }
}