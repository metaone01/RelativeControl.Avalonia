using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using RelativeControl.Avalonia;

namespace HeadlessTest.RelativeControl;

public class AbsoluteUnitTest {
    [AvaloniaFact]
    public void Test_Absolute_Unit_Pixel() {
        Container target = new() { Width = 1440, Height = 900 };
        ManualResetEvent wait = new(false);
        target.TemplateApplied += (_, _) => wait.Set();
        Window window = new() { Content = target };
        window.Show();
        wait.WaitOne();
        Relative.SetWidth(target, "20px");
        Assert.StrictEqual(20, target.Width);
    }

    [AvaloniaFact]
    public void Test_Absolute_Unit_Pixel_Change_VisualAnchor() {
        Border target = new() { Width = 800, Height = 600 };
        Border anchor = new() { Width = 1440, Height = 900, Child = target };
        ManualResetEvent wait = new(false);
        Window window = new() { Content = anchor };
        window.Show();
        Relative.SetWidth(target, "20px");
        Assert.StrictEqual(20, target.Width);
        var length = (Relative.GetWidth(target) as RelativeLengthBase)!;
        length.SetVisualAnchor(anchor);
        Assert.StrictEqual(20, target.Width);
    }

    [AvaloniaFact]
    public void Test_Absolute_Unit_Inch() {
        Container target = new() { Width = 1440, Height = 900 };
        ManualResetEvent wait = new(false);
        target.TemplateApplied += (_, _) => wait.Set();
        Window window = new() { Content = target };
        window.Show();
        wait.WaitOne();
        Relative.SetWidth(target, "20in");
        Assert.StrictEqual(96 * 20, target.Width);
    }

    [AvaloniaFact]
    public void Test_Absolute_Unit_Inch_Change_VisualAnchor() {
        Border target = new() { Width = 800, Height = 600 };
        Border anchor = new() { Width = 1440, Height = 900, Child = target };
        ManualResetEvent wait = new(false);
        Window window = new() { Content = anchor };
        window.Show();
        Relative.SetWidth(target, "20in");
        Assert.StrictEqual(96 * 20, target.Width);
        var length = (Relative.GetWidth(target) as RelativeLengthBase)!;
        length.SetVisualAnchor(anchor);
        Assert.StrictEqual(96 * 20, target.Width);
    }

    [AvaloniaFact]
    public void Test_Absolute_Unit_Millimeter() {
        Container target = new() { Width = 1440, Height = 900 };
        ManualResetEvent wait = new(false);
        target.TemplateApplied += (_, _) => wait.Set();
        Window window = new() { Content = target };
        window.Show();
        wait.WaitOne();
        Relative.SetWidth(target, "20px");
        Assert.StrictEqual(20, target.Width);
    }

    [AvaloniaFact]
    public void Test_Absolute_Unit_Millimeter_Change_VisualAnchor() {
        Border target = new() { Width = 800, Height = 600 };
        Border anchor = new() { Width = 1440, Height = 900, Child = target };
        ManualResetEvent wait = new(false);
        Window window = new() { Content = anchor };
        window.Show();
        Relative.SetWidth(target, "20mm");
        Assert.StrictEqual(96 / 2.54 * 20 / 1000, target.Width);
        var length = (Relative.GetWidth(target) as RelativeLengthBase)!;
        length.SetVisualAnchor(anchor);
        Assert.StrictEqual(96 / 2.54 * 20 / 1000, target.Width);
    }

    [AvaloniaFact]
    public void Test_Absolute_Unit_Centimeter() {
        Container target = new() { Width = 1440, Height = 900 };
        ManualResetEvent wait = new(false);
        target.TemplateApplied += (_, _) => wait.Set();
        Window window = new() { Content = target };
        window.Show();
        wait.WaitOne();
        Relative.SetWidth(target, "20cm");
        Assert.StrictEqual(96 / 2.54 * 20, target.Width);
    }

    [AvaloniaFact]
    public void Test_Absolute_Unit_Centimeter_Change_VisualAnchor() {
        Border target = new() { Width = 800, Height = 600 };
        Border anchor = new() { Width = 1440, Height = 900, Child = target };
        ManualResetEvent wait = new(false);
        Window window = new() { Content = anchor };
        window.Show();
        Relative.SetWidth(target, "20cm");
        Assert.StrictEqual(96 / 2.54 * 20, target.Width);
        var length = (Relative.GetWidth(target) as RelativeLengthBase)!;
        length.SetVisualAnchor(anchor);
        Assert.StrictEqual(96 / 2.54 * 20, target.Width);
    }
}