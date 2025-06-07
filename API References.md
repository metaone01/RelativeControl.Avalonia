## USAGE

### You can use a `RelativeLength` / `RelativeLengthMerge` like this:

```xaml
<CONTROL r:Relative.Width="20pw"/>
```

> This will set the control's width to 20% of its logical parent's width.
>
> You can also add / subtract the values:
> ```xaml
> <CONTROL r:Relative.Width="20pw+10ph"/>
> ```
>
> This will set the control's width to :
>
> 20% logical parent width + 10% logical parent height.

> Use `Relative.SetOneTimeWidth` / `Relative.SetOneTimeHeight` to set its value only once when attached to visual tree.

Other properties using RelativeLength:

- Relative.Height
- Relative.MinWidth
- Relative.MinHeight
- Relative.MaxWidth
- Relative.MaxHeight
- Relative.SetOneTimeWidth
- Relative.SetOneTimeHeight

### You can create a `RelativeCornerRadius` like this:

```xaml
<CONTROL r:Relative.CornerRadius="10sw 10sw+5sh 10sh-5sw 10sh"/>
```

> This will set the control's CornerRadius to:
>
> TopLeft = 10% width,
>
> TopRight = 10% width + 5% height,
>
> BottomRight = 10% height - 5% width,
>
> BottomLeft = 10% height

### You can create a `RelativeThickness` like this:

```xaml
<CONTROL r:Relative.BorderThickness="1em 2em-5px"/>
```

> This will set the control's BorderThickness to:
>
> Horizontal(Left,Right) = 1x FontSize,
> Vertical(Top,Bottom) = 2x FontSize - 5px

Other properties using RelativeThickness:

- Relative.Margin
- Relative.Padding

### You can bind any property like this:

```xaml
<CONTROL PROPERTY="{r:RelativeBinding {Binding SOURCE_PROPERTY},50%}"/>
```

> This will set the property to 50% of `SOURCE_PROPERTY`'s value.
>
> A valid `SourceProperty`'s value type must be:
> - double
> - any value that can convert to double (like a number string)
> - any custom structs or classes that inherits IMulDiv\<RelativeScale\> or IMulDiv\<double\>.

> Use r:RelativeBindOneTime to set its value only once when attached to visual tree.

### Use Relatives in your custom property:

StyledProperty:

```csharp
public static readonly StyledProperty<IRelative<T>> XXXProperty = 
    AvaloniaProperty.Register<..., IRelative<T>>(nameof(XXX));
```

DirectProperty:

```csharp
public static readonly DirectProperty<..., IRelative<T>> XXXProperty = 
    AvaloniaProperty.RegisterDirect<..., IRelative<T>>(...);
```

AttachedProperty:

```csharp
public static readonly AttachedProperty<IRelative<T>> XXXProperty = 
    AvaloniaProperty.RegisterAttached<...,...,IRelative<T>>(...);
```

## INFO

### Relative.cs

- Set

    - Relative.SetWidth
    - Relative.SetHeight
    - Relative.SetMinWidth
    - Relative.SetMinHeight
    - Relative.SetMaxWidth
    - Relative.SetMaxHeight
    - Relative.SetBorderThickness
    - Relative.SetCornerRadius
    - Relative.SetMargin
    - Relative.SetPadding
    - Relative.SetOneTimeWidth
    - Relative.SetOneTimeHeight

- Get

    - Relative.GetWidth
    - Relative.GetHeight
    - Relative.GetMinWidth
    - Relative.GetMinHeight
    - Relative.GetMaxWidth
    - Relative.GetMaxHeight
    - Relative.GetBorderThickness
    - Relative.GetCornerRadius
    - Relative.GetMargin
    - Relative.GetPadding

### RelativeLength.cs

- Interface

    - IAddSub\<T\>
    - IMulDiv\<T\>
    - ICopiable\<T\>
    - IRelative\<T\>

- Class

    - RelativeLengthBase
    - SingleRelativeLength
    - RelativeLengthCollection
    - RelativeLength
    - RelativeLengthMerge

- Struct

    - RelativeScale
    - RelativeCalc

