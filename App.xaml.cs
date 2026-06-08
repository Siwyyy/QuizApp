namespace QuizApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(new AppShell());
		
		// Ustawienie minimalnych wymiarów okna (głównie dla Windows/Mac)
		// Zapobiega ściśnięciu okna poniżej 650px, chroniąc nasz 600-pikselowy Grid przed ucięciem
		window.MinimumWidth = 650;
		window.MinimumHeight = 500;
		
		return window;
	}
}