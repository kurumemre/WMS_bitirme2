using WMS_bitirme2.Data; // DbContext'imizin olduðu yer
using Microsoft.EntityFrameworkCore; // EF Core kütüphanesi
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace WMS_bitirme2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Veritabaný Servisini Ekliyoruz (BU KISIM ÇOK ÖNEMLÝ)
            // appsettings.json'dan "DefaultConnection" adresini okur ve WMSDbContext'e verir.
            builder.Services.AddDbContext<WMSDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // MVC (Controller ve View) servislerini ekliyoruz
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // HTTP istek hattý (Pipeline) ayarlarý
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

