using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Firma adı zorunludur.")]
        [Display(Name = "Firma Adı")]
        public string Name { get; set; } // Örn: Samsung Türkiye, ABC Lojistik

        [Display(Name = "Yetkili Kişi")]
        public string? ContactPerson { get; set; } // Örn: Ahmet Yılmaz

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-Posta")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Display(Name = "Adres")]
        public string? Address { get; set; }

        // İleride bu tedarikçiden yaptığımız alımları burada görebiliriz
        // public List<PurchaseOrder>? PurchaseOrders { get; set; } 
    }
}