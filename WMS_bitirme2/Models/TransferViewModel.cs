using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    public class TransferViewModel
    {
        [Display(Name = "Hangi Ürün?")]
        public int ProductId { get; set; }

        [Display(Name = "Kaynak Depo (Nereden?)")]
        public int SourceWarehouseId { get; set; }

        [Display(Name = "Hedef Depo (Nereye?)")]
        public int TargetWarehouseId { get; set; }

        [Display(Name = "Adet")]
        [Range(1, 10000, ErrorMessage = "En az 1 adet transfer edilmelidir.")]
        public int Quantity { get; set; }
    }
}