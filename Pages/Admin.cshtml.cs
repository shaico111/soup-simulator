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
            await LoadData();  // Loading all data on page load
            return Page();
        }

        public async Task<IActionResult> OnPostUnflagAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ Access denied. Only admins can access this page.";
                return RedirectToPage("/Index");
            }

            Console.WriteLine($"🟢 [Unflag] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsCheater = false;  // Unflag user as cheater
                Console.WriteLine($"🔄 [Unflag] Updating user {username} - IsCheater set to false");
                await _authService.UpdateUser(user);  // Update user in Supabase
                TempData["SuccessMessage"] = $"✅ User '{username}' has been unflagged successfully.";
                Console.WriteLine($"✅ [Unflag] Updated {username} successfully.");
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ Error unflagging user '{username}'.";
                Console.WriteLine($"❌ [Unflag] User {username} not found.");
            }

            await LoadData();  // Reload data to refresh the page with updated info
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostBanAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ Access denied. Only admins can access this page.";
                return RedirectToPage("/Index");
            }

            Console.WriteLine($"🚫 [Ban] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = true;  // Ban user
                Console.WriteLine($"🔄 [Ban] Updating user {username} - IsBanned set to true");
                await _authService.UpdateUser(user);  // Update user in Supabase
                TempData["SuccessMessage"] = $"🚫 User '{username}' has been banned.";
                Console.WriteLine($"✅ [Ban] Updated {username} successfully.");
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ Error banning user '{username}'.";
                Console.WriteLine($"❌ [Ban] User {username} not found.");
            }

            await LoadData();  // Reload data to refresh the page with updated info
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnbanAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ Access denied. Only admins can access this page.";
                return RedirectToPage("/Index");
            }

            Console.WriteLine($"🔓 [Unban] Requested for: {username}");
            var user = await _authService.GetUser(username);
            if (user != null)
            {
                user.IsBanned = false;  // Unban user
                Console.WriteLine($"🔄 [Unban] Updating user {username} - IsBanned set to false");
                await _authService.UpdateUser(user);  // Update user in Supabase
                TempData["SuccessMessage"] = $"🔓 User '{username}' has been unbanned.";
                Console.WriteLine($"✅ [Unban] Updated {username} successfully.");
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ Error unbanning user '{username}'.";
                Console.WriteLine($"❌ [Unban] User {username} not found.");
            }

            await LoadData();  // Reload data to refresh the page with updated info
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string username)
        {
            if (!IsAdmin())
            {
                TempData["SuccessMessage"] = "❌ Access denied. Only admins can access this page.";
                return RedirectToPage("/Index");
            }

            Console.WriteLine($"🗑️ [Delete] Requested for: {username}");
            var success = await _authService.DeleteUser(username);  // Delete user from Supabase
            if (success)
            {
                TempData["SuccessMessage"] = $"🗑️ User '{username}' has been deleted.";
                Console.WriteLine($"✅ [Delete] User {username} deleted.");
            }
            else
            {
                TempData["SuccessMessage"] = $"❌ Error deleting user '{username}'.";
                Console.WriteLine($"❌ [Delete] Failed to delete {username}.");
            }

            await LoadData();  // Reload data to refresh the page with updated info
            return RedirectToPage();
        }

        private bool IsAdmin()
        {
            var sessionUser = HttpContext.Session.GetString("Username");
            return sessionUser == "Admin";  // Check if user is Admin
        }

        private async Task LoadData()
        {
            AllUsers = await _authService.GetAllUsers();
            Console.WriteLine($"🔄 [LoadData] Loaded {AllUsers.Count} users from Supabase.");
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

            Console.WriteLine($"🔄 [LoadData] Loaded {Cheaters.Count} cheaters, {BannedUsers.Count} banned users, and {OnlineUsers.Count} online users.");
        }
    }
}
