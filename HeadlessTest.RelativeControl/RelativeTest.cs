using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using RelativeControl.Avalonia;

namespace HeadlessTest.RelativeControl;

public class Tests {
    [AvaloniaFact]
    public void Test_Relative_SetWidth() {
        Button button = new();
        Window window = new() { Width = 1440, Content = button };
        window.Show();
        Relative.SetWidth(button, "50pw");
        Assert.Equal(0.5 * window.Width,button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_SetHeight() {
        Button button = new();
        Window window = new() { Height = 900, Content = button };
        window.Show();
        Relative.SetHeight(button, "50ph");
        Assert.Equal(0.5 * window.Width,button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_SetCornerRadius() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetCornerRadius(button, "20pw 30pw 40ph 50ph");
        Assert.Equal(new CornerRadius(1440*0.2,1440*0.3,900*0.4,900*0.5),button.CornerRadius);
    }
    [AvaloniaFact]
    public void Test_Relative_SetBorderThickness() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetBorderThickness(button, "20pw 30pw 40ph 50ph");
        Assert.Equal(new Thickness(1440*0.2,1440*0.3,900*0.4,900*0.5),button.BorderThickness);
    }
    [AvaloniaFact]
    public void Test_Relative_SetMargin() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetMargin(button, "20pw 30pw 40ph 50ph");
        Assert.Equal(new Thickness(1440*0.2,1440*0.3,900*0.4,900*0.5),button.Margin);
    }
    [AvaloniaFact]
    public void Test_Relative_SetPadding() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetPadding(button, "20pw 30pw 40ph 50ph");
        Assert.Equal(new Thickness(1440*0.2,1440*0.3,900*0.4,900*0.5),button.Padding);
    }
    [AvaloniaFact]
    public void Test_Relative_SetMinWidth() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetMinWidth(button, "20pw");
        Assert.Equal(0.2*window.Width,button.MinWidth);
    }
    [AvaloniaFact]
    public void Test_Relative_SetMaxWidth() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetMaxWidth(button, "20pw");
        Assert.Equal(0.2*window.Width,button.MaxWidth);
    }
    [AvaloniaFact]
    public void Test_Relative_SetMinHeight() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetMinHeight(button, "20ph");
        Assert.Equal(0.2*window.Height,button.MinHeight);
    }
    [AvaloniaFact]
    public void Test_Relative_SetMaxHeight() {
        Button button = new();
        Window window = new() { Width = 1440,Height = 900, Content = button };
        window.Show();
        Relative.SetMaxHeight(button, "20ph");
        Assert.Equal(0.2*window.Height,button.MaxHeight);
    }

    [AvaloniaFact]
    public void Test_Relative_Binding() {
        //TODO
    }
}