using Avalonia;
using Avalonia.Headless;
using HeadlessTest.RelativeControl;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

// ReSharper disable once CheckNamespace
#pragma warning disable CA1050
public class TestAppBuilder {
    public static AppBuilder BuildAvaloniaApp() {
        return AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
#pragma warning restore CA1050
}