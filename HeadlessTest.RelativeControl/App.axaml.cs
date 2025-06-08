using Avalonia;
using Avalonia.Markup.Xaml;

namespace HeadlessTest.RelativeControl;

public class App : Application {
    public override void Initialize() { AvaloniaXamlLoader.Load(this); }
}