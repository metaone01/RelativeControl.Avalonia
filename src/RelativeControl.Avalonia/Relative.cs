using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace RelativeControl.Avalonia;

public class Relative : AvaloniaObject {
    public static readonly AttachedProperty<IRelative<double>> WidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<double>>("Width", RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<double>> HeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<double>>("Height", RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<double>> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<double>>("MinWidth", RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<double>> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<double>>("MinHeight", RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<double>> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<double>>("MaxWidth", RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<double>> MaxHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<double>>("MaxHeight", RelativeLength.Empty);

    public static readonly AttachedProperty<IRelative<Thickness>> BorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, IRelative<Thickness>>(
            "BorderThickness",
            RelativeThickness.Empty);

    public static readonly AttachedProperty<IRelative<CornerRadius>> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, IRelative<CornerRadius>>(
            "CornerRadius",
            RelativeCornerRadius.Empty);

    public static readonly AttachedProperty<IRelative<Thickness>> MarginProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<Thickness>>("Margin", RelativeThickness.Empty);

    public static readonly AttachedProperty<IRelative<Thickness>> PaddingProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<Thickness>>("Padding", RelativeThickness.Empty);


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
        CornerRadiusProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) =>
                                                                     RelativeCornerRadiusHandler(
                                                                         layoutable,
                                                                         args,
                                                                         Border.CornerRadiusProperty));
    }

    private static void RelativeLengthHandler<T>(
        T element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property,
        double? defaultValue = null) where T : AvaloniaObject {
        if (args.NewValue is RelativeLengthBase nl) {
            element.SetValue(property, nl.Absolute());
            var reference = new WeakReference<T>(element);

            void Update(IRelative<double>? sender, RelativeChangedEventArgs<double> _) {
                if (reference.TryGetTarget(out T? target))
                    target.SetValue(property, sender?.Absolute());
            }

            nl.RelativeChanged += Update;
        } else {
            element.SetValue(property, defaultValue ?? RelativeLength.Empty.Absolute());
        }
    }

    private static void RelativeThicknessHandler<T>(
        T element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property,
        Thickness? defaultValue = null) where T : AvaloniaObject {
        if (args.NewValue is RelativeThickness nt) {
            element.SetValue(property, nt.Absolute());
            var reference = new WeakReference<T>(element);

            void Update(IRelative<Thickness>? sender, RelativeChangedEventArgs<Thickness> _) {
                if (reference.TryGetTarget(out T? target))
                    target.SetValue(property, sender?.Absolute());
            }

            nt.RelativeChanged += Update;
        } else {
            element.SetValue(property, defaultValue ?? RelativeThickness.Empty.Absolute());
        }
    }


    private static void RelativeCornerRadiusHandler<T>(
        T element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property,
        CornerRadius? defaultValue = null) where T : AvaloniaObject {
        if (args.NewValue is RelativeCornerRadius nt) {
            element.SetValue(property, nt.Absolute());
            var reference = new WeakReference<T>(element);

            void Update(IRelative<CornerRadius>? sender, RelativeChangedEventArgs<CornerRadius> _) {
                if (reference.TryGetTarget(out T? target))
                    target.SetValue(property, sender?.Absolute());
            }

            nt.RelativeChanged += Update;
        } else {
            element.SetValue(property, defaultValue ?? RelativeCornerRadius.Empty.Absolute());
        }
    }

    public static void SetWidth(Visual visual, object length) {
        RelativeLengthBase value = length switch {
            string s             => RelativeLengthBase.Parse(s, visual),
            RelativeLengthBase l => l,
            _                    => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
        };
        visual.SetValue(WidthProperty, value);
    }

    public static IRelative<double> GetWidth(Visual visual) { return visual.GetValue(WidthProperty); }


    public static void SetHeight(Visual visual, object length) {
        RelativeLengthBase value = length switch {
            string s             => RelativeLengthBase.Parse(s, visual),
            RelativeLengthBase l => l,
            _                    => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
        };
        visual.SetValue(HeightProperty, value);
    }

    public static IRelative<double> GetHeight(Visual visual) { return visual.GetValue(HeightProperty); }

    public static void SetMinWidth(Visual visual, object length) {
        visual.SetValue(
            MinWidthProperty,
            length switch {
                string s             => RelativeLengthBase.Parse(s, visual),
                IRelative<double> rd => rd,
                _                    => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<double> GetMinWidth(Visual visual) { return visual.GetValue(MinWidthProperty); }

    public static void SetMinHeight(Visual visual, object length) {
        visual.SetValue(
            MinHeightProperty,
            length switch {
                string s            => RelativeLengthBase.Parse(s, visual),
                IRelative<double> l => l,
                _                   => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<double> GetMinHeight(Visual visual) { return visual.GetValue(MinHeightProperty); }

    public static void SetMaxWidth(Visual visual, object length) {
        visual.SetValue(
            MaxWidthProperty,
            length switch {
                string s            => RelativeLengthBase.Parse(s, visual),
                IRelative<double> l => l,
                _                   => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<double> GetMaxWidth(Visual visual) { return visual.GetValue(MaxWidthProperty); }

    public static void SetMaxHeight(Visual visual, object length) {
        visual.SetValue(
            MaxHeightProperty,
            length switch {
                string s            => RelativeLengthBase.Parse(s, visual),
                IRelative<double> l => l,
                _                   => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<double> GetMaxHeight(Visual visual) { return visual.GetValue(MaxHeightProperty); }

    public static void SetBorderThickness(Visual visual, object length) {
        visual.SetValue(
            BorderThicknessProperty,
            length switch {
                string s               => RelativeThickness.Parse(s, visual),
                IRelative<Thickness> l => l,
                _                      => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<Thickness> GetBorderThickness(Visual visual) {
        return visual.GetValue(BorderThicknessProperty);
    }

    public static void SetCornerRadius(Visual visual, object length) {
        visual.SetValue(
            CornerRadiusProperty,
            length switch {
                string s                  => RelativeCornerRadius.Parse(s, visual),
                IRelative<CornerRadius> l => l,
                _                         => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static void SetCornerRadius(Visual visual, RelativeLengthBase length) {
        visual.SetValue(CornerRadiusProperty, length);
    }

    public static IRelative<CornerRadius> GetCornerRadius(Visual visual) {
        return visual.GetValue(CornerRadiusProperty);
    }


    public static void SetMargin(Visual visual, object length) {
        visual.SetValue(
            MarginProperty,
            length switch {
                string s               => RelativeThickness.Parse(s, visual),
                IRelative<Thickness> l => l,
                _                      => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<Thickness> GetMargin(Visual visual) { return visual.GetValue(MarginProperty); }

    public static void SetPadding(Visual visual, object length) {
        visual.SetValue(
            PaddingProperty,
            length switch {
                string s               => RelativeThickness.Parse(s, visual),
                IRelative<Thickness> l => l,
                _                      => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<Thickness> GetPadding(Visual visual) { return visual.GetValue(PaddingProperty); }


    public static void SetOneTimeWidth(Visual visual, string s) {
        visual.AttachedToVisualTree += Update;
        if (visual.IsAttachedToVisualTree())
            Update();
        return;

        void Update(object? _ = null, VisualTreeAttachmentEventArgs? __ = null) {
            visual.SetValue(Layoutable.WidthProperty, CalcRelative.Calc(s, visual));
        }
    }

    public static void SetOneTimeHeight(Visual visual, string s) {
        visual.AttachedToVisualTree += Update;
        if (visual.IsAttachedToVisualTree())
            Update();
        return;

        void Update(object? _ = null, VisualTreeAttachmentEventArgs? __ = null) {
            visual.SetValue(Layoutable.HeightProperty, CalcRelative.Calc(s, visual));
        }
    }
}

public class RelativeBinding(BindingBase sourceValue, string value) : MarkupExtension {
    public readonly RelativeScale Scale = RelativeScale.Parse(value);
    public readonly BindingBase SourceValue = sourceValue;

    public override object ProvideValue(IServiceProvider serviceProvider) {
        SourceValue.Converter = new RelativeConverter();
        SourceValue.ConverterParameter = Scale;
        return SourceValue;
    }
}

public class RelativeBindOneTime(BindingBase sourceValue, string value) : MarkupExtension {
    public readonly RelativeScale Scale = RelativeScale.Parse(value);
    public readonly BindingBase SourceValue = sourceValue;

    public override object ProvideValue(IServiceProvider serviceProvider) {
        SourceValue.Mode = BindingMode.OneTime;
        SourceValue.Converter = new RelativeConverter();
        SourceValue.ConverterParameter = Scale;
        return SourceValue;
    }
}