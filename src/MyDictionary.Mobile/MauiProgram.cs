using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDictionary.Core.Interfaces;
using MyDictionary.Core.Services;
using MyDictionary.Infrastructure.Data;
using MyDictionary.Infrastructure.Repositories;
using MyDictionary.Infrastructure.Services;
using MyDictionary.Mobile.ViewModels;
using MyDictionary.Mobile.Views;
using MyDictionary.Mobile.Services;

namespace MyDictionary.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Database Configuration - Create and migrate database
		var dbPath = Path.Combine(FileSystem.AppDataDirectory, "myDictionary.db");
		
		// Register DbContext as Scoped (not Singleton) for proper EF Core usage
		builder.Services.AddDbContext<ApplicationDbContext>(options =>
		{
			options.UseSqlite($"Data Source={dbPath}");
		});

		// Initialize database on first run
		builder.Services.AddSingleton<IDatabaseInitializer, DatabaseInitializer>();

		// Repository Pattern - Scoped lifetime
		builder.Services.AddScoped<IWordRepository, WordRepository>();
		builder.Services.AddScoped<ITestResultRepository, TestResultRepository>();
		builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

		// Core Services
		builder.Services.AddScoped<SRSService>();

		// External Services
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddScoped<ITranslationService, TranslationService>();
		builder.Services.AddScoped<IAIService, AIService>();
		builder.Services.AddScoped<ITextToSpeechService, MyDictionary.Mobile.Services.TextToSpeechService>();

		// ViewModels
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<WordsViewModel>();
		builder.Services.AddTransient<AddWordViewModel>();
		builder.Services.AddTransient<FlashcardsViewModel>();
		builder.Services.AddTransient<TestsViewModel>();
		builder.Services.AddTransient<SettingsViewModel>();

		// Views
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<WordsPage>();
		builder.Services.AddTransient<AddWordPage>();
		builder.Services.AddTransient<FlashcardsPage>();
		builder.Services.AddTransient<TestsPage>();
		builder.Services.AddTransient<SettingsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
