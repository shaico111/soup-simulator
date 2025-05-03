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
                TempData["SuccessMessage"] = "❌ גישה נדחתה. רק מנהל יכול לגשת לעמוד זה.";
                return RedirectToPage("/Index");
            }

            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostUnflagAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ גישה נדחתה. רק מנהל יכול לגשת לעמוד זה.";
                return RedirectToPage("/Index");
            }

            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsCheater = false;
                await _authService.UpdateUser(user);
                TempData["SuccessMessage"] = $"✅ המשתמש '{username}' שוחרר בהצלחה.";
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ שגיאה בשחרור המשתמש '{username}'.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostBanAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ גישה נדחתה. רק מנהל יכול לגשת לעמוד זה.";
                return RedirectToPage("/Index");
            }

            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = true;
                await _authService.UpdateUser(user);
                TempData["SuccessMessage"] = $"🚫 המשתמש '{username}' נחסם.";
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ שגיאה בחסימת המשתמש '{username}'.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnbanAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ גישה נדחתה. רק מנהל יכול לגשת לעמוד זה.";
                return RedirectToPage("/Index");
            }

            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = false;
                await _authService.UpdateUser(user);
                TempData["SuccessMessage"] = $"🔓 המשתמש '{username}' שוחרר מחסימה.";
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ שגיאה בשחרור חסימה של '{username}'.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ גישה נדחתה. רק מנהל יכול לגשת לעמוד זה.";
                return RedirectToPage("/Index");
            }

            var success = await _authService.DeleteUser(username);
            if (success)
            {
                TempData["SuccessMessage"] = $"🗑️ המשתמש '{username}' נמחק.";
            }
            else
            {
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
            OnlineUsers = AllUsers
                .Where(u => u.LastSeen != null && u.LastSeen > DateTime.UtcNow.AddMinutes(-5))
                .ToList();
            TopUsers = AllUsers.OrderByDescending(u => u.CorrectAnswers).Take(5).ToList();
            AverageSuccessRate = AllUsers
                .Where(u => u.TotalAnswered > 0)
                .Select(u => (double)u.CorrectAnswers / u.TotalAnswered)
                .DefaultIfEmpty(0)
                .Average() * 100;
        }
    }
}
