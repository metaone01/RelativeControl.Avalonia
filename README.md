﻿# RelativeControl.Avalonia

This provides some relative units and features for [Avalonia](https://github.com/AvaloniaUI/Avalonia).

[中文](README_CN.md)

[See Usages in Demo](./Demo.RelativeControl/Demo.RelativeControl/MainWindow.axaml)

[More Info](./API%20References.md)

## Get Started

### Add NuGet package:

```bash
dotnet add package RelativeControl.Avalonia
```

### Set a property whose value is `double`

```xaml
<CONTROL r:Relative.Width="20pw"/>
```

> This will set the `CONTROL`'s width to 20% of its logical parent's width.
>
> You can also add / subtract the values:
> ```xaml
> <CONTROL r:Relative.Width="20pw+10ph"/>
> ```
>
> This will set the `CONTROL`'s width to :
>
> 20% logical parent width + 10% logical parent height.

> You can also add / subtract / multiply / divide the value at code behind.
>
> - Multiply() and Divide() will affect the instance and all its references.
> - \* or / operation will create a light copy and will not affect the instance.

> `Relative.SetOneTimeWidth` and `Relative.SetOneTimeHeight` will update only once when the control is attached to
> visual tree.

Other properties using `RelativeLength` / `RelativeLengthMerge`:

- Relative.Height
- Relative.MinWidth
- Relative.MinHeight
- Relative.MaxWidth
- Relative.MaxHeight
- Relative.SetOneTimeWidth
- Relative.SetOneTimeHeight

### Set a property whose value is `CornerRadius`

```xaml
<CONTROL r:Relative.CornerRadius="10sw 10sw+5sh 10sh-5sw 10sh"/>
```

> This will set the `CONTROL`'s `CornerRadius` to:
>
> TopLeft = 10% width,
>
> TopRight = 10% width + 5% height,
>
> BottomRight = 10% height - 5% width,
>
> BottomLeft = 10% height

### Set a property whose value is `Thickness`

```xaml
<CONTROL r:Relative.BorderThickness="1em 2em-5px"/>
```

> This will set the `CONTROL`'s BorderThickness to:
>
> Horizontal(Left,Right) = 1x FontSize,
>
> Vertical(Top,Bottom) = 2x FontSize - 5px

Other properties using `RelativeThickness`:

- Relative.Margin
- Relative.Padding

### Bind any property

```xaml
<CONTROL PROPERTY="{r:RelativeBinding {Binding SOURCE_PROPERTY},50%}"/>
```

> This will set the `PROPERTY`'s value to 50% of `SOURCE_PROPERTY`'s value.
>
> A valid `SourceProperty`'s value must be:
> - a double
> - any value that can convert to double (like a number string)
> - any custom structs or classes that inherits `IMulDiv<RelativeScale>` or `IMulDiv<double>`.

> `RelativeBindOneTime` will update only once when the control is attached to visual tree.

### Use Relatives in your custom property:

#### StyledProperty:

```csharp
public static readonly StyledProperty<IRelative<T>> XXXProperty = 
    AvaloniaProperty.Register<..., IRelative<T>>(nameof(XXX));
```

#### DirectProperty:

```csharp
public static readonly DirectProperty<..., IRelative<T>> XXXProperty = 
    AvaloniaProperty.RegisterDirect<..., IRelative<T>>(...);
```

#### AttachedProperty:

```csharp
public static readonly AttachedProperty<IRelative<T>> XXXProperty = 
    AvaloniaProperty.RegisterAttached<...,...,IRelative<T>>(...);
```

## Units

#### Absolute Units:

    px: Device-independent Pixel(1/96th of an inch)
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
            %: Represents percentage. Only used for custom bindings. 

## Minimum Available Version of an API

### 0.0.5

- Width
- Height
- MinWidth
- MinHeight
- MaxWidth
- MaxHeight
- BorderThickness
- CornerRadius

### 0.1.0

- Margin
- Padding

### 1.0.0-alpha

- RelativeBinding

> You can bind any property! *[How to bind a custom property?](#bind-any-property)*

### 1.0.0-beta

- SetOneTimeWidth
- SetOneTimeHeight
- RelativeBindOneTime

> These will update only once when the control is attached to visual tree!

## Breaking Changes

### 0.2.1:

Relative units are using percentages (Excepts `em`). To make it more like *css*.

```diff
- <Button r:Relative.Width="0.5vw"/>
+ <Button r:Relative.Width="50vw"/>
```

### 1.0.0-alpha:

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

### 1.0.0-beta:

```diff
- public event RelativeXXXChanged(T oldValue,T newValue);
+ public event RelativeChanged<T>(IRelative<T> sender,RelativeChangedEventArgs<T> args);
+ public class RelativeChangedEventArgs<T>(T oldValue, T newValue) : RelativeChangedEventArgs {
+     public readonly T OldValue = oldValue;
+     public readonly T NewValue = newValue;
+ }
```