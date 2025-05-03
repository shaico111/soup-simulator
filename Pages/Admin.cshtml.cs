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
            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostUnflagAsync(string username)
        {
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsCheater = false;
                user.IsBanned = false;
                await _authService.UpdateUser(user);
                ViewData["SuccessMessage"] = $"✅ המשתמש '{username}' שוחרר מכל ההגבלות.";
            }
            else
            {
                ViewData["SuccessMessage"] = $"❌ המשתמש '{username}' לא נמצא.";
            }

            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostBanAsync(string username)
        {
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = true;
                await _authService.UpdateUser(user);
                ViewData["SuccessMessage"] = $"🚫 המשתמש '{username}' נחסם.";
            }
            else
            {
                ViewData["SuccessMessage"] = $"❌ המשתמש '{username}' לא נמצא.";
            }

            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostUnbanAsync(string username)
        {
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = false;
                await _authService.UpdateUser(user);
                ViewData["SuccessMessage"] = $"🔓 המשתמש '{username}' שוחרר.";
            }
            else
            {
                ViewData["SuccessMessage"] = $"❌ המשתמש '{username}' לא נמצא.";
            }

            await LoadData();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string username)
        {
            var success = await _authService.DeleteUser(username);
            if (success)
            {
                ViewData["SuccessMessage"] = $"🗑️ המשתמש '{username}' נמחק.";
            }
            else
            {
                ViewData["SuccessMessage"] = $"❌ שגיאה במחיקת המשתמש '{username}'.";
            }

            await LoadData();
            return Page();
        }

        private async Task LoadData()
        {
            AllUsers = await _authService.GetAllUsers();
            Cheaters = AllUsers.Where(u => u.IsCheater).ToList();
            BannedUsers = AllUsers.Where(u => u.IsBanned).ToList();
            OnlineUsers = AllUsers.Where(u => u.LastSeen != null && u.LastSeen > DateTime.UtcNow.AddMinutes(-5)).ToList();
            TopUsers = AllUsers.OrderByDescending(u => u.CorrectAnswers).Take(5).ToList();
            AverageSuccessRate = AllUsers.Where(u => u.TotalAnswered > 0)
                .Select(u => (double)u.CorrectAnswers / u.TotalAnswered)
                .DefaultIfEmpty(0).Average() * 100;
        }
    }
}
