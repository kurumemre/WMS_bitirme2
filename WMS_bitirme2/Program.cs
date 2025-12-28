using WMS_bitirme2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Identity; // <--- 1. YENÝ EKLENEN KÜTÜPHANE

namespace WMS_bitirme2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Veritabaný Servisini Ekliyoruz
            builder.Services.AddDbContext<WMSDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==================================================================
            // 2. IDENTITY (ÜYELÝK) SERVÝSÝNÝ BURAYA EKLÝYORUZ (YENÝ KISIM) ??
            // ==================================================================
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<WMSDbContext>();


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

            // ==================================================================
            // 3. KÝMLÝK DOÐRULAMAYI BURAYA EKLÝYORUZ (YENÝ KISIM) ??
            // (Authorization'dan ÖNCE gelmek zorunda!)
            // ==================================================================
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Identity sayfalarý için gerekli route haritalamasý
            app.MapRazorPages();

            app.Run();
        }
    }
}