using MyDictionary.Mobile.Services;

namespace MyDictionary.Mobile;

public partial class App : Application
{
	private readonly IDatabaseInitializer _databaseInitializer;

	public App(IDatabaseInitializer databaseInitializer)
	{
		InitializeComponent();

		_databaseInitializer = databaseInitializer;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Initialize database synchronously before creating window
		Task.Run(async () => await _databaseInitializer.InitializeAsync()).Wait();

		return new Window(new AppShell());
	}
}