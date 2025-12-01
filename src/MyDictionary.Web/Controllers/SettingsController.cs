using Microsoft.AspNetCore.Mvc;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;

namespace MyDictionary.Web.Controllers;

public class SettingsController : Controller
{
    private readonly IUserSettingsRepository _userSettingsRepository;

    public SettingsController(IUserSettingsRepository userSettingsRepository)
    {
        _userSettingsRepository = userSettingsRepository;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _userSettingsRepository.GetSettingsAsync();
        return View(settings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UserSettings settings)
    {
        if (ModelState.IsValid)
        {
            await _userSettingsRepository.UpdateSettingsAsync(settings);
            TempData["Success"] = "Settings updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View("Index", settings);
    }
}
