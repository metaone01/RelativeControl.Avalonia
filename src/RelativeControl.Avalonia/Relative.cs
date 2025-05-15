using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace RelativeControl.Avalonia;

public class Relative : AvaloniaObject {
    public static readonly AttachedProperty<RelativeLength> WidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength>("Width");

    public static readonly AttachedProperty<RelativeLength> HeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength>("Height", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLength> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength>("MinWidth", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLength> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength>("MinHeight", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLength> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength>("MaxWidth", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLength> MaxHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength>("MaxHeight", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeThickness> BorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, RelativeThickness>(
            "BorderThickness",
            RelativeThickness.Empty);

    public static readonly AttachedProperty<RelativeCornerRadius> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, RelativeCornerRadius>(
            "CornerRadius",
            RelativeCornerRadius.Empty);

    static Relative() {
        WidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is RelativeLength nl) {
                layoutable.Width           =  nl.Absolute();
                nl.OnRelativeLengthChanged += (_, newActualPixel) => layoutable.Width = newActualPixel;
            } else {
                layoutable.Width = double.NaN;
            }
        });
        HeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is RelativeLength nl) {
                layoutable.Height          =  nl.Absolute();
                nl.OnRelativeLengthChanged += (_, newActualPixel) => layoutable.Height = newActualPixel;
            } else {
                layoutable.Height = double.NaN;
            }
        });
        MinWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is RelativeLength nl) {
                layoutable.MinWidth        =  nl.Absolute();
                nl.OnRelativeLengthChanged += (_, newActualPixel) => layoutable.MinWidth = newActualPixel;
            } else {
                layoutable.MinWidth = double.NegativeInfinity;
            }
        });
        MinHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is RelativeLength nl) {
                layoutable.MinHeight       =  nl.Absolute();
                nl.OnRelativeLengthChanged += (_, newActualPixel) => layoutable.MinHeight = newActualPixel;
            } else {
                layoutable.MinHeight = double.NegativeInfinity;
            }
        });
        MaxWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is RelativeLength nl) {
                layoutable.MaxWidth        =  nl.Absolute();
                nl.OnRelativeLengthChanged += (_, newActualPixel) => layoutable.MaxWidth = newActualPixel;
            } else {
                layoutable.MaxWidth = double.PositiveInfinity;
            }
        });
        MaxHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is RelativeLength nl) {
                layoutable.MaxHeight       =  nl.Absolute();
                nl.OnRelativeLengthChanged += (_, newActualPixel) => layoutable.MaxHeight = newActualPixel;
            } else {
                layoutable.MaxHeight = double.PositiveInfinity;
            }
        });
        BorderThicknessProperty.Changed.AddClassHandler<TemplatedControl>((templatedControl, args) => {
            if (args.NewValue is RelativeThickness nt) {
                templatedControl.BorderThickness =  nt.Absolute();
                nt.OnRelativeThicknessChanged    += thickness => templatedControl.BorderThickness = thickness;
            } else {
                templatedControl.BorderThickness = default;
            }
        });
        CornerRadiusProperty.Changed.AddClassHandler<TemplatedControl>((templatedControl, args) => {
            if (args.NewValue is RelativeCornerRadius ncr) {
                templatedControl.CornerRadius     =  ncr.Absolute();
                ncr.OnRelativeCornerRadiusChanged += cornerRadius => templatedControl.CornerRadius = cornerRadius;
            } else {
                templatedControl.CornerRadius = default;
            }
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