using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using HelloWorldWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldWeb.Pages
{
    public class AdminModel : PageModel
    {
        private readonly AuthService _authService;

        public AdminModel(AuthService authService)
        {
            _authService = authService;
        }

        public List<User> AllUsers { get; set; } = new();
        public List<User> Cheaters { get; set; } = new();
        public List<User> BannedUsers { get; set; } = new();
        public List<User> OnlineUsers { get; set; } = new();
        public List<User> TopUsers { get; set; } = new();
        public double AverageSuccessRate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ Access denied. Only admins can access this page.";
                return RedirectToPage("/Index");
            }

            Console.WriteLine("🔄 [OnGet] Admin access confirmed.");
            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostUnflagAsync(string username)
        {
            Console.WriteLine($"🟢 [Unflag] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsCheater = false;
                user.IsBanned = false;
                await _authService.UpdateUser(user);
                Console.WriteLine($"✅ [Unflag] {username} updated: IsCheater=false, IsBanned=false");
                TempData["SuccessMessage"] = $"✅ שוחרר המשתמש '{username}' מכל ההגבלות.";
            }
            else
            {
                Console.WriteLine($"❌ [Unflag] User not found: {username}");
                TempData["SuccessMessage"] = $"❌ המשתמש '{username}' לא נמצא.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostBanAsync(string username)
        {
            Console.WriteLine($"🚫 [Ban] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = true;
                await _authService.UpdateUser(user);
                Console.WriteLine($"✅ [Ban] {username} updated: IsBanned=true");
                TempData["SuccessMessage"] = $"🚫 המשתמש '{username}' נחסם.";
            }
            else
            {
                Console.WriteLine($"❌ [Ban] User not found: {username}");
                TempData["SuccessMessage"] = $"❌ המשתמש '{username}' לא נמצא.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnbanAsync(string username)
        {
            Console.WriteLine($"🔓 [Unban] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = false;
                await _authService.UpdateUser(user);
                Console.WriteLine($"✅ [Unban] {username} updated: IsBanned=false");
                TempData["SuccessMessage"] = $"🔓 המשתמש '{username}' שוחרר.";
            }
            else
            {
                Console.WriteLine($"❌ [Unban] User not found: {username}");
                TempData["SuccessMessage"] = $"❌ המשתמש '{username}' לא נמצא.";
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string username)
        {
            Console.WriteLine($"🗑️ [Delete] Requested for: {username}");
            var success = await _authService.DeleteUser(username);
            if (success)
            {
                Console.WriteLine($"✅ [Delete] Deleted user: {username}");
                TempData["SuccessMessage"] = $"🗑️ המשתמש '{username}' נמחק.";
            }
            else
            {
                Console.WriteLine($"❌ [Delete] Failed to delete: {username}");
                TempData["SuccessMessage"] = $"❌ שגיאה במחיקת המשתמש '{username}'.";
            }
            return RedirectToPage();
        }

        private bool IsAdmin()
        {
            var sessionUser = HttpContext.Session.GetString("Username");
            return sessionUser == "Admin";
        }

        private async Task LoadData()
        {
            AllUsers = await _authService.GetAllUsers();
            Cheaters = AllUsers.Where(u => u.IsCheater).ToList();
            BannedUsers = AllUsers.Where(u => u.IsBanned).ToList();
            OnlineUsers = AllUsers.Where(u => u.LastSeen != null && u.LastSeen > DateTime.UtcNow.AddMinutes(-5)).ToList();
            TopUsers = AllUsers.OrderByDescending(u => u.CorrectAnswers).Take(5).ToList();
            AverageSuccessRate = AllUsers.Where(u => u.TotalAnswered > 0).Select(u => (double)u.CorrectAnswers / u.TotalAnswered).DefaultIfEmpty(0).Average() * 100;
        }
    }
}
