using Avalonia;
using Avalonia.Headless;
using HeadlessTest.RelativeControl;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

// ReSharper disable once CheckNamespace
public class TestAppBuilder {
    public static AppBuilder BuildAvaloniaApp() {
        return AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}