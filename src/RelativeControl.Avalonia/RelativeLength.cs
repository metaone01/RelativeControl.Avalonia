using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
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

[TypeConverter(typeof(RelativeTypeConverter))]
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
    public abstract void SetVisualAnchor(Visual? anchor);

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

    [Pure]
    public static RelativeLengthBase Parse(string s, AvaloniaObject? target, Visual? visualAnchor = null) {
        RelativeLengthBase[] lengths = ParseSingle(s, target,visualAnchor).ToArray<RelativeLengthBase>();
        return lengths.Length switch {
            0 => RelativeLength.Empty,
            1 => lengths[0],
            _ => new RelativeLengthMerge(lengths)
        };
    }

    [Pure]
    public static RelativeLengthBase[] Parse(string[] s, AvaloniaObject? target) {
        return [
            ..s.Select(single => ParseSingle(single, target).ToArray<RelativeLengthBase>())
               .Select(lengths => lengths.Length switch {
                   0 => RelativeLength.Empty,
                   1 => lengths[0],
                   _ => new RelativeLengthMerge(lengths)
               })
        ];
    }

    [Pure]
    private static IEnumerable<RelativeLength> ParseSingle(
        string s,
        AvaloniaObject? target = null,
        Visual? visualAnchor = null) {
        s = s.Trim();
        if (s.Length == 0)
            yield break;
        int begin = 0;
        bool isPositive = s[0] != '-';
        for (int i = 1; i < s.Length; i++) {
            if (s[i] is not ('+' or '-'))
                continue;
            RelativeLength length = RelativeLength.Parse(s[begin..i], target, visualAnchor);
            if (!isPositive)
                length.Multiply(-1);
            yield return length;
            isPositive = s[i] == '+';
            begin = i + 1;
        }

        yield return RelativeLength.Parse(s[begin..], target, visualAnchor);
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

public abstract class SingleRelativeLength(AvaloniaObject? target, Visual? visualAnchor = null, Visual? source = null)
    : RelativeLengthBase, IMulDiv<RelativeScale> {
    protected readonly WeakReference<AvaloniaObject>? _target =
        target is null ? null : new WeakReference<AvaloniaObject>(target);

    protected readonly WeakReference<Visual?> _visualAnchor =
        new(visualAnchor ?? (target as Visual));

    protected WeakReference<Visual>? _source = source is null ? null : new WeakReference<Visual>(source);

    public virtual double Value {
        [Pure]
        get;
        protected set;
    }

    public virtual Units Unit {
        [Pure]
        get;
        protected init;
    }

    [Pure]
    public virtual AvaloniaObject? Target {
        get {
            if (_target is null)
                return null;
            _target.TryGetTarget(out AvaloniaObject? target);
            return target;
        }
    }

    [Pure]
    public virtual Visual? Source {
        get {
            if (_source is null)
                return null;
            _source.TryGetTarget(out Visual? source);
            return source;
        }
    }

    [Pure]
    public virtual Visual? VisualAnchor {
        get {
            _visualAnchor.TryGetTarget(out Visual? visualAnchor);
            return visualAnchor;
        }
    }


    public virtual void Multiply(RelativeScale other) { Value *= other.Scale; }

    public virtual void Divide(RelativeScale other) { Value /= other.Scale; }

    public abstract override SingleRelativeLength Copy();
    public abstract override LightSingleRelativeLength LightCopy();

    public override void SetVisualAnchor(Visual? anchor) { _visualAnchor.SetTarget(anchor); }

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

    [Pure]
    public override string ToString() { return $"{Value}{Converters.UnitToString(Unit)}"; }

    [Pure]
    public static RelativeLengthBase operator +(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.Unit.Equals(right.Unit) && ReferenceEquals(left.Target, right.Target))
            return new RelativeLength(left.Value + right.Value, left.Unit, left.Target);
        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLength(left.ActualPixels + right.ActualPixels);
        return new RelativeLengthMerge(left, right);
    }

    [Pure]
    public static RelativeLengthBase operator -(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.Unit.Equals(right.Unit) && ReferenceEquals(left.Target, right.Target))
            return new RelativeLength(left.Value - right.Value, left.Unit, left.Target);
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
    public virtual List<RelativeLengthBase> Children {
        [Pure]
        get;
    } = [];

    public virtual double Scale {
        [Pure]
        get;
        protected set;
    } = 1D;

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


    public override void SetVisualAnchor(Visual? anchor) {
        foreach (RelativeLengthBase relative in Children)
            relative.SetVisualAnchor(anchor);
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

    [Pure]
    public override double ActualPixels => _actualPixels * Scale;

    public override RelativeLengthMerge Copy() { return new RelativeLengthMerge(Children); }

    [Pure]
    public override RelativeLengthMerge LightCopy() { return new RelativeLengthMerge(this); }

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

    [Pure]
    public static RelativeLengthMerge Parse(string s, AvaloniaObject? visual = null) {
        return new RelativeLengthMerge(ParseSingle(s, visual));
    }

    private static IEnumerable<RelativeLength> ParseSingle(string s, AvaloniaObject? visual = null) {
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
    public static readonly RelativeLength Empty = new(-1D);
    public static readonly RelativeLength PositiveInfinity = new(double.PositiveInfinity);
    public static readonly RelativeLength NegativeInfinity = new(double.NegativeInfinity);

    private double _actualPixels;


    /// <summary>
    ///     A relative length to calculate the target's property with specified unit.
    /// </summary>
    /// <param name="value">Relative value.</param>
    /// <param name="unit">Relative unit.</param>
    /// <param name="target">The target object.</param>
    /// <param name="source">The source object for relative calculation.</param>
    /// <param name="visualAnchor">The visual component that the <paramref name="target" /> is depend on.</param>
    public RelativeLength(
        double value,
        Units unit = Units.Pixel,
        AvaloniaObject? target = null,
        Visual? visualAnchor = null,
        Visual? source = null) : base(target, visualAnchor, source) {
        Value = value;
        Unit = unit;
        Initialize();
    }

    /// <inheritdoc />
    public RelativeLength(double value, string unit, AvaloniaObject? target = null) : this(
        value,
        Converters.StringToUnit(unit),
        target) { }

    [Pure]
    public override double ActualPixels => _actualPixels;

    public override event RelativeChangedEventHandler<double>? RelativeChanged;

    protected override void InvokeIfChanged(double oldActualPixels, double newActualPixels) {
        if (Math.Abs(oldActualPixels - newActualPixels) > 1e-5)
            RelativeChanged?.Invoke(this, new RelativeChangedEventArgs<double>(oldActualPixels, newActualPixels));
    }

    public override RelativeLength Copy() { return new RelativeLength(Value, Unit, Target); }

    public override LightSingleRelativeLength LightCopy() { return new LightSingleRelativeLength(this); }

    private void SetSource() {
        if (!_visualAnchor.TryGetTarget(out Visual? visual))
            return;
        if (Unit switch {
                Units.TemplatedParentWidth  => visual.TemplatedParent as Visual,
                Units.TemplatedParentHeight => visual.TemplatedParent as Visual,
                Units.LogicalParentWidth    => visual.Parent as Visual,
                Units.LogicalParentHeight   => visual.Parent as Visual,
                Units.VisualParentWidth     => visual.GetVisualParent(),
                Units.VisualParentHeight    => visual.GetVisualParent(),
                Units.SelfWidth             => visual,
                Units.SelfHeight            => visual,
                Units.FontSize              => visual,
                Units.ViewPortWidth         => TopLevel.GetTopLevel(visual),
                Units.ViewPortHeight        => TopLevel.GetTopLevel(visual),
                _                           => null
            } is { } source)
            _source = new WeakReference<Visual>(source);
        else
            throw new InvalidOperationException($"Cannot find {visual}'s relative source.");
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

        if (VisualAnchor is not { } visualAnchor)
            return;
        if (!visualAnchor.IsAttachedToVisualTree()) {
            visualAnchor.AttachedToVisualTree += UpdateOnAttachedToVisualTree;
        } else {
            SetSource();
            if (Unit.IsRelative() && Source is { } source)
                source.PropertyChanged += Update;
        }

        Update();
    }

    private void UpdateOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
        SetSource();
        if (Unit.IsRelative() && Source is { } source)
            source.PropertyChanged += Update;
        Update();
        VisualAnchor!.AttachedToVisualTree -= UpdateOnAttachedToVisualTree;
    }

    private void Update(object? sender = null, AvaloniaPropertyChangedEventArgs? args = null) {
        if (Source is not { } source)
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
        InvokeIfChanged(oldActualPixels, ActualPixels);
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
    /// <param name="visualAnchor">The anchor for evaluating relative values.</param>
    [Pure]
    public new static RelativeLength Parse(string length, AvaloniaObject? target = null, Visual? visualAnchor = null) {
        length = length.Trim();
        if (char.IsNumber(length[^1]))
            return new RelativeLength(Convert.ToDouble(length));

        int i = length.Length - 2;
        while (!(char.IsNumber(length[i]) || length[i] == '.'))
            --i;

        return new RelativeLength(
            Convert.ToDouble(length[..(i + 1)]),
            Converters.StringToUnit(length[(i + 1)..]),
            target,
            visualAnchor);
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
        if (Source is { } source)
            source.PropertyChanged -= Update;
    }
}

public sealed class LightSingleRelativeLength : SingleRelativeLength {
    public LightSingleRelativeLength(SingleRelativeLength length) : base(null) {
        Base = length;
        if (RelativeChanged is not null)
            Base.RelativeChanged += Update;
    }

    public override double Value { get; protected set; } = 100D;
    public override Units Unit => Base.Unit;
    public override double ActualPixels => Base.ActualPixels * Value / 100D;

    public SingleRelativeLength Base { get; }

    public override AvaloniaObject? Target => Base.Target;
    public override Visual? Source => Base.Source;

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

    [Pure]
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

public sealed class RelativeExpression(string expression)
    : IRelative<double>, IRelative<Thickness>, IRelative<CornerRadius>, IRelative<Size> {
    [Pure]
    public string Expression { get; } = expression;


    CornerRadius IRelative<CornerRadius>.ActualValue => ThrowHelper<CornerRadius>();

    event RelativeChangedEventHandler<CornerRadius>? IRelative<CornerRadius>.RelativeChanged {
        // ReSharper disable ValueParameterNotUsed
        add => ThrowHelper<CornerRadius>();
        remove => ThrowHelper<CornerRadius>();
        // ReSharper restore ValueParameterNotUsed
    }

    CornerRadius IRelative<CornerRadius>.Absolute() { return ((IRelative<CornerRadius>)this).ActualValue; }

    double IRelative<double>.ActualValue => ThrowHelper<double>();


    double IRelative<double>.Absolute() { return ((IRelative<double>)this).ActualValue; }

    event RelativeChangedEventHandler<double>? IRelative<double>.RelativeChanged {
        // ReSharper disable ValueParameterNotUsed
        add => ThrowHelper<double>();
        remove => ThrowHelper<double>();
        // ReSharper restore ValueParameterNotUsed
    }

    public Size ActualValue => ThrowHelper<Size>();

    public event RelativeChangedEventHandler<Size>? RelativeChanged;

    public Size Absolute() { return ((IRelative<Size>)this).ActualValue; }


    Thickness IRelative<Thickness>.ActualValue => ThrowHelper<Thickness>();

    event RelativeChangedEventHandler<Thickness>? IRelative<Thickness>.RelativeChanged {
        // ReSharper disable ValueParameterNotUsed
        add => ThrowHelper<Thickness>();
        remove => ThrowHelper<Thickness>();
        // ReSharper restore ValueParameterNotUsed
    }

    Thickness IRelative<Thickness>.Absolute() { return ((IRelative<Thickness>)this).ActualValue; }

    [DoesNotReturn]
    private static T ThrowHelper<T>() { throw new NotEvaluatedException(); }

    private sealed class NotEvaluatedException() : Exception("This expression have not been evaluated.");
}

public sealed class RelativeTypeConverter : TypeConverter {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {
        if (value is string s)
            return new RelativeExpression(s);
        return base.ConvertFrom(context, culture, value);
    }
}