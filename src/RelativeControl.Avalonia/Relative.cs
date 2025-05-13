using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace RelativeControl.Avalonia;

public class Relative : AvaloniaObject {
    public RelativeLength? Width {
        get => GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public RelativeLength? Height {
        get => GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    public RelativeLength? MinWidth {
        get => GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    public RelativeLength? MinHeight {
        get => GetValue(MinHeightProperty);
        set => SetValue(MinHeightProperty, value);
    }

    public RelativeLength? MaxWidth {
        get => GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    public RelativeLength? MaxHeight {
        get => GetValue(MinHeightProperty);
        set => SetValue(MinHeightProperty, value);
    }

    public RelativeThickness? BorderThickness {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public RelativeLength? CornerRadius {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }


    public static readonly AttachedProperty<RelativeLength?> WidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(Width));

    public static readonly AttachedProperty<RelativeLength?> HeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(Height));

    public static readonly AttachedProperty<RelativeLength?> MinWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MinWidth));

    public static readonly AttachedProperty<RelativeLength?> MinHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MinHeight));

    public static readonly AttachedProperty<RelativeLength?> MaxWidthProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MaxWidth));

    public static readonly AttachedProperty<RelativeLength?> MaxHeightProperty =
        AvaloniaProperty.RegisterAttached<Relative, Layoutable, RelativeLength?>(nameof(MaxHeight));

    public static readonly AttachedProperty<RelativeThickness?> BorderThicknessProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, RelativeThickness?>(nameof(BorderThickness));

    public static readonly AttachedProperty<RelativeLength?> CornerRadiusProperty =
        AvaloniaProperty.RegisterAttached<Relative, TemplatedControl, RelativeLength?>(nameof(CornerRadius));

    static Relative() {
        WidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is not RelativeLength nl)
                return;
            nl.SetTarget(layoutable);
            layoutable.Width = nl.Absolute();
        });
        HeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is not RelativeLength nl)
                return;
            nl.SetTarget(layoutable);
            layoutable.Width = nl.Absolute();
        });
        MinWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is not RelativeLength nl)
                return;
            nl.SetTarget(layoutable);
            layoutable.Width = nl.Absolute();
        });
        MinHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is not RelativeLength nl)
                return;
            nl.SetTarget(layoutable);
            layoutable.Width = nl.Absolute();
        });
        MaxWidthProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is not RelativeLength nl)
                return;
            nl.SetTarget(layoutable);
            layoutable.Width = nl.Absolute();
        });
        MaxHeightProperty.Changed.AddClassHandler<Layoutable>((layoutable, args) => {
            if (args.NewValue is not RelativeLength nl)
                return;
            nl.SetTarget(layoutable);
            layoutable.Width = nl.Absolute();
        });
        BorderThicknessProperty.Changed.AddClassHandler<TemplatedControl>((templatedControl, args) => {
            if (args.NewValue is not RelativeThickness nt)
                return;
            nt.WithTarget(templatedControl);
            templatedControl.BorderThickness = nt.Absolute();
        });
        CornerRadiusProperty.Changed.AddClassHandler<TemplatedControl>((templatedControl, args) => {
            if (args.NewValue is not RelativeCornerRadius ncr)
                return;
            ncr.WithTarget(templatedControl);
            templatedControl.CornerRadius = ncr.Absolute();
        });
    }
}