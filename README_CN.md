# RelativeControl.Avalonia

本项目为[Avalonia](https://github.com/AvaloniaUI/Avalonia)提供了部分相对单位和功能

[English](README.md)

[在demo中查看用例](https://github.com/metaone01/RelativeControl.Avalonia/tree/main/Demo.RelativeControl/Demo.RelativeControl/MainWindow.axaml)

[更多信息](API%20References.md)

## 开始使用

### 添加NuGet包:

```bash
dotnet add package RelativeControl.Avalonia
```

### 设置值为 `double` 属性的相对值:

```xaml
<CONTROL r:Relative.Width="20pw"/>
```

> 这会设定此`Control`的宽为20%逻辑父控件宽度。
>
> 你还可以对相对长度进行加减:
> ```xaml
> <CONTROL r:Relative.Width="20pw+10ph"/>
> ```
>
> 这会设定此`Control`的宽为20%逻辑父控件宽度+10%逻辑父控件高度。


> 在code behind中，除了加减，还可以对相对单位进行乘除运算。
>
> - Multiply()和Divide()会改变其自身的倍率，同时影响所有对它的引用。
> - \* 和 / 运算会生成一个轻量级副本，不会影响原实例。

> `Relative.SetOneTimeWidth` 和 `Relative.SetOneTimeHeight` 仅在控件附加到视觉树时更新一次。

其它使用`RelativeLength`/`RelativeLengthMerge`的属性：

- Relative.Height
- Relative.MinWidth
- Relative.MinHeight
- Relative.MaxWidth
- Relative.MaxHeight
- Relative.SetOneTimeWidth
- Relative.SetOneTimeHeight

### 设定值为 `CornerRadius` 属性的相对值:

```xaml
<CONTROL r:Relative.CornerRadius="10sw 10sw+5sh 10sh-5sw 10sh"/>
```

> 这会设定此`Control`的圆角为
>
> TopLeft = 10% 自身宽度,
>
> TopRight = 10% 自身宽度 + 5% 自身高度,
>
> BottomRight = 10% 自身高度 - 5% 自身宽度,
>
> BottomLeft = 10% 自身高度

### 设定值为 `Thickness` 属性的相对值

```xaml
<CONTROL r:Relative.BorderThickness="1em 2em-5px"/>
```

> 这会设定此`Control`的值为：
>
> Horizontal(Left,Right) = 1倍字宽,
>
> Vertical(Top,Bottom) = 2倍字宽 - 5像素

其它使用`RelativeThickness`的属性:

- Relative.Margin
- Relative.Padding

### 为任意属性绑定相对值

```xaml
<CONTROL PROPERTY="{Binding SOURCE_PROPERTY,Converter={x:Static r:RelativeConverter.Instance},ConverterParameter={r:Scale 50%}}"/>
```

> 这会设定此`Property`的值为`SOURCE_PROPERTY`值的50%.
>
> 一个合法的 `SourceProperty`的值必须为以下类型:
> - double
> - 任意可转换为double的类型（如数字字符串）
> - 任意继承了`IMulDiv<RelativeScale>`或`IMulDiv<double>`的自定义结构或类.

> `RelativeBindOneTime`只在控件附加到视觉树时更新一次。

### 为非Visual控件，但是有布局属性的实例使用相对单位

> 实际上，此功能是将Source的搜索锚点由Target修改为VisualAnchor实现的。
>
> 此功能还可以用于让某个控件以另一个控件作为自己的相对源

```csharp
Button button = new();
AvaloniaObject _object = new();
Relative.SetWidth(_object,"10pw",button);    
```

### 在自定义属性中使用相对单位:

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

## 所有单位

#### 绝对单位:

    px: 设备无关像素（1/96英寸）
    cm: 厘米
    mm: 毫米
    in: 英寸

#### 相对单位:

          tpw: 模板父控件宽度
          tph: 模板父控件高度
    lpw or pw: 逻辑父控件宽度
    lph or ph: 逻辑父控件高度
          vpw: 视觉父控件宽度
          vph: 视觉父控件高度
           sw: 自身宽度
           sh: 自身高度
           em: 字宽
           vw: 窗口宽度
           vh: 窗口高度
            %: 百分比，仅用于RelativeScale