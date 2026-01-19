using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WMS_bitirme2.Models;

namespace WMS_bitirme2.Data
{
    public class WMSDbContext : IdentityDbContext<IdentityUser>
    {
        public WMSDbContext(DbContextOptions<WMSDbContext> options) : base(options)
        {
        }

        // Mevcut Tablolar
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }

        // YENİ EKLENEN TABLOLAR (İsimleri Çoğul Yaptık!)
        // Category -> Categories
        public DbSet<Category> Categories { get; set; } = default!;
        
        // Brand -> Brands
        public DbSet<Brand> Brands { get; set; } = default!;
        
        // Unit -> Units
        public DbSet<Unit> Units { get; set; } = default!;

        // Supplier -> Suppliers
        public DbSet<Supplier> Suppliers { get; set; } = default!;

        // Customer -> Customers
        public DbSet<Customer> Customers { get; set; } = default!;

        // PurchaseOrder -> PurchaseOrders
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = default!;

        // PurchaseOrderItem -> PurchaseOrderItems
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = default!;

        // SalesOrder -> SalesOrders
        public DbSet<SalesOrder> SalesOrders { get; set; } = default!;

        // SalesOrderItem -> SalesOrderItems
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Bunu silme sakın!

            // --- SORUNU ÇÖZEN KOD BAŞLANGICI ---

            // PurchaseOrder (Sipariş) ile Warehouse (Depo) arasındaki ilişki
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.Warehouse)
                .WithMany() // Bir deponun çok siparişi olur
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 İŞTE ÇÖZÜM: Depo silinirse siparişleri silme!

            // Aynı hatayı PurchaseOrderItem (Kalemler) ve Shelf (Raf) için de alabilirsin, onu da sağlama alalım:
            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(p => p.Shelf)
                .WithMany()
                .HasForeignKey(p => p.ShelfId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 Raf silinirse, içindeki sipariş kaydını silme!

            // --- SORUNU ÇÖZEN KOD BİTİŞİ ---
        }
    }
}