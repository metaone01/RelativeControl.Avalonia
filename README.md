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
	<Button r:Relative.Width="0.5vw"/>
</Window>
```

## Supported Properties

0.0.5

- Width
- Height
- MinWidth
- MinHeight
- MaxWidth
- MaxHeight
- BorderThickness
- CornerRadius

0.1.0

- Margin
- Padding
    
