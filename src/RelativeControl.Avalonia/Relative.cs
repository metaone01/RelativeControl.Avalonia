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
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<Thickness>>(
            "Margin",
            RelativeThickness.Empty);

    public static readonly AttachedProperty<IRelative<Thickness>> PaddingProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, IRelative<Thickness>>(
            "Padding",
            RelativeThickness.Empty);

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

    private static void RelativeLengthHandler(
        Visual element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property,
        double? defaultValue = null) {
        switch (args.NewValue) {
            case RelativeExpression expression:
                element.SetValue(args.Property, RelativeLengthBase.Parse(expression.Expression, element));
                break;
            case IRelative<double> nl: {
                if (ReferenceEquals(nl, RelativeLength.Empty))
                    return;
                element.SetValue(property, nl.Absolute());
                WeakReference<Visual> reference = new(element);

                nl.RelativeChanged -= Update;
                nl.RelativeChanged += Update;
                break;

                void Update(IRelative<double>? sender, RelativeChangedEventArgs<double> _) {
                    if (reference.TryGetTarget(out Visual? target))
                        target.SetValue(property, sender!.Absolute());
                }
            }
            case null:
                if (defaultValue is not null)
                    element.SetValue(property, defaultValue);
                break;
            default:
                throw new InvalidOperationException($"{args.NewValue.GetType()} is not a correct type.");
        }
    }

    private static void RelativeThicknessHandler(
        Visual element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property,
        Thickness? defaultValue = null) {
        switch (args.NewValue) {
            case RelativeExpression expression:
                element.SetValue(args.Property, RelativeThickness.Parse(expression.Expression, element));
                break;
            case IRelative<Thickness> nt: {
                if (ReferenceEquals(nt, RelativeThickness.Empty))
                    return;
                element.SetValue(property, nt.Absolute());
                WeakReference<Visual> reference = new(element);

                nt.RelativeChanged -= Update;
                nt.RelativeChanged += Update;
                break;

                void Update(IRelative<Thickness>? sender, RelativeChangedEventArgs<Thickness> _) {
                    if (reference.TryGetTarget(out Visual? target))
                        target.SetValue(property, sender!.Absolute());
                }
            }
            case null:
                if (defaultValue is not null)
                    element.SetValue(property, defaultValue);
                break;
            default:
                throw new InvalidOperationException($"{args.NewValue.GetType()} is not a correct type.");
        }
    }


    private static void RelativeCornerRadiusHandler(
        Visual element,
        AvaloniaPropertyChangedEventArgs args,
        AvaloniaProperty property,
        CornerRadius? defaultValue = null) {
        switch (args.NewValue) {
            case RelativeExpression expression:
                element.SetValue(property, RelativeCornerRadius.Parse(expression.Expression, element));
                break;
            case IRelative<CornerRadius> nc: {
                if (ReferenceEquals(nc, RelativeCornerRadius.Empty))
                    return;
                element.SetValue(property, nc.Absolute());
                WeakReference<Visual> reference = new(element);

                nc.RelativeChanged -= Update;
                nc.RelativeChanged += Update;
                break;

                void Update(IRelative<CornerRadius>? sender, RelativeChangedEventArgs<CornerRadius> _) {
                    if (reference.TryGetTarget(out Visual? target))
                        target.SetValue(property, sender!.Absolute());
                }
            }
            case null:
                if (defaultValue is not null)
                    element.SetValue(property, defaultValue);
                break;
            default:
                throw new InvalidOperationException($"{args.NewValue.GetType()} is not a correct type.");
        }
    }

    public static void SetWidth(Visual target, object length) {
        SetRelativeDoubleProperty(target, WidthProperty, length);
    }

    public static void SetWidth(AvaloniaObject target, object length, Visual visualAnchor) {
        SetRelativeDoubleProperty(target, WidthProperty, length, visualAnchor);
    }

    public static IRelative<double> GetWidth(AvaloniaObject target) { return target.GetValue(WidthProperty); }

    public static void SetHeight(Visual target, object length) {
        SetRelativeDoubleProperty(target, HeightProperty, length);
    }

    public static void SetHeight(AvaloniaObject target, object length, Visual visualAnchor) {
        SetRelativeDoubleProperty(target, HeightProperty, length, visualAnchor);
    }

    public static IRelative<double> GetHeight(AvaloniaObject target) { return target.GetValue(HeightProperty); }

    public static void SetMinWidth(Visual target, object length) {
        SetRelativeDoubleProperty(target, MinWidthProperty, length);
    }

    public static void SetMinWidth(AvaloniaObject target, object length, Visual? visualAnchor = null) {
        SetRelativeDoubleProperty(target, MinWidthProperty, length, visualAnchor);
    }

    public static IRelative<double> GetMinWidth(AvaloniaObject target) { return target.GetValue(MinWidthProperty); }

    public static void SetMinHeight(Visual target, object length) {
        SetRelativeDoubleProperty(target, MinHeightProperty, length);
    }

    public static void SetMinHeight(AvaloniaObject target, object length, Visual? visualAnchor = null) {
        SetRelativeDoubleProperty(target, MinHeightProperty, length, visualAnchor);
    }

    public static IRelative<double> GetMinHeight(AvaloniaObject target) { return target.GetValue(MinHeightProperty); }

    public static void SetMaxWidth(Visual target, object length) {
        SetRelativeDoubleProperty(target, WidthProperty, length);
    }

    public static void SetMaxWidth(AvaloniaObject target, object length, Visual? visualAnchor = null) {
        SetRelativeDoubleProperty(target, WidthProperty, length, visualAnchor);
    }

    public static IRelative<double> GetMaxWidth(AvaloniaObject target) { return target.GetValue(MaxWidthProperty); }

    public static void SetMaxHeight(Visual target, object length) {
        SetRelativeDoubleProperty(target, WidthProperty, length);
    }

    public static void SetMaxHeight(AvaloniaObject target, object length, Visual? visualAnchor = null) {
        SetRelativeDoubleProperty(target, WidthProperty, length, visualAnchor);
    }

    public static IRelative<double> GetMaxHeight(AvaloniaObject target) { return target.GetValue(MaxHeightProperty); }

    public static void SetBorderThickness(Visual visual, object length) {
        visual.SetValue(BorderThicknessProperty, RelativeParseHelper.ParseToThickness(visual, length));
    }

    public static IRelative<Thickness> GetBorderThickness(Visual visual) {
        return visual.GetValue(BorderThicknessProperty);
    }

    public static void SetCornerRadius(Visual visual, object length) {
        visual.SetValue(
            CornerRadiusProperty,
            length switch {
                RelativeExpression expression => RelativeCornerRadius.Parse(expression.Expression, visual),
                string s => RelativeCornerRadius.Parse(s, visual),
                IRelative<CornerRadius> l => l,
                _ => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<CornerRadius> GetCornerRadius(Visual visual) {
        return visual.GetValue(CornerRadiusProperty);
    }


    public static void SetMargin(Visual visual, object length) {
        visual.SetValue(
            MarginProperty,
            length switch {
                RelativeExpression expression => RelativeThickness.Parse(expression.Expression, visual),
                string s => RelativeThickness.Parse(s, visual),
                IRelative<Thickness> l => l,
                _ => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<Thickness> GetMargin(Visual visual) { return visual.GetValue(MarginProperty); }

    public static void SetPadding(Visual visual, object length) {
        visual.SetValue(
            PaddingProperty,
            length switch {
                RelativeExpression expression => RelativeThickness.Parse(expression.Expression, visual),
                string s => RelativeThickness.Parse(s, visual),
                IRelative<Thickness> l => l,
                _ => throw new InvalidCastException($"{length.GetType()} is not a valid type.")
            });
    }

    public static IRelative<Thickness> GetPadding(Visual visual) { return visual.GetValue(PaddingProperty); }


    public static void SetOneTimeWidth(Visual target, string s) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.WidthProperty, s);
    }

    public static void SetOneTimeWidth(AvaloniaObject target, string s, Visual visualAnchor) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.WidthProperty, s, visualAnchor);
    }

    public static void SetOneTimeMinWidth(Visual target, string s) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MinWidthProperty, s);
    }

    public static void SetOneTimeMinWidth(AvaloniaObject target, string s, Visual visualAnchor) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MinWidthProperty, s, visualAnchor);
    }

    public static void SetOneTimeMaxWidth(Visual target, string s) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MaxWidthProperty, s);
    }

    public static void SetOneTimeMaxWidth(AvaloniaObject target, string s, Visual visualAnchor) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MaxWidthProperty, s, visualAnchor);
    }

    public static void SetOneTimeHeight(Visual target, string s) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.HeightProperty, s);
    }

    public static void SetOneTimeHeight(AvaloniaObject target, string s, Visual visualAnchor) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.HeightProperty, s, visualAnchor);
    }

    public static void SetOneTimeMinHeight(Visual target, string s) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MinHeightProperty, s);
    }

    public static void SetOneTimeMinHeight(AvaloniaObject target, string s, Visual visualAnchor) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MinHeightProperty, s, visualAnchor);
    }

    public static void SetOneTimeMaxHeight(Visual target, string s) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MaxHeightProperty, s);
    }

    public static void SetOneTimeMaxHeight(AvaloniaObject target, string s, Visual visualAnchor) {
        SetOneTimeRelativeDoubleProperty(target, Layoutable.MaxHeightProperty, s, visualAnchor);
    }

    public static void SetOneTimeMargin(Visual target, string s) {
        SetOneTimeRelativeThicknessProperty(target, Layoutable.MarginProperty, s);
    }

    public static void SetOneTimeBorder(Visual target, string s) {
        SetOneTimeRelativeThicknessProperty(target, Border.BorderThicknessProperty, s);
    }

    public static void SetOneTimePadding(Visual target, string s) {
        SetOneTimeRelativeThicknessProperty(target, Decorator.PaddingProperty, s);
    }

    public static void SetOneTimeCornerRadius(Visual target, string s) {
        SetOneTimeRelativeCornerRadiusProperty(target, Border.CornerRadiusProperty, s);
    }

    public static void SetRelativeDoubleProperty(
        AvaloniaObject target,
        AvaloniaProperty property,
        object value,
        Visual? visualAnchor = null) {
        if ((visualAnchor ?? target as Visual) is null)
            throw new InvalidOperationException("Either Target or visualAnchor must be a Visual control.");
        if (visualAnchor is not null)
            (value as RelativeLengthBase)?.SetVisualAnchor(visualAnchor);
        target.SetValue(property, RelativeParseHelper.ParseToDouble(target, value, visualAnchor));
    }

    public static void SetRelativeThicknessProperty(Visual target, AvaloniaProperty property, object value) {
        target.SetValue(property, RelativeParseHelper.ParseToThickness(target, value));
    }

    public static void SetRelativeCornerRadiusProperty(Visual target, AvaloniaProperty property, object value) {
        target.SetValue(property, RelativeParseHelper.ParseToCornerRadius(target, value));
    }

    private static void SetOneTimeRelativeProperty<T>(
        AvaloniaObject target,
        AvaloniaProperty property,
        object value,
        Func<AvaloniaObject, object, Visual?, IRelative<T>> parser,
        Visual? visualAnchor = null) {
        if ((visualAnchor ?? target as Visual) is not { } anchor) {
            throw new InvalidCastException("Either target or visualAnchor must be a Visual control.");
        }

        if (!anchor.IsAttachedToVisualTree()) {
            anchor.AttachedToVisualTree -= Update;
            anchor.AttachedToVisualTree += Update;
            return;
        }

        Update();
        return;


        void Update(object? _ = null, VisualTreeAttachmentEventArgs? __ = null) {
            if ((visualAnchor ?? target as Visual) is null)
                throw new InvalidOperationException("Either Target or visualAnchor must be a Visual control.");
            if (visualAnchor is not null)
                (value as RelativeLengthBase)?.SetVisualAnchor(visualAnchor);
            target.SetValue(property, parser(target, value, visualAnchor));
        }
    }

    private static void SetOneTimeRelativeProperty<T>(
        Visual target,
        AvaloniaProperty property,
        object value,
        Func<Visual, object, IRelative<T>> parser) {
        if (!target.IsAttachedToVisualTree()) {
            target.AttachedToVisualTree -= Update;
            target.AttachedToVisualTree += Update;
            return;
        }

        Update();
        return;


        void Update(object? _ = null, VisualTreeAttachmentEventArgs? __ = null) {
            target.SetValue(property, parser(target, value));
        }
    }

    public static void SetOneTimeRelativeDoubleProperty(
        AvaloniaObject target,
        AvaloniaProperty property,
        object value,
        Visual? visualAnchor = null) {
        SetOneTimeRelativeProperty(target, property, value, RelativeParseHelper.ParseToDouble, visualAnchor);
    }

    public static void SetOneTimeRelativeThicknessProperty(Visual target, AvaloniaProperty property, object value) {
        SetOneTimeRelativeProperty(target, property, value, RelativeParseHelper.ParseToThickness);
    }

    public static void SetOneTimeRelativeCornerRadiusProperty(Visual target, AvaloniaProperty property, object value) {
        SetOneTimeRelativeProperty(target, property, value, RelativeParseHelper.ParseToCornerRadius);
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

public class OneTimeRelativeBinding(BindingBase sourceValue, string value) : MarkupExtension {
    public readonly RelativeScale Scale = RelativeScale.Parse(value);
    public readonly BindingBase SourceValue = sourceValue;

    public override object ProvideValue(IServiceProvider serviceProvider) {
        SourceValue.Mode = BindingMode.OneTime;
        SourceValue.Converter = new RelativeConverter();
        SourceValue.ConverterParameter = Scale;
        return SourceValue;
    }
}

public static class RelativeParseHelper {
    public static IRelative<double> ParseToDouble(AvaloniaObject target, object value, Visual? visualAnchor = null) {
        return value switch {
            RelativeExpression expression => RelativeLengthBase.Parse(expression.Expression, target, visualAnchor),
            string s                      => RelativeLengthBase.Parse(s, target, visualAnchor),
            IRelative<double> relative    => relative,
            _                             => throw new InvalidCastException($"{value.GetType()} is not a valid type.")
        };
    }

    public static IRelative<double> ParseToThickness(Visual target, object value) {
        return value switch {
            RelativeExpression expression => RelativeLengthBase.Parse(expression.Expression, target),
            string s                      => RelativeLengthBase.Parse(s, target),
            IRelative<double> relative    => relative,
            _                             => throw new InvalidCastException($"{value.GetType()} is not a valid type.")
        };
    }

    public static IRelative<double> ParseToCornerRadius(Visual target, object value) {
        return value switch {
            RelativeExpression expression => RelativeLengthBase.Parse(expression.Expression, target),
            string s                      => RelativeLengthBase.Parse(s, target),
            IRelative<double> relative    => relative,
            _                             => throw new InvalidCastException($"{value.GetType()} is not a valid type.")
        };
    }
}