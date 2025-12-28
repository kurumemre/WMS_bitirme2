using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Müşteri adı/ünvanı zorunludur.")]
        [Display(Name = "Müşteri / Firma Adı")]
        public string Name { get; set; }

        [Display(Name = "Yetkili Kişi")]
        public string? ContactPerson { get; set; }

        [EmailAddress]
        [Display(Name = "E-Posta")]
        public string? Email { get; set; } // Müşteride e-posta zorunlu olmasın (belki yoktur)

        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [Display(Name = "Vergi No / TC")]
        public string? TaxNumber { get; set; } // Fatura kesmek için lazım olur
    }
}