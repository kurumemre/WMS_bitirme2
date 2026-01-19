using WMS_bitirme2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Identity;

namespace WMS_bitirme2
{
    public class Program
    {
        // DİKKAT: "void Main" yerine "async Task Main" 👇
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Veritabanı Servisini Ekliyoruz
            builder.Services.AddDbContext<WMSDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. IDENTITY (ÜYELİK) SERVİSİNİ BURAYA EKLİYORUZ
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>() // ROL DESTEĞİ EKLENDİ 👈
                .AddEntityFrameworkStores<WMSDbContext>();

            // MVC (Controller ve View) servislerini ekliyoruz
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // HTTP istek hattı (Pipeline) ayarları
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 3. KİMLİK DOĞRULAMAYI BURAYA EKLİYORUZ
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Identity sayfaları için gerekli route haritalaması
            app.MapRazorPages();

            // --- SEEDER BAŞLANGIÇ ---
            // Uygulama başlarken rolleri kontrol et ve oluştur
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await DbSeeder.SeedRolesAndAdminAsync(services); // ✅ Artık await kullanılabilir
            }
            // --- SEEDER BİTİŞ ---

            await app.RunAsync(); // ✅ RunAsync kullanıyoruz (opsiyonel ama daha tutarlı)
        }
    }
}   