using System;
using System.ComponentModel.DataAnnotations;

namespace WMS_bitirme2.Models
{
    // Önce Hareket Tipini belirleyelim (Giriş mi Çıkış mı?)
    public enum MovementType
    {
        Giris = 1, // Stok Ekleme
        Cikis = 2  // Stok Düşme
    }

    public class StockMovement
    {
        public int Id { get; set; }

        // --- HANGİ ÜRÜN? ---
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // --- HANGİ RAF? ---
        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; }

        // --- DETAYLAR ---
        public int Miktar { get; set; } // Kaç adet?
        public MovementType HareketTipi { get; set; } // Giriş mi, Çıkış mı?
        public DateTime Tarih { get; set; } // İşlem ne zaman yapıldı?
    }
}