using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using RelativeControl.Avalonia;

namespace HeadlessTest.RelativeControl;

public class RelativeOperatorTest {
    [AvaloniaFact]
    public void Test_RelativeLength_Operator_Add_Absolute_Unit() {
        Border border = new();
        Window window = new() { Content = border };
        window.Show();
        RelativeLength a = new(20, Units.Pixel, border);
        RelativeLength b = new(15, Units.Centimeter, border);
        RelativeLength c = new(10, Units.Millimeter, border);
        RelativeLength d = new(5, Units.Inch, border);
        Assert.Equivalent(new RelativeLength(20 + 96 / 2.54 * 15), a + b, true);
        Assert.Equivalent(new RelativeLength(20 + 96 / 2.54 * 15 + 96 / 2.54 * 10 / 1000), a + b + c, true);
        Assert.Equivalent(
            new RelativeLength(20 + 96 / 2.54 * 15 + 96 / 2.54 * 10 / 1000 + 96 * 5),
            a + b + c + d,
            true);
        Relative.SetWidth(border, a + b + c + d);
        Assert.StrictEqual(20 + 96 / 2.54 * 15 + 96 / 2.54 * 10 / 1000 + 96 * 5, border.Width);
    }

    [AvaloniaFact]
    public void Test_RelativeLength_Operator_Add_Same_Relative_Unit() {
        Border border = new();
        Window window = new() { Width = 1440, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(10, Units.LogicalParentWidth, border);
        Relative.SetWidth(border, a + b);
        Assert.Equivalent(new RelativeLength(30, Units.LogicalParentWidth, border), Relative.GetWidth(border), true);
        Assert.StrictEqual(0.3 * window.Width, border.Width);
    }

    [AvaloniaFact]
    public void Test_RelativeLength_Operator_Subtract_Same_Relative_Unit() {
        Border border = new();
        Window window = new() { Width = 1440, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(10, Units.LogicalParentWidth, border);
        Relative.SetWidth(border, a - b);
        Assert.Equivalent(new RelativeLength(10, Units.LogicalParentWidth, border), Relative.GetWidth(border), true);
        Assert.StrictEqual(0.1 * window.Width, border.Width);
    }

    [AvaloniaFact]
    public void Test_RelativeLengthMerge() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(10, Units.LogicalParentHeight, border);
        Relative.SetWidth(border, a + b);
        Assert.Equivalent(new RelativeLengthMerge(a, b), Relative.GetWidth(border), true);
        Assert.StrictEqual(0.2 * window.Width + 0.1 * window.Height, border.Width);
    }

    [AvaloniaFact]
    public void Test_RelativeLengthSub() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(10, Units.LogicalParentHeight, border);
        Relative.SetWidth(border, a - b);
        Assert.Equivalent(new RelativeLengthMerge(a, b * -1), Relative.GetWidth(border), true);
        Assert.StrictEqual(0.2 * window.Width - 0.1 * window.Height, border.Width);
    }

    [AvaloniaFact]
    public void Test_Mix_RelativeLengthMerge_And_RelativeLengthSub() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(5, Units.LogicalParentHeight, border);
        RelativeLength c = new(10, Units.LogicalParentWidth, border);
        Relative.SetWidth(border, a + b - c);
        Assert.Equivalent(
            new RelativeLengthMerge(new RelativeLengthMerge(a, b), c * -1),
            Relative.GetWidth(border),
            true);
        Assert.StrictEqual(0.1 * window.Width + 0.05 * window.Height, border.Width);
    }

    [AvaloniaFact]
    public void Test_Mix_RelativeLengthSub_And_RelativeLengthMerge() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(10, Units.LogicalParentHeight, border);
        RelativeLength c = new(5, Units.LogicalParentWidth, border);
        Relative.SetWidth(border, a - b + c);
        Assert.Equivalent(
            new RelativeLengthMerge(new RelativeLengthMerge(a, b * -1), c),
            Relative.GetWidth(border),
            true);
        Assert.StrictEqual(0.25 * window.Width - 0.1 * window.Height, border.Width);
    }
}

