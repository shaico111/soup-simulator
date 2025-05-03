using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using HelloWorldWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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
                Console.WriteLine("❌ Access denied. User is not admin.");
                TempData["ErrorMessage"] = "❌ גישה נדחתה! אתה לא מנהל.";
                return Page();
            }

            Console.WriteLine("🔄 [OnGet] Admin access confirmed.");
            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostUnflagAsync(string username)
        {
            if (!IsAdmin())
            {
                Console.WriteLine("❌ Access denied. User is not admin.");
                TempData["ErrorMessage"] = "❌ גישה נדחתה! אתה לא מנהל.";
                return Page();
            }

            Console.WriteLine($"🟢 [Unflag] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsCheater = false;
                await _authService.UpdateUser(user);
                Console.WriteLine($"✅ [Unflag] Updated {username} successfully.");
                TempData["SuccessMessage"] = $"✅ המשתמש '{username}' שוחרר בהצלחה.";
            }
            else
            {
                Console.WriteLine($"❌ [Unflag] User {username} not found.");
                TempData["ErrorMessage"] = $"❌ שגיאה בשחרור המשתמש '{username}'.";
            }

            return Page(); // לא מעבר לדף אחר
        }

        public async Task<IActionResult> OnPostBanAsync(string username)
        {
            if (!IsAdmin())
            {
                Console.WriteLine("❌ Access denied. User is not admin.");
                TempData["ErrorMessage"] = "❌ גישה נדחתה! אתה לא מנהל.";
                return Page();
            }

            Console.WriteLine($"🚫 [Ban] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = true;
                await _authService.UpdateUser(user);
                Console.WriteLine($"✅ [Ban] Updated {username} successfully.");
                TempData["SuccessMessage"] = $"🚫 המשתמש '{username}' נחסם.";
            }
            else
            {
                Console.WriteLine($"❌ [Ban] User {username} not found.");
                TempData["ErrorMessage"] = $"❌ שגיאה בחסימת המשתמש '{username}'.";
            }

            return Page(); // לא מעבר לדף אחר
        }

        public async Task<IActionResult> OnPostUnbanAsync(string username)
        {
            if (!IsAdmin())
            {
                Console.WriteLine("❌ Access denied. User is not admin.");
                TempData["ErrorMessage"] = "❌ גישה נדחתה! אתה לא מנהל.";
                return Page();
            }

            Console.WriteLine($"🔓 [Unban] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = false;
                await _authService.UpdateUser(user);
                Console.WriteLine($"✅ [Unban] Updated {username} successfully.");
                TempData["SuccessMessage"] = $"🔓 המשתמש '{username}' שוחרר מחסימה.";
            }
            else
            {
                Console.WriteLine($"❌ [Unban] User {username} not found.");
                TempData["ErrorMessage"] = $"❌ שגיאה בשחרור חסימה של '{username}'.";
            }

            return Page(); // לא מעבר לדף אחר
        }

        public async Task<IActionResult> OnPostDeleteAsync(string username)
        {
            if (!IsAdmin())
            {
                Console.WriteLine("❌ Access denied. User is not admin.");
                TempData["ErrorMessage"] = "❌ גישה נדחתה! אתה לא מנהל.";
                return Page();
            }

            Console.WriteLine($"🗑️ [Delete] Requested for: {username}");
            var success = await _authService.DeleteUser(username);
            if (success)
            {
                Console.WriteLine($"✅ [Delete] User {username} deleted.");
                TempData["SuccessMessage"] = $"🗑️ המשתמש '{username}' נמחק.";
            }
            else
            {
                Console.WriteLine($"❌ [Delete] Failed to delete {username}.");
                TempData["ErrorMessage"] = $"❌ שגיאה במחיקת המשתמש '{username}'.";
            }

            return Page(); // לא מעבר לדף אחר
        }

        private bool IsAdmin()
        {
            var sessionUser = HttpContext.Session.GetString("Username");
            Console.WriteLine($"🔑 [IsAdmin] Session username: {sessionUser}");
            return sessionUser == "Admin";
        }

        private async Task LoadData()
        {
            AllUsers = await _authService.GetAllUsers();
            Cheaters = AllUsers.Where(u => u.IsCheater).ToList();
            BannedUsers = AllUsers.Where(u => u.IsBanned).ToList();
            OnlineUsers = AllUsers
                .Where(u => u.LastSeen != null && u.LastSeen > DateTime.UtcNow.AddMinutes(-5))
                .ToList();
            TopUsers = AllUsers.OrderByDescending(u => u.CorrectAnswers).Take(5).ToList();
            AverageSuccessRate = AllUsers
                .Where(u => u.TotalAnswered > 0)
                .Select(u => (double)u.CorrectAnswers / u.TotalAnswered)
                .DefaultIfEmpty(0)
                .Average() * 100;

            Console.WriteLine("🔄 [LoadData] Loaded user data successfully.");
        }
    }
}
