# RelativeControl.Avalonia

This provides some relative units and features for [Avalonia](https://github.com/AvaloniaUI/Avalonia).

## Get Started

#### Add nuget package:

```bash
dotnet add package RelativeControl.Avalonia
```

#### Use `Relative` in controls.

```xaml
<Window 
    ...
    xmlns:r="https://github.com/metaone01/RelativeControl.Avalonia"
    ...>
	<Button r:Relative.Width="50vw"/>
</Window>
```

#### Bind Custom Properties:

*requires Version>=1.0.0*

```xaml
<Button YourCustomProperty="{r:RelativeBinding {Binding RELATIVE_PROPERTY},50%}">
</Button>
```

## Units

#### Absolute Units:

    px: Pixel
    cm: Centimeter
    mm: Millimeter
    in: Inch

#### Relative Units:

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
            %: Only used for custom bindings. Represents percentage.

## Supported Properties

### \>=0.0.5

- Width
- Height
- MinWidth
- MinHeight
- MaxWidth
- MaxHeight
- BorderThickness
- CornerRadius

### \>=0.1.0

- Margin
- Padding

### \>=1.0.0

- All Custom Properties! *[How to bind a custom property?](#bind-custom-properties)*

## Breaking Changes

### 0.2.1:

Relative units are using percentages (Excepts `em`). To make it more like *css*.

```diff
- <Button r:Relative.Width="0.5vw"/>
+ <Button r:Relative.Width="50vw"/>
```

### 1.0.0:

*Move all relative length from **Units.cs** to **RelativeLength.cs***

#### RelativeLength.cs:

```diff
+ public abstract class RelativeLengthBase

- RelativeLength.RelativeLengthChanged (Rename)
+ RelativeLengthBase.RelativeLengthChanged

- RelativeLengthBase.OnRelativeLengthChanged (Rename)
+ RelativeLengthBase.RelativeLengthChanged

- RelativeMerge.Multiplier (Rename)
+ RelativeLengthCollection.Scaler

- SetTarget
```

#### RelativeThickness.cs:

```diff
- RelativeThicknessChanged (Rename)
+ RelativeThicknessChangedHandler

- OnRelativeThicknessChanged (Rename)
+ RelativeThicknessChanged

- SetTarget
```

#### RelativeSize.cs:

```diff
- RelativeSizeChanged (Rename)
+ RelativeSizeChangedHandler

- OnRelativeSizeChanged (Rename)
+ RelativeSizeChanged

- SetTarget
```

#### RelativeCornerRadius.cs:

```diff
- RelativeCornerRadiusChanged (Rename)
+ RelativeCornerRadiusChangedHandler

- OnRelativeCornerRadiusChanged (Rename)
+ RelativeCornerRadiusChanged

- SetTarget
```
 






