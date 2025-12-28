using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class Brand
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Marka adı zorunludur.")]
        [Display(Name = "Marka Adı")]
        public string Ad { get; set; }

       
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        // Markanın bağlı olduğu kategori
        public Category? Category { get; set; }
        // ------------------------------------------

        public List<Product>? Products { get; set; }
    }
}