public class RelativeOperatorSourceUnchangedTest {
    [AvaloniaFact]
    public void RelativeLength_Operator_Add() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(20, Units.LogicalParentHeight, border);
        RelativeLength la = a.Copy();
        RelativeLength lb = b.Copy();
        _ = a + b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(b, lb, true);
    }

    [AvaloniaFact]
    public void RelativeLength_Operator_Sub() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeLength b = new(20, Units.LogicalParentHeight, border);
        RelativeLength la = a.Copy();
        RelativeLength lb = b.Copy();
        _ = a + b;
        _ = a - b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(b, lb, true);
    }

    [AvaloniaFact]
    public void RelativeLength_Operator_Multiply() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeScale b = 0.2;
        RelativeLength la = a.Copy();
        LightSingleRelativeLength c = a * b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(a * 0.2, c, true);
    }

    [AvaloniaFact]
    public void RelativeLength_Operator_Divide() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLength a = new(20, Units.LogicalParentWidth, border);
        RelativeScale b = 0.2;
        RelativeLength la = a.Copy();
        var d = a / b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(a / 0.2, d, true);
    }

    [Fact]
    public void Relative_Operator_Accessibility() {
        RelativeLength length = RelativeLength.Empty;
        RelativeLengthMerge merge = RelativeLengthMerge.Empty;
        SingleRelativeLength single = length;
        RelativeLengthBase base1 = length;
        RelativeLengthCollection collection = merge;
        RelativeLengthBase base2 = merge;
        Assert.Equivalent(length + merge, merge + length, true);
        Assert.StrictEqual((length - merge).ActualPixels * -1, (merge - length).ActualPixels);
        Assert.Equivalent(single + merge, merge + single, true);
        Assert.StrictEqual((single - merge).ActualPixels * -1, (merge - single).ActualPixels);
        Assert.Equivalent(base1 + merge, merge + base1, true);
        Assert.StrictEqual((base1 - merge).ActualPixels * -1, (merge - base1).ActualPixels);
        Assert.Equivalent(length + collection, collection + length, true);
        Assert.StrictEqual((length - collection).ActualPixels * -1, (collection - length).ActualPixels);
        Assert.Equivalent(single + collection, collection + single, true);
        Assert.StrictEqual((single - collection).ActualPixels * -1, (collection - single).ActualPixels);
        Assert.Equivalent(base1 + collection, collection + base1, true);
        Assert.StrictEqual((base1 - collection).ActualPixels * -1, (collection - base1).ActualPixels);
        Assert.Equivalent(length + base2, base2 + length, true);
        Assert.StrictEqual((length - base2).ActualPixels * -1, (base2 - length).ActualPixels);
        Assert.Equivalent(single + base2, single + base2, true);
        Assert.StrictEqual((single - base2).ActualPixels * -1, (base2 - single).ActualPixels);
        Assert.Equivalent(base1 + base2, base2 + base1, true);
        Assert.StrictEqual((base1 - base2).ActualPixels * -1, (base2 - base1).ActualPixels);
    }

    [AvaloniaFact]
    public void RelativeLengthMerge_Operator_Add() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLengthMerge a = new(new RelativeLength(20, Units.LogicalParentWidth, border));
        RelativeLength b = new(20, Units.LogicalParentHeight, border);
        RelativeLengthMerge la = a.Copy();
        RelativeLength lb = b.Copy();
        _ = a + b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(b, lb, true);
    }

    [AvaloniaFact]
    public void RelativeLengthMerge_Operator_Sub() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLengthMerge a = new(new RelativeLength(20, Units.LogicalParentWidth, border));
        RelativeLength b = new(20, Units.LogicalParentHeight, border);
        RelativeLengthMerge la = a.Copy();
        RelativeLength lb = b.Copy();
        _ = a + b;
        _ = a - b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(b, lb, true);
    }

    [AvaloniaFact]
    public void RelativeLengthMerge_Operator_Multiply() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLengthMerge a = new(new RelativeLength(20, Units.LogicalParentWidth, border));
        RelativeScale b = 0.2;
        RelativeLengthMerge la = a.Copy();
        RelativeLengthMerge c = a * b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(a * 0.2, c, true);
    }

    [AvaloniaFact]
    public void RelativeLengthMerge_Operator_Divide() {
        Border border = new();
        Window window = new() { Width = 1440, Height = 900, Content = border };
        window.Show();
        RelativeLengthMerge a = new(new RelativeLength(20, Units.LogicalParentWidth, border));
        RelativeScale b = 0.2;
        RelativeLengthMerge la = a.Copy();
        RelativeLengthMerge d = a / b;
        Assert.Equivalent(a, la, true);
        Assert.Equivalent(a / 0.2, d, true);
    }
}