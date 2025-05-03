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
            Console.WriteLine("🔄 [OnGet] Admin access confirmed.");
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ Access denied. Only admins can access this page.";
                return RedirectToPage("/Index");
            }

            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostUnflagAsync(string username)
        {
            Console.WriteLine($"🔥 [DEBUG] Entered OnPostUnflagAsync with {username}");
            if (!IsAdmin()) return RedirectWithAccessDenied();

            var user = await _authService.GetUser(username);
            if (user == null) return HandleUserNotFound("Unflag", username);

            user.IsCheater = false;
            await _authService.UpdateUser(user);
            TempData["SuccessMessage"] = $"✅ שוחרר הסימון על '{username}'";
            Console.WriteLine($"✅ [Unflag] Updated {username}");

            await LoadData();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostBanAsync(string username)
        {
            Console.WriteLine($"🔥 [DEBUG] Entered OnPostBanAsync with {username}");
            if (!IsAdmin()) return RedirectWithAccessDenied();

            var user = await _authService.GetUser(username);
            if (user == null) return HandleUserNotFound("Ban", username);

            user.IsBanned = true;
            await _authService.UpdateUser(user);
            TempData["SuccessMessage"] = $"🚫 המשתמש '{username}' נחסם.";
            Console.WriteLine($"✅ [Ban] Updated {username}");

            await LoadData();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnbanAsync(string username)
        {
            Console.WriteLine($"🔥 [DEBUG] Entered OnPostUnbanAsync with {username}");
            if (!IsAdmin()) return RedirectWithAccessDenied();

            var user = await _authService.GetUser(username);
            if (user == null) return HandleUserNotFound("Unban", username);

            user.IsBanned = false;
            await _authService.UpdateUser(user);
            TempData["SuccessMessage"] = $"🔓 המשתמש '{username}' שוחרר מהחסימה.";
            Console.WriteLine($"✅ [Unban] Updated {username}");

            await LoadData();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string username)
        {
            Console.WriteLine($"🔥 [DEBUG] Entered OnPostDeleteAsync with {username}");
            if (!IsAdmin()) return RedirectWithAccessDenied();

            var success = await _authService.DeleteUser(username);
            if (success)
            {
                TempData["SuccessMessage"] = $"🗑️ המשתמש '{username}' נמחק.";
                Console.WriteLine($"✅ [Delete] User {username} deleted.");
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ שגיאה במחיקת המשתמש '{username}'.";
                Console.WriteLine($"❌ [Delete] Failed to delete {username}.");
            }

            await LoadData();
            return RedirectToPage();
        }

        private IActionResult RedirectWithAccessDenied()
        {
            TempData["SuccessMessage"] = "❌ גישה נדחתה. רק מנהלים רשאים.";
            return RedirectToPage("/Index");
        }

        private IActionResult HandleUserNotFound(string action, string username)
        {
            TempData["SuccessMessage"] = $"❌ שגיאה ב־{action} למשתמש '{username}'.";
            Console.WriteLine($"❌ [{action}] User {username} not found.");
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
            AverageSuccessRate = AllUsers
                .Where(u => u.TotalAnswered > 0)
                .Select(u => (double)u.CorrectAnswers / u.TotalAnswered)
                .DefaultIfEmpty(0)
                .Average() * 100;

            Console.WriteLine($"🔄 [LoadData] Cheaters: {Cheaters.Count}, Banned: {BannedUsers.Count}, Online: {OnlineUsers.Count}");
        }
    }
}
