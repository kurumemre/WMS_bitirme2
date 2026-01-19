using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMS_bitirme2.Models;

namespace WMS_bitirme2.Controllers
{
    [Authorize(Roles = "Admin")] // 🔒 Sadece Mevcut Adminler Girebilir!
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // KULLANICILARI LİSTELE
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var thisViewModel = new UserViewModel();
                thisViewModel.Id = user.Id;
                thisViewModel.Email = user.Email;
                thisViewModel.Name = user.UserName;
                thisViewModel.Roles = new List<string>(await _userManager.GetRolesAsync(user));
                userViewModels.Add(thisViewModel);
            }

            return View(userViewModels);
        }

        // YÖNETİCİ YAPMA BUTONU ÇALIŞINCA BURAYA GELECEK
        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Önce varsa eski rollerini temizleyelim (Opsiyonel, temiz iş olsun diye)
                // await _userManager.RemoveFromRoleAsync(user, "User"); 

                // Admin rolünü ekle
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }

        // YÖNETİCİLİĞİ GERİ ALMA (İşçiye Çevir)
        [HttpPost]
        public async Task<IActionResult> RemoveAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                // Gerekirse User rolü geri verilebilir
                // await _userManager.AddToRoleAsync(user, "User");
            }
            return RedirectToAction("Index");
        }
    }
}