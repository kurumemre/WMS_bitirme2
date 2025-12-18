using Microsoft.EntityFrameworkCore;
using WMS_bitirme2.Models;

namespace WMS_bitirme2.Data
{
    public class WMSDbContext : DbContext
    {
        public WMSDbContext(DbContextOptions<WMSDbContext> options) : base(options)
        {
        }

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }

        // YENİ EKLENEN:
        public DbSet<Shelf> Shelves { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
    }
}