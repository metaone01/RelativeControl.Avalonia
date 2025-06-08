using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace RelativeControl.Avalonia;

public class RelativeChangedEventArgs : EventArgs {
    public new static readonly RelativeChangedEventArgs Empty = new();
}

public class RelativeChangedEventArgs<T>(T oldValue, T newValue) : RelativeChangedEventArgs {
    public readonly T NewValue = newValue;
    public readonly T OldValue = oldValue;
}

public delegate void RelativeChangedEventHandler<T>(IRelative<T>? sender, RelativeChangedEventArgs<T> args);

/// <summary>
///     Addable and Subtractable.
/// </summary>
public interface IAddSub<in T> {
    void Add(T other);
    void Subtract(T other);
}

/// <summary>
///     Multipliable and Dividable.
/// </summary>
public interface IMulDiv<in T> {
    void Multiply(T other);
    void Divide(T other);
}

public interface ICopiable<out T> {
    T Copy();
    T LightCopy();
}

public interface IRelative<T> {
    /// <summary>
    ///     This property returns the true absolute value for calculation.
    /// </summary>
    T ActualValue { get; }

    /// <summary>
    ///     This function should return a non-negative value to make sure no invalid value while applying to UI.
    /// </summary>
    /// <returns>non-negative <see cref="T" />.</returns>
    T Absolute();

    event RelativeChangedEventHandler<T> RelativeChanged;
}

public abstract class RelativeLengthBase : IRelative<double>, ICopiable<RelativeLengthBase> {
    /// <summary>
    ///     This returns its actual pixels.
    /// </summary>
    public abstract double ActualPixels { get; }

    public abstract RelativeLengthBase Copy();

    public abstract RelativeLengthBase LightCopy();

    public virtual double ActualValue => ActualPixels;

    public virtual double Absolute() { return double.Max(ActualPixels, 0); }
    public abstract event RelativeChangedEventHandler<double>? RelativeChanged;

    protected abstract void InvokeIfChanged(double oldActualPixels, double newActualPixels);

    [Pure]
    public static RelativeLengthBase Min(RelativeLengthBase left, RelativeLengthBase right) {
        if (left.ActualPixels is double.NaN)
            return right;
        if (right.ActualPixels is double.NaN)
            return left;
        return left.ActualPixels < right.ActualPixels ? left : right;
    }

    [Pure]
    public static RelativeLengthBase Max(RelativeLengthBase left, RelativeLengthBase right) {
        if (left.ActualPixels is double.NaN)
            return right;
        if (right.ActualPixels is double.NaN)
            return left;
        return left.ActualPixels > right.ActualPixels ? left : right;
    }

    public static RelativeLengthBase Parse(string s, Visual? visual = null) {
        var lengths = new List<RelativeLength>(ParseSingle(s, visual));
        return lengths.Count switch {
            0 => RelativeLength.Empty,
            1 => lengths[0],
            _ => new RelativeLengthMerge(lengths)
        };
    }

    private static IEnumerable<RelativeLength> ParseSingle(string s, Visual? visual = null) {
        s = s.Trim();
        if (s.Length == 0)
            yield break;
        int begin = 0;
        bool isPositive = s[0] != '-';
        for (int i = 1; i < s.Length; i++) {
            if (s[i] is not ('+' or '-'))
                continue;
            RelativeLength length = RelativeLength.Parse(s[begin..i], visual);
            if (!isPositive)
                length.Multiply(-1);
            yield return length;
            isPositive = s[i] == '+';
            begin = i + 1;
        }

        yield return RelativeLength.Parse(s[begin..], visual);
    }

