using Avalonia.Controls;
using Avalonia.Interactivity;
using RelativeControl.Avalonia;

namespace Demo.RelativeControl;

public partial class MainWindow : Window {
    public MainWindow() { InitializeComponent(); }

    private void Button_OnClick(object? sender, RoutedEventArgs e) {
        Relative.SetWidth(
            Container,
            (Container.GetValue(Relative.WidthProperty) as RelativeLengthBase)! +
            RelativeLength.Parse("5vw", Container));
        Relative.SetHeight(
            Container,
            (Container.GetValue(Relative.HeightProperty) as RelativeLengthBase)! +
            RelativeLength.Parse("5vh", Container));
    }

    private void Button_OnClick2(object? sender, RoutedEventArgs e) {
        Relative.SetWidth(
            Container,
            (Container.GetValue(Relative.WidthProperty) as RelativeLengthBase)! -
            RelativeLength.Parse("5vw", Container));
        Relative.SetHeight(
            Container,
            (Container.GetValue(Relative.HeightProperty) as RelativeLengthBase)! -
            RelativeLength.Parse("5vh", Container));
    }
}