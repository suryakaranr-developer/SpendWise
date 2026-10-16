namespace SpendWise.App;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
    }

    // The shell is resolved here, after App.xaml's resources have loaded. Taking it as a constructor
    // parameter would build it first, and its {StaticResource ...} lookups would fail at startup.
    protected override Window CreateWindow(IActivationState? activationState) =>
        new(_services.GetRequiredService<AppShell>());
}
