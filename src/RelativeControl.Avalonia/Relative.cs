using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;

namespace RelativeControl.Avalonia;

public class Relative : AvaloniaObject {
    public static readonly AttachedProperty<RelativeLengthBase> WidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLengthBase>("Width");

    public static readonly AttachedProperty<RelativeLengthBase> HeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLengthBase>("Height", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLengthBase> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLengthBase>("MinWidth", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLengthBase> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLengthBase>("MinHeight", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLengthBase> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLengthBase>("MaxWidth", RelativeLength.Empty);

    public static readonly AttachedProperty<RelativeLengthBase> MaxHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLengthBase>("MaxHeight", RelativeLength.Empty);

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
                ncr.RelativeCornerRadiusChanged += cornerRadius =>
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
        if (args.NewValue is RelativeLengthBase nl) {
            element.SetValue(property, nl.Absolute());
            nl.RelativeLengthChanged += (_, newActualPixel) => element.SetValue(property, newActualPixel);
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
            nt.RelativeThicknessChanged += thickness => element.SetValue(property, thickness);
        } else {
            element.SetValue(property, RelativeThickness.Empty);
        }
    }

    public static void SetWidth(Visual visual, string s) {
        visual.SetValue(WidthProperty, RelativeLength.Parse(s, visual));
    }

    public static RelativeLengthBase GetWidth(Visual visual) { return visual.GetValue(WidthProperty); }

    public static void SetHeight(Visual visual, string s) {
        visual.SetValue(HeightProperty, RelativeLength.Parse(s, visual));
    }

    public static RelativeLengthBase GetHeight(Visual visual) { return visual.GetValue(HeightProperty); }

    public static void SetMinWidth(Visual visual, string s) {
        visual.SetValue(MinWidthProperty, RelativeLength.Parse(s, visual));
    }

    public static RelativeLengthBase GetMinWidth(Visual visual) { return visual.GetValue(MinWidthProperty); }

    public static void SetMinHeight(Visual visual, string s) {
        visual.SetValue(MinHeightProperty, RelativeLength.Parse(s, visual));
    }

    public static RelativeLengthBase GetMinHeight(Visual visual) { return visual.GetValue(MinHeightProperty); }

    public static void SetMaxWidth(Visual visual, string s) {
        visual.SetValue(MaxWidthProperty, RelativeLength.Parse(s, visual));
    }

    public static RelativeLengthBase GetMaxWidth(Visual visual) { return visual.GetValue(MaxWidthProperty); }

    public static void SetMaxHeight(Visual visual, string s) {
        visual.SetValue(MaxHeightProperty, RelativeLength.Parse(s, visual));
    }

    public static RelativeLengthBase GetMaxHeight(Visual visual) { return visual.GetValue(MaxHeightProperty); }

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

public class RelativeBinding(BindingBase sourceProperty,string value) : MarkupExtension {
    public readonly RelativeScaler Scaler = RelativeScaler.Parse(value);

    public override object ProvideValue(IServiceProvider serviceProvider) {
        sourceProperty.Converter = new RelativeConverter();
        sourceProperty.ConverterParameter = Scaler;
        return sourceProperty;
    }
}