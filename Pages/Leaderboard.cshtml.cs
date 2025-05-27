using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HelloWorldWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HelloWorldWeb.Pages
{
    public class LeaderboardModel : PageModel
    {
        private readonly AuthService _authService;

        public LeaderboardModel(AuthService authService)
        {
            _authService = authService;
        }

        public string GameMode { get; set; } = "noodles";

        public async Task<IActionResult> OnGetAsync(string gameMode = "noodles")
        {
            GameMode = gameMode;
            var users = await _authService.GetAllUsers();
            var activeUsers = users.Where(u => !u.IsBanned && !u.IsCheater).ToList();

            if (GameMode == "noodles")
            {
                ViewData["TopUsers"] = activeUsers
                    .Where(u => u.TotalAnswered > 0)
                    .OrderByDescending(u => (double)u.CorrectAnswers / u.TotalAnswered)
                    .ThenByDescending(u => u.CorrectAnswers)
                    .Take(10)
                    .Select(u => new
                    {
                        u.Username,
                        Correct = u.CorrectAnswers,
                        Total = u.TotalAnswered,
                        SuccessRate = (int)((double)u.CorrectAnswers / u.TotalAnswered * 100)
                    });
            }
            else
            {
                ViewData["TopUsers"] = activeUsers
                    .Where(u => u.SoupTotalAnswered > 0)
                    .OrderByDescending(u => (double)u.SoupCorrectAnswers / u.SoupTotalAnswered)
                    .ThenByDescending(u => u.SoupCorrectAnswers)
                    .Take(10)
                    .Select(u => new
                    {
                        u.Username,
                        Correct = u.SoupCorrectAnswers,
                        Total = u.SoupTotalAnswered,
                        SuccessRate = (int)((double)u.SoupCorrectAnswers / u.SoupTotalAnswered * 100)
                    });
            }

            return Page();
        }
    }
}