    public static RelativeLengthBase operator +(RelativeLengthBase left, RelativeLengthBase right) {
        if (left is SingleRelativeLength l && right is SingleRelativeLength r)
            return l + r;
        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLengthBase operator -(RelativeLengthBase left, RelativeLengthBase right) {
        if (left is SingleRelativeLength l && right is SingleRelativeLength r)
            return l - r;
        return new RelativeLengthMerge(left, right * -1);
    }

    public static RelativeLengthBase operator *(RelativeLengthBase left, RelativeScale right) {
        return left switch {
            SingleRelativeLength srl     => srl * right,
            RelativeLengthCollection rlc => rlc * right,
            _                            => throw new NotSupportedException()
        };
    }

    public static RelativeLengthBase operator *(RelativeScale left, RelativeLengthBase right) { return right * left; }

    public static RelativeLengthBase operator /(RelativeLengthBase left, RelativeScale right) {
        return left switch {
            SingleRelativeLength slr     => slr / right,
            RelativeLengthCollection rlc => rlc / right,
            _                            => throw new NotSupportedException()
        };
    }

    [Pure]
    public static bool operator <(RelativeLengthBase a, RelativeLengthBase b) {
        return a.ActualPixels < b.ActualPixels;
    }

    [Pure]
    public static bool operator <(RelativeLength a, RelativeLengthBase b) { return a.ActualPixels < b.ActualPixels; }

    [Pure]
    public static bool operator <(RelativeLengthBase a, RelativeLength b) { return a.ActualPixels < b.ActualPixels; }

    [Pure]
    public static bool operator >(RelativeLengthBase a, RelativeLengthBase b) {
        return a.ActualPixels > b.ActualPixels;
    }

    [Pure]
    public static bool operator >(RelativeLength a, RelativeLengthBase b) { return a.ActualPixels > b.ActualPixels; }

    [Pure]
    public static bool operator >(RelativeLengthBase a, RelativeLength b) { return a.ActualPixels > b.ActualPixels; }
}

public abstract class SingleRelativeLength : RelativeLengthBase, IMulDiv<RelativeScale> {
    public virtual double Value { [Pure] get; protected set; }
    public virtual Units Unit { [Pure] get; protected init; }

    protected internal virtual WeakReference<Visual>? Source { [Pure] get; protected set; }

    protected internal virtual WeakReference<Visual>? Target { [Pure] get; protected init; }

    public virtual void Multiply(RelativeScale other) { Value *= other.Scale; }

    public virtual void Divide(RelativeScale other) { Value /= other.Scale; }

    [Pure]
    public virtual Visual? GetTarget() {
        if (Target is null)
            return null;
        Target.TryGetTarget(out Visual? target);
        return target;
    }

    [Pure]
    public virtual Visual? GetSource() {
        if (Source is null)
            return null;
        Source.TryGetTarget(out Visual? source);
        return source;
    }

    public abstract override SingleRelativeLength Copy();
    public abstract override LightSingleRelativeLength LightCopy();

    [Pure]
    public static SingleRelativeLength Min(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.ActualPixels is double.NaN)
            return right;
        if (right.ActualPixels is double.NaN)
            return left;
        return left.ActualPixels < right.ActualPixels ? left : right;
    }

    [Pure]
    public static SingleRelativeLength Max(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.ActualPixels is double.NaN)
            return right;
        if (right.ActualPixels is double.NaN)
            return left;
        return left.ActualPixels > right.ActualPixels ? left : right;
    }

    [Pure] public override string ToString() { return $"{Value}{Converters.UnitToString(Unit)}"; }

    [Pure]
    public static RelativeLengthBase operator +(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.Unit.Equals(right.Unit) && left.GetTarget() == right.GetTarget())
            return new RelativeLength(left.Value + right.Value, left.Unit, left.GetTarget());
        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLength(left.ActualPixels + right.ActualPixels);
        return new RelativeLengthMerge(left, right);
    }

    [Pure]
    public static RelativeLengthBase operator -(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.Unit.Equals(right.Unit) && left.GetTarget() == right.GetTarget())
            return new RelativeLength(left.Value - right.Value, left.Unit, left.GetTarget());
        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLength(left.ActualPixels - right.ActualPixels);
        return new RelativeLengthMerge(left, right * -1);
    }

    [Pure]
    public static SingleRelativeLength operator *(SingleRelativeLength relativeLength, RelativeScale scale) {
        SingleRelativeLength lightCopy = relativeLength.LightCopy();
        lightCopy.Value *= scale.Scale;
        return lightCopy;
    }

    [Pure]
    public static SingleRelativeLength operator *(RelativeScale scale, SingleRelativeLength relativeLength) {
        return relativeLength * scale;
    }

    [Pure]
    public static SingleRelativeLength operator /(SingleRelativeLength relativeLength, RelativeScale scale) {
        SingleRelativeLength lightCopy = relativeLength.LightCopy();
        lightCopy.Value /= scale.Scale;
        return lightCopy;
    }
}

