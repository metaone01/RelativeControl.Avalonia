using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace RelativeControl.Avalonia;

public class Relative : AvaloniaObject {
    public static readonly AttachedProperty<RelativeLength?> WidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(WidthProperty)[..^8]);

    public static readonly AttachedProperty<RelativeLength?> HeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(HeightProperty)[..^8]);

    public static readonly AttachedProperty<RelativeLength?> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MinWidthProperty)[..^8]);

    public static readonly AttachedProperty<RelativeLength?> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MinHeightProperty)[..^8]);

    public static readonly AttachedProperty<RelativeLength?> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MaxWidthProperty)[..^8]);

    public static readonly AttachedProperty<RelativeLength?> MaxHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MaxHeightProperty)[..^8]);

    public static readonly AttachedProperty<RelativeThickness?> BorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, RelativeThickness?>(
            nameof(BorderThicknessProperty)[..^8]);

    public static readonly AttachedProperty<RelativeLength?> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, RelativeLength?>(
            nameof(CornerRadiusProperty)[..^8]);

    static Relative() {
        WidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            RelativeLength? nl = args.NewValue as RelativeLength;
            layoutable.Width = nl?.Absolute() ?? double.NaN;
        });
        HeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            RelativeLength? nl = args.NewValue as RelativeLength;
            layoutable.Height = nl?.Absolute() ?? double.NaN;
        });
        MinWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            RelativeLength? nl = args.NewValue as RelativeLength;
            layoutable.MinWidth = nl?.Absolute() ?? double.NaN;
        });
        MinHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            RelativeLength? nl = args.NewValue as RelativeLength;
            layoutable.MinHeight = nl?.Absolute() ?? double.NaN;
        });
        MaxWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            RelativeLength? nl = args.NewValue as RelativeLength;
            layoutable.MaxWidth = nl?.Absolute() ?? double.NaN;
        });
        MaxHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            RelativeLength? nl = args.NewValue as RelativeLength;
            layoutable.MaxHeight = nl?.Absolute() ?? double.NaN;
        });
        BorderThicknessProperty.Changed.AddClassHandler<TemplatedControl>((templatedControl, args) => {
            var nt = args.NewValue as RelativeThickness?;
            templatedControl.BorderThickness = nt?.Absolute() ?? default;
        });
        CornerRadiusProperty.Changed.AddClassHandler<TemplatedControl>((templatedControl, args) => {
            var ncr = args.NewValue as RelativeCornerRadius?;
            templatedControl.CornerRadius = ncr?.Absolute() ?? default;
        });
    }

    public static void SetWidth(Visual visual, string s) {
        visual.SetValue(WidthProperty, new RelativeLength(s, visual));
    }

    public static void GetWidth(Visual visual) { visual.GetValue(WidthProperty); }

    public static void SetHeight(Visual visual, string s) {
        visual.SetValue(HeightProperty, new RelativeLength(s, visual));
    }

    public static void GetHeight(Visual visual) { visual.GetValue(HeightProperty); }

    public static void SetMinWidth(Visual visual, string s) {
        visual.SetValue(MinWidthProperty, new RelativeLength(s, visual));
    }

    public static void GetMinWidth(Visual visual) { visual.GetValue(MinWidthProperty); }

    public static void SetMinHeight(Visual visual, string s) {
        visual.SetValue(MinHeightProperty, new RelativeLength(s, visual));
    }

    public static void GetMinHeight(Visual visual) { visual.GetValue(MinHeightProperty); }

    public static void SetMaxWidth(Visual visual, string s) {
        visual.SetValue(MaxWidthProperty, new RelativeLength(s, visual));
    }

    public static void GetMaxWidth(Visual visual) { visual.GetValue(MaxWidthProperty); }

    public static void SetMaxHeight(Visual visual, string s) {
        visual.SetValue(MaxHeightProperty, new RelativeLength(s, visual));
    }

    public static void GetMaxHeight(Visual visual) { visual.GetValue(MaxHeightProperty); }

    public static void SetBorderThickness(Visual visual, string s) {
        visual.SetValue(BorderThicknessProperty, RelativeThickness.Parse(s, visual));
    }

    public static void GetBorderThickness(Visual visual) { visual.GetValue(BorderThicknessProperty); }

    public static void SetCornerRadius(Visual visual, string s) {
        visual.SetValue(CornerRadiusProperty, RelativeCornerRadius.Parse(s, visual));
    }

    public static void GetCornerRadius(Visual visual) { visual.GetValue(CornerRadiusProperty); }
}

public sealed class Rel : Relative;