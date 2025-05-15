# RelativeControl.Avalonia

This provides some relative features for [Avalonia](https://github.com/AvaloniaUI/Avalonia).

## Get Started

Add nuget package:

```bash
dotnet add package RelativeControl.Avalonia
```

Use `Relative`(or `Rel` as an abbreviation) in controls.

```xaml
<Window 
    ...
    xmlns:r="https://github.com/metaone01/RelativeControl.Avalonia"
    ...>
	<Button r:Rel.Width="0.5vw"/>
</Window>
```