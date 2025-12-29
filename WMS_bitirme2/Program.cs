using WMS_bitirme2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Identity; // <--- 1. YEN� EKLENEN K�T�PHANE

namespace WMS_bitirme2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Veritaban� Servisini Ekliyoruz
            builder.Services.AddDbContext<WMSDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==================================================================
            // 2. IDENTITY (�YEL�K) SERV�S�N� BURAYA EKL�YORUZ (YEN� KISIM) ??
            // ==================================================================
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<WMSDbContext>();


            // MVC (Controller ve View) servislerini ekliyoruz
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // HTTP istek hatt� (Pipeline) ayarlar�
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ==================================================================
            // 3. K�ML�K DO�RULAMAYI BURAYA EKL�YORUZ (YEN� KISIM) ??
            // (Authorization'dan �NCE gelmek zorunda!)
            // ==================================================================
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Identity sayfalar� i�in gerekli route haritalamas�
            app.MapRazorPages();

            app.Run();
        }
    }
}