public abstract class RelativeLengthCollection : RelativeLengthBase,
                                                 IAddSub<RelativeLengthBase>,
                                                 IMulDiv<RelativeScale> {
    public virtual List<RelativeLengthBase> Children { [Pure] get; } = [];
    public virtual double Scale { [Pure] get; protected set; } = 1D;
    public virtual void Add(RelativeLengthBase elements) { Add([elements]); }
    public virtual void Subtract(RelativeLengthBase other) { Add(other * -1); }

    public void Multiply(RelativeScale other) {
        double old = ActualPixels;
        Scale *= other.Scale;
        InvokeIfChanged(old, ActualPixels);
    }

    public void Divide(RelativeScale other) {
        double old = ActualPixels;
        Scale /= other.Scale;
        InvokeIfChanged(old, ActualPixels);
    }

    public abstract override RelativeLengthCollection Copy();
    public abstract override RelativeLengthCollection LightCopy();
    public abstract void Add(params RelativeLengthBase[] elements);
    public abstract void Remove(params RelativeLengthBase[] elements);

    public static RelativeLengthMerge operator +(RelativeLengthCollection left, RelativeLengthBase right) {
        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLengthMerge operator -(RelativeLengthCollection left, RelativeLengthBase right) {
        return new RelativeLengthMerge(left, right * -1);
    }

    public static RelativeLengthCollection operator *(RelativeLengthCollection self, RelativeScale scale) {
        RelativeLengthCollection lightCopy = self.LightCopy();
        lightCopy.Scale *= scale.Scale;
        return lightCopy;
    }

    public static RelativeLengthCollection operator *(RelativeScale scale, RelativeLengthCollection self) {
        return self * scale;
    }

    public static RelativeLengthCollection operator /(RelativeLengthCollection self, RelativeScale scale) {
        RelativeLengthCollection lightCopy = self.LightCopy();
        lightCopy.Scale /= scale.Scale;
        return lightCopy;
    }

    public override string ToString() {
        string result = "";
        foreach (RelativeLengthBase length in Children)
            if (length is SingleRelativeLength single) {
                if (single.Value >= 0)
                    result += '+';
                result += single.ToString();
            } else {
                result += $"({length})";
            }

        return result[0] == '+' ? result[1..] : result;
    }
}

public sealed class RelativeLengthMerge : RelativeLengthCollection {
    public static readonly RelativeLengthMerge Empty = new();
    private double _actualPixels;
    private RelativeLengthMerge() { }

    public RelativeLengthMerge(params RelativeLengthBase[] lengths) { Add(lengths); }

    public RelativeLengthMerge(params ICollection<RelativeLengthBase>[] lengthsArray) {
        foreach (ICollection<RelativeLengthBase> lengthCollection in lengthsArray) {
            Children.AddRange(lengthCollection);
            foreach (RelativeLengthBase length in lengthCollection) {
                _actualPixels += length.ActualPixels;
                length.RelativeChanged += Update;
            }
        }

        InvokeIfChanged(0, ActualPixels);
    }

    public RelativeLengthMerge(params IEnumerable<RelativeLengthBase>[] lengthsArray) {
        foreach (IEnumerable<RelativeLengthBase> lengths in lengthsArray)
        foreach (RelativeLengthBase length in lengths) {
            Children.Add(length);
            _actualPixels += length.ActualPixels;
            length.RelativeChanged += Update;
        }

        InvokeIfChanged(0, ActualPixels);
    }

    public RelativeLengthMerge(ICollection<RelativeLengthBase> lengths) { AddRange(lengths); }

    public RelativeLengthMerge(IEnumerable<RelativeLengthBase> lengths) { AddRange(lengths); }
    [Pure] public override double ActualPixels => _actualPixels * Scale;

    public override RelativeLengthMerge Copy() { return new RelativeLengthMerge(Children); }
    [Pure] public override RelativeLengthMerge LightCopy() { return new RelativeLengthMerge(this); }

    public override event RelativeChangedEventHandler<double>? RelativeChanged;

    protected override void InvokeIfChanged(double oldActualPixels, double newActualPixels) {
        if (Math.Abs(oldActualPixels - newActualPixels) > 1e-5)
            RelativeChanged?.Invoke(this, new RelativeChangedEventArgs<double>(oldActualPixels, newActualPixels));
    }

    private void Update(object? sender, RelativeChangedEventArgs<double> args) {
        double old = ActualPixels;
        _actualPixels -= args.OldValue;
        _actualPixels += args.NewValue;
        InvokeIfChanged(old, ActualPixels);
    }

    public override void Add(params RelativeLengthBase[] lengths) {
        double old = ActualPixels;
        foreach (RelativeLengthBase length in lengths) {
            _actualPixels += length.ActualPixels;
            Children.Add(length);
            length.RelativeChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public void AddRange(ICollection<RelativeLengthBase> lengths) {
        double old = ActualPixels;
        Children.AddRange(lengths);
        foreach (RelativeLengthBase length in lengths) {
            _actualPixels += length.ActualPixels;
            length.RelativeChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public void AddRange(IEnumerable<RelativeLengthBase> lengths) {
        double old = ActualPixels;
        foreach (RelativeLengthBase length in lengths) {
            Children.Add(length);
            _actualPixels += length.ActualPixels;
            length.RelativeChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public override void Remove(params RelativeLengthBase[] removes) {
        double old = ActualPixels;
        foreach (RelativeLengthBase length in removes) {
            if (Children.Remove(length))
                _actualPixels -= length.ActualPixels;
            length.RelativeChanged -= Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public static RelativeLengthMerge operator +(RelativeLengthMerge self, RelativeLengthBase right) {
        RelativeLengthMerge lightCopy = self.LightCopy();
        lightCopy.Add(right);
        return lightCopy;
    }

    public static RelativeLengthMerge operator -(RelativeLengthMerge self, RelativeLengthBase right) {
        RelativeLengthMerge lightCopy = self.LightCopy();
        lightCopy.Subtract(right);
        return lightCopy;
    }


    public static RelativeLengthMerge operator *(RelativeLengthMerge self, RelativeScale scale) {
        RelativeLengthMerge lightCopy = self.LightCopy();
        lightCopy.Multiply(scale);
        return lightCopy;
    }

    public static RelativeLengthMerge operator *(RelativeScale scale, RelativeLengthMerge self) { return self * scale; }

    public static RelativeLengthMerge operator /(RelativeLengthMerge self, RelativeScale scale) {
        RelativeLengthMerge lightCopy = self.LightCopy();
        lightCopy.Divide(scale);
        return lightCopy;
    }

    public new static RelativeLengthMerge Parse(string s, Visual? visual = null) {
        return new RelativeLengthMerge(ParseSingle(s, visual));
    }

    private static IEnumerable<RelativeLength> ParseSingle(string s, Visual? visual = null) {
        s = s.Trim();
        if (s.Length == 0)
            yield break;
        int begin = 0;
        bool isPositive = s[0] != '-';
        for (int i = 1; i < s.Length; i++) {
            if (s[i] is not ('+' or '-'))
                continue;
            RelativeLength length = RelativeLength.Parse(s[begin..i], visual);
            if (!isPositive)
                length.Multiply(-1);
            yield return length;
            isPositive = s[i] == '+';
            begin = i + 1;
        }

        yield return RelativeLength.Parse(s[begin..], visual);
    }

    ~RelativeLengthMerge() {
        foreach (RelativeLengthBase length in Children)
            length.RelativeChanged -= Update;
    }
}

public sealed class RelativeLength : SingleRelativeLength {
    public static readonly RelativeLength Empty = new(0D);
    public static readonly RelativeLength PositiveInfinity = new(double.PositiveInfinity);
    public static readonly RelativeLength NegativeInfinity = new(double.NegativeInfinity);

    private double _actualPixels;


    /// <summary>
    ///     A relative length to calculate the target's property with specified unit.
    /// </summary>
    /// <param name="value">Relative value.</param>
    /// <param name="unit">Relative unit.</param>
    /// <param name="target">The target control.</param>
    public RelativeLength(double value, Units unit = Units.Pixel, Visual? target = null) {
        Value = value;
        Unit = unit;
        Target = target is null ? null : new WeakReference<Visual>(target);
        Initialize();
    }

    /// <inheritdoc />
    public RelativeLength(double value, string unit, Visual? target = null) : this(
        value,
        Converters.StringToUnit(unit),
        target) { }

    [Pure] public override double ActualPixels => _actualPixels;

    public override event RelativeChangedEventHandler<double>? RelativeChanged;

    protected override void InvokeIfChanged(double oldActualPixels, double newActualPixels) {
        if (Math.Abs(oldActualPixels - newActualPixels) > 1e-5)
            RelativeChanged?.Invoke(this, new RelativeChangedEventArgs<double>(oldActualPixels, newActualPixels));
    }

    public override RelativeLength Copy() { return new RelativeLength(Value, Unit, GetTarget()); }

    public override LightSingleRelativeLength LightCopy() { return new LightSingleRelativeLength(this); }

    private void SetSource() {
        if (Unit switch {
                Units.TemplatedParentWidth  => GetTarget()?.TemplatedParent as Visual,
                Units.TemplatedParentHeight => GetTarget()?.TemplatedParent as Visual,
                Units.LogicalParentWidth    => GetTarget()?.Parent as Visual,
                Units.LogicalParentHeight   => GetTarget()?.Parent as Visual,
                Units.VisualParentWidth     => GetTarget()?.GetVisualParent(),
                Units.VisualParentHeight    => GetTarget()?.GetVisualParent(),
                Units.SelfWidth             => GetTarget(),
                Units.SelfHeight            => GetTarget(),
                Units.FontSize              => GetTarget(),
                Units.ViewPortWidth         => TopLevel.GetTopLevel(GetTarget()),
                Units.ViewPortHeight        => TopLevel.GetTopLevel(GetTarget()),
                _                           => null
            } is { } source)
            Source = new WeakReference<Visual>(source);
        else
            throw new InvalidOperationException($"Cannot find {GetTarget()}'s relative source.");
    }

    private void Initialize() {
        if (Unit.IsAbsolute()) {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            _actualPixels = Unit switch {
                Units.Pixel      => Value,
                Units.Centimeter => 96 / 2.54 * Value,
                Units.Millimeter => 96 / 2.54 * Value / 1000,
                Units.Inch       => 96 * Value,
                _                => throw new ArgumentOutOfRangeException()
            };
            return;
        }

        if (GetTarget() is not { } target)
            return;
        if (!target.IsAttachedToVisualTree()) {
            target.AttachedToVisualTree += UpdateOnAttachedToVisualTree;
        } else {
            SetSource();
            if (Unit.IsRelative() && GetSource() is { } source)
                source.PropertyChanged += Update;
        }

        Update();
    }

    private void UpdateOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
        SetSource();
        if (Unit.IsRelative() && GetSource() is { } source)
            source.PropertyChanged += Update;
        Update();
        GetTarget()!.AttachedToVisualTree -= UpdateOnAttachedToVisualTree;
    }

    private void Update(object? sender = null, AvaloniaPropertyChangedEventArgs? args = null) {
        if (GetSource() is not { } source)
            return;
        double oldActualPixels = _actualPixels;
        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        _actualPixels = Unit switch {
            Units.TemplatedParentWidth  => GetWidth() * Value / 100d,
            Units.TemplatedParentHeight => GetHeight() * Value / 100d,
            Units.LogicalParentWidth    => GetWidth() * Value / 100d,
            Units.LogicalParentHeight   => GetHeight() * Value / 100d,
            Units.VisualParentWidth     => GetWidth() * Value / 100d,
            Units.VisualParentHeight    => GetHeight() * Value / 100d,
            Units.SelfWidth             => GetWidth() * Value / 100d,
            Units.SelfHeight            => GetHeight() * Value / 100d,
            Units.FontSize              => source.GetValue(TextElement.FontSizeProperty) * Value, // not percent
            Units.ViewPortWidth         => (source as TopLevel)!.ClientSize.Width * Value / 100d,
            Units.ViewPortHeight        => (source as TopLevel)!.ClientSize.Height * Value / 100d,
#pragma warning disable CA2208
            _ => throw new ArgumentOutOfRangeException(nameof(Unit))
#pragma warning restore CA2208
        };
        InvokeIfChanged(oldActualPixels, _actualPixels);
        return;

        double GetWidth() {
            double width = source.GetValue(Layoutable.WidthProperty);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (width is not double.NaN and not double.PositiveInfinity and not double.NegativeInfinity)
                return width;
            return source.Bounds.Width;
        }

        double GetHeight() {
            double height = source.GetValue(Layoutable.HeightProperty);
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (height is not double.NaN and not double.PositiveInfinity and not double.NegativeInfinity)
                return height;
            return source.Bounds.Height;
        }
    }

    /// <summary>
    ///     Parse a string to relative length.
    /// </summary>
    /// <param name="length">A string representing relative length.</param>
    /// <param name="target">The target control.</param>
    public new static RelativeLength Parse(string length, Visual? target = null) {
        length = length.Trim();
        if (char.IsNumber(length[^1]))
            return new RelativeLength(Convert.ToDouble(length));

        int i = length.Length - 2;
        while (!(char.IsNumber(length[i]) || length[i] == '.'))
            --i;

        return new RelativeLength(
            Convert.ToDouble(length[..(i + 1)]),
            Converters.StringToUnit(length[(i + 1)..]),
            target);
    }

    public static LightSingleRelativeLength operator *(RelativeLength relativeLength, RelativeScale scale) {
        LightSingleRelativeLength lightCopy = relativeLength.LightCopy();
        lightCopy.Multiply(scale);
        return lightCopy;
    }

    public static LightSingleRelativeLength operator *(RelativeScale scale, RelativeLength relativeLength) {
        return relativeLength * scale;
    }

    public static LightSingleRelativeLength operator /(RelativeLength relativeLength, RelativeScale scale) {
        LightSingleRelativeLength lightCopy = relativeLength.LightCopy();
        lightCopy.Divide(scale);
        return lightCopy;
    }

    public static implicit operator RelativeLength(double value) { return new RelativeLength(value); }

    ~RelativeLength() {
        RelativeChanged = null;
        if (GetSource() is { } source)
            source.PropertyChanged -= Update;
    }
}

public sealed class LightSingleRelativeLength : SingleRelativeLength {
    public LightSingleRelativeLength(SingleRelativeLength length) {
        Base = length;
        if (RelativeChanged is not null)
            Base.RelativeChanged += Update;
    }

    public override double Value { get; protected set; } = 100D;
    public override Units Unit => Base.Unit;
    public override double ActualPixels => Base.ActualPixels * Value / 100D;

    public SingleRelativeLength Base { get; }

    protected internal override WeakReference<Visual>? Target => Base.Target;
    protected internal override WeakReference<Visual>? Source => Base.Source;

    public override event RelativeChangedEventHandler<double>? RelativeChanged;

    public override LightSingleRelativeLength Copy() { return new LightSingleRelativeLength(this); }
    public override LightSingleRelativeLength LightCopy() { return Copy(); }

    private void Update(object? _, RelativeChangedEventArgs<double> args) { RelativeChanged?.Invoke(this, args); }

    protected override void InvokeIfChanged(double oldActualPixels, double newActualPixels) {
        if (Math.Abs(oldActualPixels - newActualPixels) > 1e-5)
            RelativeChanged?.Invoke(this, new RelativeChangedEventArgs<double>(oldActualPixels, newActualPixels));
    }
}

public readonly struct RelativeScale(double scale) {
    public readonly double Scale = scale;
    public double Value => Scale * 100;
    public Units Unit => Units.Percent;

    public static RelativeScale Parse(string scale) {
        scale = scale.Trim();
        if (scale[^1] != '%')
            throw new FormatException("Relative scale must ends with '%'.");
        return new RelativeScale(Convert.ToDouble(scale[..^1]) / 100);
    }

    public override string ToString() { return $"{Value}%"; }

    public static implicit operator double(RelativeScale scale) { return scale.Scale; }

    public static implicit operator RelativeScale(double scale) { return new RelativeScale(scale); }
}

public sealed class RelativeConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        RelativeScale para = parameter switch {
            RelativeScale rs => rs,
            double d         => new RelativeScale(d),
            string s         => RelativeScale.Parse(s),
            _                => parameter as double? ?? throw new ArgumentException("Parameter must have a valid value")
        };

        return value switch {
            RelativeLengthBase relativeLength => relativeLength.Absolute() * para.Scale,
            RelativeThickness thickness       => thickness * para.Scale,
            RelativeCornerRadius cornerRadius => cornerRadius * para.Scale,
            RelativeSize size                 => size * para.Scale,
            double length                     => length * para.Scale,
            Thickness thickness               => thickness * para.Scale,
            CornerRadius cornerRadius         => ((RelativeCornerRadius)cornerRadius * para.Scale).Absolute(),
            Size size                         => size * para.Scale,
            string s                          => System.Convert.ToDouble(s) * para.Scale,
            null                              => null,
            _ => (value as double?) * para.Scale ??
                 throw new InvalidCastException("This type is not supported. Please create a custom converter.")
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        // ReSharper disable once InvertIf
        if (parameter is not RelativeScale para) {
            if (parameter is not double val)
                return null;
            para = new RelativeScale(val);
        }

        return value switch {
            RelativeLengthBase relativeLength => relativeLength.Absolute() / para.Scale,
            RelativeThickness thickness       => thickness / para.Scale,
            RelativeCornerRadius cornerRadius => cornerRadius / para.Scale,
            RelativeSize size                 => size / para.Scale,
            Thickness thickness               => ((RelativeThickness)thickness / para.Scale).Absolute(),
            CornerRadius cornerRadius         => ((RelativeCornerRadius)cornerRadius / para.Scale).Absolute(),
            Size size                         => size / para.Scale,
            double length                     => length / para.Scale,
            string s                          => System.Convert.ToDouble(s) / para.Scale,
            _                                 => (value as double?) / para.Scale
        };
    }
}

public readonly struct RelativeCalc(double value, Units unit = Units.Pixel, Visual? target = null) {
    public readonly double Value = value;
    public readonly Units Unit = unit;
    public readonly Visual? Target = target;

    public RelativeCalc(double value, string unit, Visual? target = null) : this(
        value,
        Converters.StringToUnit(unit),
        target) { }


    public double Calc() {
        if (Unit.IsAbsolute())
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return Unit switch {
                Units.Pixel      => Value,
                Units.Centimeter => 96 / 2.54 * Value,
                Units.Millimeter => 96 / 2.54 * Value / 1000,
                Units.Inch       => 96 * Value,
                _                => throw new ArgumentOutOfRangeException(nameof(unit))
            };

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        double? result = Unit switch {
            Units.TemplatedParentWidth  => GetWidth(Target?.TemplatedParent as Visual) * Value / 100d,
            Units.TemplatedParentHeight => GetHeight(Target?.TemplatedParent as Visual) * Value / 100d,
            Units.LogicalParentWidth    => GetWidth(Target?.Parent as Visual) * Value / 100d,
            Units.LogicalParentHeight   => GetHeight(Target?.Parent as Visual) * Value / 100d,
            Units.VisualParentWidth     => GetWidth(Target?.GetVisualParent()) * Value / 100d,
            Units.VisualParentHeight    => GetHeight(Target?.GetVisualParent()) * Value / 100d,
            Units.SelfWidth             => GetWidth(Target) * Value / 100d,
            Units.SelfHeight            => GetHeight(Target) * Value / 100d,
            Units.FontSize              => Target?.GetValue(TextElement.FontSizeProperty) * Value, // not percent
            Units.ViewPortWidth         => TopLevel.GetTopLevel(Target)?.ClientSize.Width * Value / 100d,
            Units.ViewPortHeight        => TopLevel.GetTopLevel(Target)?.ClientSize.Height * Value / 100d,
            _                           => throw new ArgumentOutOfRangeException($"{Unit} is not implemented by now.")
        };
        return result ?? throw new NullReferenceException("Relative source returns a null value.");
    }

    private static double GetWidth(Visual? source) {
        if (source is null)
            throw new NullReferenceException("The relative source does not exist.");
        double width = source.GetValue(Layoutable.WidthProperty);
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (width is not double.NaN and not double.PositiveInfinity and not double.NegativeInfinity)
            return width;
        return source.Bounds.Width;
    }

    private static double GetHeight(Visual? source) {
        if (source is null)
            throw new NullReferenceException("The relative source does not exist.");
        double height = source.GetValue(Layoutable.HeightProperty);
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (height is not double.NaN and not double.PositiveInfinity and not double.NegativeInfinity)
            return height;
        return source.Bounds.Height;
    }

    public static double Calc(string s, Visual? visual = null) {
        double value = 0;
        s = s.Trim();
        if (s.Length == 0)
            return 0;
        int begin = 0;
        bool isPositive = s[0] != '-';
        for (int i = 1; i < s.Length; i++) {
            if (s[i] is not ('+' or '-'))
                continue;
            value += CalcSingle(s[begin..i]);
            if (!isPositive)
                value *= -1;
            isPositive = s[i] == '+';
            begin = i + 1;
        }

        value += CalcSingle(s[begin..]);
        return value;

        double CalcSingle(string length) {
            length = length.Trim();
            if (char.IsNumber(length[^1]))
                return Convert.ToDouble(length);

            int i = length.Length - 2;
            while (!(char.IsNumber(length[i]) || length[i] == '.'))
                --i;

            return new RelativeCalc(
                Convert.ToDouble(length[..(i + 1)]),
                Converters.StringToUnit(length[(i + 1)..]),
                visual).Calc();
        }
    }

    public static double Calc(double value, Units unit = Units.Pixel, Visual? target = null) {
        if (unit.IsAbsolute())
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return unit switch {
                Units.Pixel      => value,
                Units.Centimeter => 96 / 2.54 * value,
                Units.Millimeter => 96 / 2.54 * value / 1000,
                Units.Inch       => 96 * value,
                _                => throw new ArgumentOutOfRangeException(nameof(unit))
            };

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        double? result = unit switch {
            Units.TemplatedParentWidth  => GetWidth(target?.TemplatedParent as Visual) * value / 100d,
            Units.TemplatedParentHeight => GetHeight(target?.TemplatedParent as Visual) * value / 100d,
            Units.LogicalParentWidth    => GetWidth(target?.Parent as Visual) * value / 100d,
            Units.LogicalParentHeight   => GetHeight(target?.Parent as Visual) * value / 100d,
            Units.VisualParentWidth     => GetWidth(target?.GetVisualParent()) * value / 100d,
            Units.VisualParentHeight    => GetHeight(target?.GetVisualParent()) * value / 100d,
            Units.SelfWidth             => GetWidth(target) * value / 100d,
            Units.SelfHeight            => GetHeight(target) * value / 100d,
            Units.FontSize              => target?.GetValue(TextElement.FontSizeProperty) * value, // not percent
            Units.ViewPortWidth         => TopLevel.GetTopLevel(target)?.ClientSize.Width * value / 100d,
            Units.ViewPortHeight        => TopLevel.GetTopLevel(target)?.ClientSize.Height * value / 100d,
            _                           => throw new ArgumentOutOfRangeException(nameof(unit))
        };

        return result ?? throw new NullReferenceException("Relative source returns a null value.");
    }
}