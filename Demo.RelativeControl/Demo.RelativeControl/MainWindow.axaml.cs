using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RelativeControl.Avalonia;

namespace Demo.RelativeControl;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        RelativeLength a = new(20, Units.ViewPortHeight, Second);
        RelativeLength b = new(20, Units.LogicalParentHeight, Second);
        RelativeLength c = new(10, Units.ViewPortWidth,Second);
        // RelativeBinding d = new RelativeBind();
        Relative.SetHeight(Second,a+b-c);//TODO
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e) { 
        Relative.SetWidth(Container,(Container.GetValue(Relative.WidthProperty) as RelativeLengthBase)! + RelativeLength.Parse("5vw",Container));
        Relative.SetHeight(Container,(Container.GetValue(Relative.HeightProperty) as RelativeLengthBase)! + RelativeLength.Parse("5vh",Container));
    }
    private void Button_OnClick2(object? sender, RoutedEventArgs e) { 
        Relative.SetWidth(Container,(Container.GetValue(Relative.WidthProperty) as RelativeLengthBase)! - RelativeLength.Parse("5vw",Container));
        Relative.SetHeight(Container,(Container.GetValue(Relative.HeightProperty) as RelativeLengthBase)! - RelativeLength.Parse("5vh",Container));
    }
}