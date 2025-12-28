using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [Display(Name = "Kategori Adı")]
        public string Ad { get; set; }

        // Bir kategoride binlerce ürün olabilir (İlişki)
        public List<Product>? Products { get; set; }
    }
}