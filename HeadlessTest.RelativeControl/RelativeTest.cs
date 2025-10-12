using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using RelativeControl.Avalonia;

namespace HeadlessTest.RelativeControl;

public class RelativeTest {
    [AvaloniaFact]
    public void Test_Relative_SetBeforeShow_When_Source_Is_Window() {
        Button button = new();
        Window window = new() { Width = 1440, Content = button };
        Relative.SetWidth(button, "50vw");
        window.Show();
        Assert.Equivalent(
            new RelativeLength(50, Units.ViewPortWidth, button),
            button.GetValue(Relative.WidthProperty),
            true);
        Assert.StrictEqual(0.5 * window.Width, button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_SetBeforeShow_When_Source_Is_Control() {
        Button button = new();
        Border border = new() { Width = 1440, Child = button };
        Window window = new() { Width = 1920, Content = border };
        Relative.SetWidth(button, "50pw");
        window.Show();
        Assert.Equivalent(
            new RelativeLength(50, Units.LogicalParentWidth, button),
            button.GetValue(Relative.WidthProperty),
            true);
        Assert.StrictEqual(0.5 * border.Width, button.Width);
    }


    [AvaloniaFact]
    public void Test_Relative_SetWidth() {
        Button button = new();
        Window window = new() { Width = 1440, Content = button };
        window.Show();
        Relative.SetWidth(button, "50pw");
        Assert.Equivalent(
            new RelativeLength(50, Units.LogicalParentWidth, button),
            button.GetValue(Relative.WidthProperty),
            true);
        Assert.StrictEqual(0.5 * window.Width, button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_SetHeight() {
        Button button = new();
        Window window = new() { Height = 900, Content = button };
        window.Show();
        Relative.SetHeight(button, "50ph");
        Assert.Equivalent(
            new RelativeLength(50, Units.LogicalParentHeight, button),
            button.GetValue(Relative.HeightProperty),
            true);
        Assert.StrictEqual(0.5 * window.Height, button.Height);
    }

    [AvaloniaFact]
    public void Test_Relative_SetCornerRadius() {
        Button button = new();
        Window window = new() { Width = 1440, Height = 900, Content = button };
        window.Show();
        Relative.SetCornerRadius(button, "20pw 30pw 40ph 50ph");
        Assert.Equivalent(
            new RelativeCornerRadius(
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(50, Units.LogicalParentHeight, button)),
            button.GetValue(Relative.CornerRadiusProperty),
            true);
        Assert.StrictEqual(
            new CornerRadius(window.Width * 0.2, window.Width * 0.3, window.Height * 0.4, window.Height * 0.5),
            button.CornerRadius);
    }

    [AvaloniaFact]
    public void Test_Relative_SetBorderThickness() {
        Button button = new();
        Window window = new() { Width = 1440, Height = 900, Content = button };
        window.Show();
        Relative.SetBorderThickness(button, "20pw 30pw 40ph 50ph");
        Assert.Equivalent(
            new RelativeThickness(
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(50, Units.LogicalParentHeight, button)),
            button.GetValue(Relative.BorderThicknessProperty),
            true);
        Assert.StrictEqual(
            new Thickness(window.Width * 0.2, window.Width * 0.3, window.Height * 0.4, window.Height * 0.5),
            button.BorderThickness);
    }

    [AvaloniaFact]
    public void Test_Relative_SetMargin() {
        Button button = new();
        Window window = new() { Width = 1440, Height = 900, Content = button };
        window.Show();
        Relative.SetMargin(button, "20pw 30pw 40ph 50ph");
        Assert.Equivalent(
            new RelativeThickness(
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(50, Units.LogicalParentHeight, button)),
            button.GetValue(Relative.MarginProperty),
            true);
        Assert.StrictEqual(
            new Thickness(window.Width * 0.2, window.Width * 0.3, window.Height * 0.4, window.Height * 0.5),
            button.Margin);
    }

    [AvaloniaFact]
    public void Test_Relative_SetPadding() {
        Button button = new();
        Window window = new() { Width = 1440, Height = 900, Content = button };
        window.Show();
        Relative.SetPadding(button, "20pw 30pw 40ph 50ph");
        Assert.Equivalent(
            new RelativeThickness(
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(50, Units.LogicalParentHeight, button)),
            button.GetValue(Relative.PaddingProperty),
            true);
        Assert.StrictEqual(
            new Thickness(window.Width * 0.2, window.Width * 0.3, window.Height * 0.4, window.Height * 0.5),
            button.Padding);
    }

    [AvaloniaFact]
    public void Test_Relative_SetMinWidth() {
        Button button = new();
        Window window = new() { Width = 1440, Content = button };
        window.Show();
        Relative.SetMinWidth(button, "20pw");
        Assert.Equivalent(
            new RelativeLength(20, Units.LogicalParentWidth, button),
            button.GetValue(Relative.MinWidthProperty),
            true);
        Assert.StrictEqual(0.2 * window.Width, button.MinWidth);
    }

    [AvaloniaFact]
    public void Test_Relative_SetMaxWidth() {
        Button button = new();
        Window window = new() { Width = 1440, Content = button };
        window.Show();
        Relative.SetMaxWidth(button, "20pw");
        Assert.Equivalent(
            new RelativeLength(20, Units.LogicalParentWidth, button),
            button.GetValue(Relative.MaxWidthProperty),
            true);
        Assert.StrictEqual(0.2 * window.Width, button.MaxWidth);
    }

    [AvaloniaFact]
    public void Test_Relative_SetMinHeight() {
        Button button = new();
        Window window = new() { Height = 900, Content = button };
        window.Show();
        Relative.SetMinHeight(button, "20ph");
        Assert.Equivalent(
            new RelativeLength(20, Units.LogicalParentHeight, button),
            button.GetValue(Relative.MinHeightProperty),
            true);
        Assert.StrictEqual(0.2 * window.Height, button.MinHeight);
    }

    [AvaloniaFact]
    public void Test_Relative_SetMaxHeight() {
        Button button = new();
        Window window = new() { Height = 900, Content = button };
        window.Show();
        Relative.SetMaxHeight(button, "20ph");
        Assert.Equivalent(
            new RelativeLength(20, Units.LogicalParentHeight, button),
            button.GetValue(Relative.MaxHeightProperty),
            true);
        Assert.StrictEqual(0.2 * window.Height, button.MaxHeight);
    }

    [AvaloniaFact]
    public void Test_Relative_Binding() {
        //TODO
    }
}

public class RelativeSingleValueChangedTest {
    [AvaloniaFact]
    public void Test_Relative_SingleValue_Double_Changed() {
        Button button = new();
        Border border = new() { Width = 1440, Child = button };
        Window window = new() { Width = 2560, Content = border };
        window.Show();
        Relative.SetWidth(button, "50pw");
        Assert.Equivalent(
            new RelativeLength(50, Units.LogicalParentWidth, button),
            button.GetValue(Relative.WidthProperty),
            true);
        Assert.StrictEqual(0.5 * border.Width, button.Width);

        border.Width = 800;
        Assert.StrictEqual(0.5 * border.Width, button.Width);

        border.Width = 1920;
        Assert.StrictEqual(0.5 * border.Width, button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_SingleValue_CornerRadius_Changed() {
        Button button = new();
        Border border = new() { Width = 1440, Height = 900, Child = button };
        Window window = new() { Width = 2560, Height = 1600, Content = border };
        window.Show();
        Relative.SetCornerRadius(button, "20pw 30pw 40ph 50ph");
        Assert.Equivalent(
            new RelativeCornerRadius(
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(50, Units.LogicalParentHeight, button)),
            button.GetValue(Relative.CornerRadiusProperty),
            true);
        Assert.StrictEqual(
            new CornerRadius(border.Width * 0.2, border.Width * 0.3, border.Height * 0.4, border.Height * 0.5),
            button.CornerRadius);

        border.Width = 800;
        border.Height = 600;
        Assert.StrictEqual(
            new CornerRadius(border.Width * 0.2, border.Width * 0.3, border.Height * 0.4, border.Height * 0.5),
            button.CornerRadius);

        border.Width = 1920;
        border.Height = 1080;
        Assert.StrictEqual(
            new CornerRadius(border.Width * 0.2, border.Width * 0.3, border.Height * 0.4, border.Height * 0.5),
            button.CornerRadius);
    }

    [AvaloniaFact]
    public void Test_Relative_SingleValue_Thickness_Changed() {
        Button button = new();
        Border border = new() { Width = 1440, Height = 900, Child = button };
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        Relative.SetBorderThickness(button, "20pw 30pw 40ph 50ph");
        Assert.Equivalent(
            new RelativeThickness(
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(50, Units.LogicalParentHeight, button)),
            button.GetValue(Relative.BorderThicknessProperty),
            true);
        Assert.StrictEqual(
            new Thickness(border.Width * 0.2, border.Width * 0.3, border.Height * 0.4, border.Height * 0.5),
            button.BorderThickness);

        border.Width = 800;
        border.Height = 600;
        Assert.StrictEqual(
            new Thickness(border.Width * 0.2, border.Width * 0.3, border.Height * 0.4, border.Height * 0.5),
            button.BorderThickness);

        border.Width = 1920;
        border.Height = 1080;
        Assert.StrictEqual(
            new Thickness(border.Width * 0.2, border.Width * 0.3, border.Height * 0.4, border.Height * 0.5),
            button.BorderThickness);
    }
}

public class RelativeMultiValueTest {
    [AvaloniaFact]
    public void Test_Relative_MultiValue_SetDouble() {
        Button button = new();
        Window window = new() { Width = 1440, Height = 900, Content = button };
        window.Show();
        Relative.SetWidth(button, "50pw + 20ph");
        Assert.Equivalent(
            new RelativeLength(50, Units.LogicalParentWidth, button) +
            new RelativeLength(20, Units.LogicalParentHeight, button),
            button.GetValue(Relative.WidthProperty),
            true);
        Assert.StrictEqual(0.5 * window.Width + 0.2 * window.Height, button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_MultiValue_SetCornerRadius() {
        Button button = new();
        Window window = new() { Width = 1440, Height = 900, Content = button };
        window.Show();
        Relative.SetCornerRadius(button, "20pw+40ph 30pw+30ph 40ph+20pw 50ph+10pw");
        Assert.Equivalent(
            new RelativeCornerRadius(
                new RelativeLength(20, Units.LogicalParentWidth, button) +
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(30, Units.LogicalParentHeight, button) +
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button) +
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(50, Units.LogicalParentHeight, button) +
                new RelativeLength(10, Units.LogicalParentWidth, button)),
            button.GetValue(Relative.CornerRadiusProperty),
            true);
        Assert.StrictEqual(
            new CornerRadius(
                window.Width * 0.2 + window.Height * 0.4,
                window.Width * 0.3 + window.Height * 0.3,
                window.Height * 0.4 + window.Width * 0.2,
                window.Height * 0.5 + window.Width * 0.1),
            button.CornerRadius);
    }

    [AvaloniaFact]
    public void Test_Relative_MultiValue_SetThickness() {
        Button button = new();
        Window window = new() { Width = 1440, Height = 900, Content = button };
        window.Show();
        Relative.SetBorderThickness(button, "20pw+40ph 30pw+30ph 40ph+20pw 50ph+10pw");
        Assert.Equivalent(
            new RelativeThickness(
                new RelativeLength(20, Units.LogicalParentWidth, button) +
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(30, Units.LogicalParentHeight, button) +
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button) +
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(50, Units.LogicalParentHeight, button) +
                new RelativeLength(10, Units.LogicalParentWidth, button)),
            button.GetValue(Relative.BorderThicknessProperty),
            true);
        Assert.StrictEqual(
            new Thickness(
                window.Width * 0.2 + window.Height * 0.4,
                window.Width * 0.3 + window.Height * 0.3,
                window.Height * 0.4 + window.Width * 0.2,
                window.Height * 0.5 + window.Width * 0.1),
            button.BorderThickness);
    }
}

public class RelativeMultiValueChangedTest {
    [AvaloniaFact]
    public void Test_Relative_MultiValue_Double_Changed() {
        Button button = new();
        Border border = new() { Width = 1440, Height = 900, Child = button };
        Window window = new() { Width = 2560, Height = 1600, Content = border };
        window.Show();
        Relative.SetWidth(button, "50pw + 20ph");
        Assert.Equivalent(
            new RelativeLength(50, Units.LogicalParentWidth, button) +
            new RelativeLength(20, Units.LogicalParentHeight, button),
            button.GetValue(Relative.WidthProperty),
            true);
        Assert.StrictEqual(0.5 * border.Width + 0.2 * border.Height, button.Width);

        border.Width = 800;
        border.Height = 600;
        Assert.StrictEqual(0.5 * border.Width + 0.2 * border.Height, button.Width);

        border.Width = 1920;
        border.Height = 1080;
        Assert.StrictEqual(0.5 * border.Width + 0.2 * border.Height, button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_MultiValue_CornerRadius_Changed() {
        Button button = new();
        Border border = new() { Width = 1440, Height = 900, Child = button };
        Window window = new() { Width = 2560, Height = 1600, Content = border };
        window.Show();
        Relative.SetCornerRadius(button, "20pw+40ph 30pw+30ph 40ph+20pw 50ph+10pw");
        Assert.Equivalent(
            new RelativeCornerRadius(
                new RelativeLength(20, Units.LogicalParentWidth, button) +
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(30, Units.LogicalParentHeight, button) +
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button) +
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(50, Units.LogicalParentHeight, button) +
                new RelativeLength(10, Units.LogicalParentWidth, button)),
            button.GetValue(Relative.CornerRadiusProperty),
            true);
        Assert.StrictEqual(
            new CornerRadius(
                border.Width * 0.2 + border.Height * 0.4,
                border.Width * 0.3 + border.Height * 0.3,
                border.Height * 0.4 + border.Width * 0.2,
                border.Height * 0.5 + border.Width * 0.1),
            button.CornerRadius);

        border.Width = 800;
        border.Height = 600;
        Assert.StrictEqual(
            new CornerRadius(
                border.Width * 0.2 + border.Height * 0.4,
                border.Width * 0.3 + border.Height * 0.3,
                border.Height * 0.4 + border.Width * 0.2,
                border.Height * 0.5 + border.Width * 0.1),
            button.CornerRadius);

        border.Width = 1920;
        border.Height = 1080;
        Assert.StrictEqual(
            new CornerRadius(
                border.Width * 0.2 + border.Height * 0.4,
                border.Width * 0.3 + border.Height * 0.3,
                border.Height * 0.4 + border.Width * 0.2,
                border.Height * 0.5 + border.Width * 0.1),
            button.CornerRadius);
    }

    [AvaloniaFact]
    public void Test_Relative_MultiValue_Thickness_Changed() {
        Button button = new();
        Border border = new() { Width = 1440, Height = 900, Child = button };
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        Relative.SetBorderThickness(button, "20pw+40ph 30pw+30ph 40ph+20pw 50ph+10pw");
        Assert.Equivalent(
            new RelativeThickness(
                new RelativeLength(20, Units.LogicalParentWidth, button) +
                new RelativeLength(40, Units.LogicalParentHeight, button),
                new RelativeLength(30, Units.LogicalParentHeight, button) +
                new RelativeLength(30, Units.LogicalParentWidth, button),
                new RelativeLength(40, Units.LogicalParentHeight, button) +
                new RelativeLength(20, Units.LogicalParentWidth, button),
                new RelativeLength(50, Units.LogicalParentHeight, button) +
                new RelativeLength(10, Units.LogicalParentWidth, button)),
            button.GetValue(Relative.BorderThicknessProperty),
            true);
        Assert.StrictEqual(
            new Thickness(
                border.Width * 0.2 + border.Height * 0.4,
                border.Width * 0.3 + border.Height * 0.3,
                border.Height * 0.4 + border.Width * 0.2,
                border.Height * 0.5 + border.Width * 0.1),
            button.BorderThickness);

        border.Width = 800;
        border.Height = 600;
        Assert.StrictEqual(
            new Thickness(
                border.Width * 0.2 + border.Height * 0.4,
                border.Width * 0.3 + border.Height * 0.3,
                border.Height * 0.4 + border.Width * 0.2,
                border.Height * 0.5 + border.Width * 0.1),
            button.BorderThickness);

        border.Width = 1920;
        border.Height = 1080;
        Assert.StrictEqual(
            new Thickness(
                border.Width * 0.2 + border.Height * 0.4,
                border.Width * 0.3 + border.Height * 0.3,
                border.Height * 0.4 + border.Width * 0.2,
                border.Height * 0.5 + border.Width * 0.1),
            button.BorderThickness);
    }

    [AvaloniaFact]
    public void Test_Relative_SetOneTimeWidth() {
        Button button = new();
        Border border = new() { Width = 1440, Child = button };
        Window window = new() { Width = 1920, Content = border };
        window.Show();
        Relative.SetOneTimeWidth(button, "50pw");
        Assert.StrictEqual(0.5 * border.Width, button.Width);
        border.Width = 800;
        Assert.StrictEqual(0.5 * 1440, button.Width);
    }

    [AvaloniaFact]
    public void Test_Relative_SetOneTimeHeight() {
        Button button = new();
        Border border = new() { Height = 900, Child = button };
        Window window = new() { Height = 1080, Content = border };
        window.Show();
        Relative.SetOneTimeHeight(button, "50ph");
        Assert.StrictEqual(0.5 * border.Height, button.Height);
        border.Height = 600;
        Assert.StrictEqual(0.5 * 900, button.Height);
    }
}