using Microsoft.AspNetCore.Mvc;
using MyDictionary.Core.Entities;
using MyDictionary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MyDictionary.Web.Controllers;

public class DailyChallengesController : Controller
{
    private readonly ApplicationDbContext _context;

    public DailyChallengesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var challenges = await _context.DailyChallenges
            .Where(c => c.Date == today)
            .ToListAsync();

        if (!challenges.Any())
        {
            // Generate daily challenges if they don't exist
            challenges = new List<DailyChallenge>
            {
                new DailyChallenge
                {
                    Date = today,
                    ChallengeName = "Review Master",
                    Description = "Review 10 words in Flashcards",
                    TargetCount = 10,
                    CurrentCount = 0,
                    XPReward = 50
                },
                new DailyChallenge
                {
                    Date = today,
                    ChallengeName = "New Learner",
                    Description = "Add 3 new words to your dictionary",
                    TargetCount = 3,
                    CurrentCount = 0,
                    XPReward = 30
                },
                new DailyChallenge
                {
                    Date = today,
                    ChallengeName = "Quiz Whiz",
                    Description = "Complete 1 practice test with >80% score",
                    TargetCount = 1,
                    CurrentCount = 0,
                    XPReward = 100
                }
            };

            _context.DailyChallenges.AddRange(challenges);
            await _context.SaveChangesAsync();
        }

        return View(challenges);
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id)
    {
        var challenge = await _context.DailyChallenges.FindAsync(id);
        if (challenge != null && !challenge.IsCompleted)
        {
            challenge.IsCompleted = true;
            challenge.CompletedAt = DateTime.Now;
            challenge.CurrentCount = challenge.TargetCount;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        return Json(new { success = false });
    }
}
