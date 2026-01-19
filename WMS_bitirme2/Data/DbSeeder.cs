using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace WMS_bitirme2.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            // 1. ROLLERİ OLUŞTUR (Hata veren kısım burasıydı, şimdi düzeldi)
            await CreateRoleAsync(roleManager, "Admin");
            await CreateRoleAsync(roleManager, "User");

            // 2. ADMİN KULLANCISINI BUL
            var adminUser = await userManager.FindByEmailAsync("admin@wms.com");

            // Eğer admin kullanıcısı HİÇ YOKSA oluştur
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = "admin@wms.com",
                    Email = "admin@wms.com",
                    EmailConfirmed = true
                };
                // Admin şifresi oluştur
                await userManager.CreateAsync(adminUser, "Admin123!");
            }

            // 3. KRİTİK DÜZELTME: KULLANICI VARSA BİLE ROLÜNÜ KONTROL ET
            // Eğer "Admin" rolü atanmamışsa, hemen şimdi ata!
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        
        private static async Task CreateRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}