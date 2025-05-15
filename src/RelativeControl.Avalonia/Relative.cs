using Avalonia;
using Avalonia.Controls;
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

    public static readonly AttachedProperty<RelativeThickness> MarginProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeThickness>("Margin", RelativeThickness.Empty);

    public static readonly AttachedProperty<RelativeThickness> PaddingProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeThickness>("Padding", RelativeThickness.Empty);


    static Relative() {
        WidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                              RelativeLengthHandler(
                                                                  layoutable,
                                                                  args,
                                                                  Layoutable.WidthProperty));
        HeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                               RelativeLengthHandler(
                                                                   layoutable,
                                                                   args,
                                                                   Layoutable.HeightProperty));
        MinWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                                 RelativeLengthHandler(
                                                                     layoutable,
                                                                     args,
                                                                     Layoutable.MinWidthProperty,
                                                                     double.NegativeInfinity));
        MinHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                                  RelativeLengthHandler(
                                                                      layoutable,
                                                                      args,
                                                                      Layoutable.MinHeightProperty,
                                                                      double.NegativeInfinity));
        MaxWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                                 RelativeLengthHandler(
                                                                     layoutable,
                                                                     args,
                                                                     Layoutable.MaxWidthProperty,
                                                                     double.PositiveInfinity));
        MaxHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                                  RelativeLengthHandler(
                                                                      layoutable,
                                                                      args,
                                                                      Layoutable.MaxHeightProperty,
                                                                      double.PositiveInfinity));
        BorderThicknessProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => RelativeThicknessHandler(
                                                                        layoutable,
                                                                        args,
                                                                        Border.BorderThicknessProperty));
        MarginProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                               RelativeThicknessHandler(
                                                                   layoutable,
                                                                   args,
                                                                   Layoutable.MarginProperty));
        PaddingProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                                RelativeThicknessHandler(
                                                                    layoutable,
                                                                    args,
                                                                    Decorator.PaddingProperty));
        CornerRadiusProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is RelativeCornerRadius ncr) {
                layoutable.SetValue(Border.CornerRadiusProperty, ncr.Absolute());
                ncr.OnRelativeCornerRadiusChanged += cornerRadius =>
                    layoutable.SetValue(Border.CornerRadiusProperty, cornerRadius);
            } else {
                layoutable.SetValue(Border.CornerRadiusProperty, default);
            }
        });
    }

    private static void RelativeLengthHandler<T>(
        T element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property,
        double defaultValue = double.NaN) where T : AvaloniaObject {
        if (args.NewValue is RelativeLength nl) {
            element.SetValue(property, nl.Absolute());
            nl.OnRelativeLengthChanged += (_, newActualPixel) => element.SetValue(property, newActualPixel);
        } else {
            element.SetValue(property, defaultValue);
        }
    }

    private static void RelativeThicknessHandler<T>(
        T element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property) where T : AvaloniaObject {
        if (args.NewValue is RelativeThickness nt) {
            element.SetValue(property, nt.Absolute());
            nt.OnRelativeThicknessChanged += thickness => element.SetValue(property, thickness);
        } else {
            element.SetValue(property, RelativeThickness.Empty);
        }
    }

    public static void SetWidth(Visual visual, string s) {
        visual.SetValue(WidthProperty, new RelativeLength(s, visual));
    }

    public static RelativeLength GetWidth(Visual visual) { return visual.GetValue(WidthProperty); }

    public static void SetHeight(Visual visual, string s) {
        visual.SetValue(HeightProperty, new RelativeLength(s, visual));
    }

    public static RelativeLength GetHeight(Visual visual) { return visual.GetValue(HeightProperty); }

    public static void SetMinWidth(Visual visual, string s) {
        visual.SetValue(MinWidthProperty, new RelativeLength(s, visual));
    }

    public static RelativeLength GetMinWidth(Visual visual) { return visual.GetValue(MinWidthProperty); }

    public static void SetMinHeight(Visual visual, string s) {
        visual.SetValue(MinHeightProperty, new RelativeLength(s, visual));
    }

    public static RelativeLength GetMinHeight(Visual visual) { return visual.GetValue(MinHeightProperty); }

    public static void SetMaxWidth(Visual visual, string s) {
        visual.SetValue(MaxWidthProperty, new RelativeLength(s, visual));
    }

    public static RelativeLength GetMaxWidth(Visual visual) { return visual.GetValue(MaxWidthProperty); }

    public static void SetMaxHeight(Visual visual, string s) {
        visual.SetValue(MaxHeightProperty, new RelativeLength(s, visual));
    }

    public static RelativeLength GetMaxHeight(Visual visual) { return visual.GetValue(MaxHeightProperty); }

    public static void SetBorderThickness(Visual visual, string s) {
        visual.SetValue(BorderThicknessProperty, RelativeThickness.Parse(s, visual));
    }

    public static RelativeThickness GetBorderThickness(Visual visual) {
        return visual.GetValue(BorderThicknessProperty);
    }

    public static void SetCornerRadius(Visual visual, string s) {
        visual.SetValue(CornerRadiusProperty, RelativeCornerRadius.Parse(s, visual));
    }

    public static RelativeCornerRadius GetCornerRadius(Visual visual) { return visual.GetValue(CornerRadiusProperty); }


    public static void SetMargin(Visual visual, string s) {
        visual.SetValue(MarginProperty, RelativeThickness.Parse(s, visual));
    }

    public static RelativeThickness GetMargin(Visual visual) { return visual.GetValue(MarginProperty); }

    public static void SetPadding(Visual visual, string s) {
        visual.SetValue(PaddingProperty, RelativeThickness.Parse(s, visual));
    }

    public static RelativeThickness GetPadding(Visual visual) { return visual.GetValue(PaddingProperty); }
}