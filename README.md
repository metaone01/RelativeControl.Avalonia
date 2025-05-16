# RelativeControl.Avalonia

This provides some relative units and features for [Avalonia](https://github.com/AvaloniaUI/Avalonia).

## Get Started

Add nuget package:

```bash
dotnet add package RelativeControl.Avalonia
```

Use `Relative` in controls.

```xaml
<Window 
    ...
    xmlns:r="https://github.com/metaone01/RelativeControl.Avalonia"
    ...>
	<Button r:Relative.Width="50vw"/>
</Window>
```

## Units

Absolute Units:

    px: Pixel
    cm: Centimeter
    mm: Millimeter
    in: Inch

Relative Units:

          tpw: TemplatedParent's width
          tph: TemplatedParent's height
    lpw or pw: LogicalParent's width
    lph or ph: LogicalParent's height
          vpw: VisualParent's width
          vph: VisualParent's height
           sw: The control itself's width
           sh: The control itself's height
           em: The control's FontSize
           vw: Window's width
           vh: Window's height

## Supported Properties

\>=0.0.5

- Width
- Height
- MinWidth
- MinHeight
- MaxWidth
- MaxHeight
- BorderThickness
- CornerRadius

\>=0.1.0

- Margin
- Padding

## Breaking Changes

