using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless.XUnit;
using Avalonia.VisualTree;
using RelativeControl.Avalonia;

namespace HeadlessTest.RelativeControl;

public class RelativeUnitTest {
    [AvaloniaFact]
    public void Test_Relative_Unit_TemplatedParentWidth() {
        Container container = new() { Width = 1440, Height = 900 };
        ManualResetEvent wait = new(false);
        container.TemplateApplied += (_, _) => wait.Set();
        Window window = new() { Content = container };
        window.Show();
        wait.WaitOne();
        Control child = container.GetTemplateChildren().First();
        Relative.SetWidth(child, "20tpw");
        Assert.Equal(0.2 * container.Width, child.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_Unit_TemplatedParentHeight() {
        Container container = new() { Width = 1440, Height = 900 };
        ManualResetEvent wait = new(false);
        container.TemplateApplied += (_, _) => wait.Set();
        Window window = new() { Content = container };
        window.Show();
        wait.WaitOne();
        Control child = container.GetTemplateChildren().First();
        Relative.SetHeight(child, "20tph");
        Assert.Equal(0.2 * container.Height, child.Height);
    }

    [AvaloniaFact]
    public void Test_Relative_Unit_LogicalParentWidth() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        Relative.SetWidth(border, "20pw");
        Assert.Equal(0.2 * window.Width, border.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_Unit_LogicalParentHeight() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        Relative.SetHeight(border, "20ph");
        Assert.Equal(0.2 * window.Height, border.Height);
    }

    [AvaloniaFact]
    public void Test_Relative_Unit_VisualParentWidth() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        Relative.SetWidth(border, "20vpw");
        Assert.Equal(0.2 * border.GetVisualParent()!.Bounds.Width, border.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_Unit_VisualParentHeight() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        Relative.SetHeight(border, "20vph");
        Assert.Equal(0.2 * border.GetVisualParent()!.Bounds.Height, border.Height);
    }

    [AvaloniaFact]
    public void Test_Relative_Unit_SelfWidth() {
        Border border = new() { Height = 1440 };
        Window window = new() { Content = border };
        window.Show();
        Relative.SetWidth(border, "20sh");
        Assert.Equal(0.2 * border.Height, border.Width);
    }
    [AvaloniaFact]
    public void Test_Relative_Unit_SelfHeight() {
        Border border = new() { Width = 1440 };
        Window window = new() { Content = border };
        window.Show();
        Relative.SetHeight(border, "20sw");
        Assert.Equal(0.2 * border.Width, border.Height);
    }
    [AvaloniaFact]
    public void Test_Relative_Unit_FontSize() {
        TextBlock textBlock = new() { FontSize=14 };
        Window window = new() { Content = textBlock };
        window.Show();
        Relative.SetHeight(textBlock, "2em");
        Assert.Equal(2 * textBlock.FontSize, textBlock.Height);
    }
    [AvaloniaFact]
    public void Test_Relative_Unit_ViewPortHeight() {
        Border border = new();
        Window window = new() { Height=900,Content = border };
        window.Show();
        Relative.SetHeight(border, "20vh");
        Assert.Equal(0.2* window.Height, border.Height);
    }
}