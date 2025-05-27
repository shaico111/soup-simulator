using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HelloWorldWeb.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace HelloWorldWeb.Pages
{
    public class StatsModel : PageModel
    {
        private readonly AuthService _authService;

        public StatsModel(AuthService authService)
        {
            _authService = authService;
        }

        public string Username { get; set; } = "";
        public int CorrectAnswers { get; set; }
        public int TotalAnswered { get; set; }
        public int SuccessRate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");

            var user = await _authService.GetUser(username);
            if (user == null)
                return RedirectToPage("/Login");

            var onlineCount = (await _authService.GetAllUsers())
                .Where(u => u.LastSeen != null && u.LastSeen > System.DateTime.UtcNow.AddMinutes(-5))
                .Count();

            var noodlesSuccessRate = user.TotalAnswered > 0 ? (user.CorrectAnswers * 100.0 / user.TotalAnswered) : 0;
            var soupSuccessRate = user.SoupTotalAnswered > 0 ? (user.SoupCorrectAnswers * 100.0 / user.SoupTotalAnswered) : 0;

            Username = user.Username;
            CorrectAnswers = user.CorrectAnswers;
            TotalAnswered = user.TotalAnswered;
            SuccessRate = (TotalAnswered > 0)
                ? (int)((double)CorrectAnswers / TotalAnswered * 100)
                : 0;

            return new JsonResult(new
            {
                noodles = new
                {
                    correct = user.CorrectAnswers,
                    total = user.TotalAnswered,
                    successRate = (int)noodlesSuccessRate
                },
                soup = new
                {
                    correct = user.SoupCorrectAnswers,
                    total = user.SoupTotalAnswered,
                    successRate = (int)soupSuccessRate
                },
                online = onlineCount
            });
        }
    }
}
