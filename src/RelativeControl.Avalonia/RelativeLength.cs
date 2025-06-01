using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data.Converters;
using Avalonia.VisualTree;

namespace RelativeControl.Avalonia;

public interface IRelativeLength {
    delegate void RelativeLengthChangedHandler(double oldActualPixels, double newActualPixels);

    double ActualPixels { get; }
    double Absolute();

    event RelativeLengthChangedHandler? RelativeLengthChanged;
}

public abstract class RelativeLengthBase : IRelativeLength {
    public abstract double ActualPixels { get; }
    public abstract double Absolute();

    public abstract event IRelativeLength.RelativeLengthChangedHandler? RelativeLengthChanged;

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


    public static RelativeLengthBase operator +(RelativeLengthBase left, RelativeLengthBase right) {
        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLengthBase operator +(RelativeLength left, RelativeLengthBase right) {
        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLengthBase operator +(RelativeLengthBase left, RelativeLength right) {
        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLengthBase operator -(RelativeLengthBase left, RelativeLengthBase right) {
        return new RelativeLengthSub(left, right);
    }

    public static RelativeLengthBase operator -(RelativeLength left, RelativeLengthBase right) {
        return new RelativeLengthSub(left, right);
    }

    public static RelativeLengthBase operator -(RelativeLengthBase left, RelativeLength right) {
        return new RelativeLengthSub(left, right);
    }

    public static RelativeLengthBase operator *(RelativeLengthBase left, double right) {
        switch (left) {
            case SingleRelativeLength slr:
                slr *= right;
                return slr;
            case RelativeLengthCollection rlc:
                rlc *= right;
                return rlc;
            default:
                throw new NotSupportedException();
        }
    }

    public static RelativeLengthBase operator *(double left, RelativeLengthBase right) {
        switch (right) {
            case SingleRelativeLength slr:
                slr *= left;
                return slr;
            case RelativeLengthCollection rlc:
                rlc *= left;
                return rlc;
            default:
                throw new NotSupportedException();
        }
    }

    public static RelativeLengthBase operator /(RelativeLengthBase left, double right) {
        switch (left) {
            case SingleRelativeLength slr:
                slr *= right;
                return slr;
            case RelativeLengthCollection rlc:
                rlc *= right;
                return rlc;
            default:
                throw new NotSupportedException();
        }
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

public abstract class SingleRelativeLength : RelativeLengthBase {
    public abstract double Value { [Pure] get; protected set; }
    public abstract Units Unit { [Pure] get; }
    public override double ActualPixels => Absolute();
    [Pure] protected abstract Visual? GetTarget();
    public abstract override double Absolute();
    protected abstract void InvokeIfChanged(double oldActualPixels, double newActualPixels);
    public abstract override event IRelativeLength.RelativeLengthChangedHandler? RelativeLengthChanged;

    [Pure]
    public static SingleRelativeLength Min(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.ActualPixels is double.NaN)
            return right;
        if (right.ActualPixels is double.NaN)
            return left;
        return left.ActualPixels < right.ActualPixels ? left : right;
    }

    [Pure]
    public static RelativeLengthBase Max(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.ActualPixels is double.NaN)
            return right;
        if (right.ActualPixels is double.NaN)
            return left;
        return left.ActualPixels > right.ActualPixels ? left : right;
    }

    [Pure] public override string ToString() { return $"{Value}{Converters.UnitToString(Unit)}"; }

    public static RelativeLengthBase operator +(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLength(left.ActualPixels + right.ActualPixels);
        if (left.Unit.Equals(right.Unit) && left.GetTarget() == right.GetTarget())
            return new RelativeLength(left.Value + right.Value, left.Unit, left.GetTarget());
        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLengthBase operator -(SingleRelativeLength left, SingleRelativeLength right) {
        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLength(left.ActualPixels - right.ActualPixels);
        if (left.Unit.Equals(right.Unit) && left.GetTarget() == right.GetTarget())
            return new RelativeLength(left.Value - right.Value, left.Unit, left.GetTarget());
        return new RelativeLengthSub(left, right);
    }

    public static SingleRelativeLength operator *(SingleRelativeLength relativeLength, double scaler) {
        relativeLength.Value *= scaler;
        return relativeLength;
    }

    public static SingleRelativeLength operator *(double scaler, SingleRelativeLength relativeLength) {
        relativeLength.Value *= scaler;
        return relativeLength;
    }

    public static SingleRelativeLength operator /(SingleRelativeLength relativeLength, double scaler) {
        relativeLength.Value /= scaler;
        return relativeLength;
    }
}

public abstract class RelativeLengthCollection : RelativeLengthBase {
    public abstract List<RelativeLengthBase> Children { [Pure] get; }
    public abstract double Scaler { [Pure] get; protected set; }
    [Pure] public override double ActualPixels => Absolute();
    [Pure] public abstract override double Absolute();
    public abstract override event IRelativeLength.RelativeLengthChangedHandler? RelativeLengthChanged;
    public abstract void Add(params RelativeLengthBase[] elements);
    public abstract void Remove(params RelativeLengthBase[] elements);

    protected abstract void InvokeIfChanged(double oldActualPixels, double newActualPixels);

    public static RelativeLengthMerge operator +(RelativeLengthCollection left, RelativeLengthBase right) {
        // ReSharper disable once InvertIf
        if (left is RelativeLengthMerge rlm) {
            rlm.Add(right);
            return rlm;
        }

        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLengthSub operator -(RelativeLengthCollection left, RelativeLengthBase right) {
        // ReSharper disable once InvertIf
        if (left is RelativeLengthSub rls) {
            rls.Add(right);
            return rls;
        }

        return new RelativeLengthSub(left, right);
    }

    public static RelativeLengthCollection operator *(RelativeLengthCollection self, double scaler) {
        double old = self.ActualPixels;
        self.Scaler *= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    public static RelativeLengthCollection operator *(double scaler, RelativeLengthCollection self) {
        double old = self.ActualPixels;
        self.Scaler *= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    public static RelativeLengthCollection operator /(RelativeLengthCollection self, double scaler) {
        double old = self.ActualPixels;
        self.Scaler /= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }
}

public sealed class RelativeLengthMerge : RelativeLengthCollection {
    public static readonly RelativeLengthMerge Empty = new();
    private double _actualPixels;

    public RelativeLengthMerge() { }

    public RelativeLengthMerge(params RelativeLengthBase[] lengths) { Add(lengths); }

    public RelativeLengthMerge(params ICollection<RelativeLengthBase>[] lengthsArray) {
        foreach (ICollection<RelativeLengthBase> lengthCollection in lengthsArray) {
            Children.AddRange(lengthCollection);
            foreach (RelativeLengthBase length in lengthCollection) {
                _actualPixels += length.ActualPixels;
                length.RelativeLengthChanged += Update;
            }
        }

        InvokeIfChanged(0, ActualPixels);
    }

    public RelativeLengthMerge(params IEnumerable<RelativeLengthBase>[] lengthsArray) {
        foreach (IEnumerable<RelativeLengthBase> lengths in lengthsArray)
        foreach (RelativeLengthBase length in lengths) {
            Children.Add(length);
            _actualPixels += length.ActualPixels;
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(0, ActualPixels);
    }

    public RelativeLengthMerge(ICollection<RelativeLengthBase> lengths) { AddRange(lengths); }

    public RelativeLengthMerge(IEnumerable<RelativeLengthBase> lengths) { AddRange(lengths); }
    public override List<RelativeLengthBase> Children { [Pure] get; } = [];
    public override double Scaler { [Pure] get; protected set; } = 1D;
    [Pure] public override double ActualPixels => _actualPixels * Scaler;

    [Pure] public override double Absolute() { return ActualPixels; }

    public override event IRelativeLength.RelativeLengthChangedHandler? RelativeLengthChanged;

    protected override void InvokeIfChanged(double oldActualPixels, double newActualPixels) {
        if (Math.Abs(oldActualPixels - newActualPixels) > 1e-5)
            RelativeLengthChanged?.Invoke(oldActualPixels, newActualPixels);
    }

    private void Update(double oldActualPixels, double newActualPixels) {
        double old = ActualPixels;
        _actualPixels -= oldActualPixels;
        _actualPixels += newActualPixels;
        InvokeIfChanged(old, ActualPixels);
    }

    public override void Add(params RelativeLengthBase[] lengths) {
        double old = ActualPixels;
        foreach (RelativeLengthBase length in lengths) {
            _actualPixels += length.ActualPixels;
            Children.Add(length);
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public void AddRange(ICollection<RelativeLengthBase> lengths) {
        double old = ActualPixels;
        Children.AddRange(lengths);
        foreach (RelativeLengthBase length in lengths) {
            _actualPixels += length.ActualPixels;
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public void AddRange(IEnumerable<RelativeLengthBase> lengths) {
        double old = ActualPixels;
        foreach (RelativeLengthBase length in lengths) {
            Children.Add(length);
            _actualPixels += length.ActualPixels;
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public override void Remove(params RelativeLengthBase[] removes) {
        double old = ActualPixels;
        foreach (RelativeLengthBase length in removes) {
            if (Children.Remove(length))
                _actualPixels -= length.ActualPixels;
            length.RelativeLengthChanged -= Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public static RelativeLengthMerge operator +(RelativeLengthMerge self, RelativeLengthBase right) {
        self.Add(right);
        return self;
    }

    public static RelativeLengthMerge operator *(RelativeLengthMerge self, double scaler) {
        double old = self.ActualPixels;
        self.Scaler *= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    public static RelativeLengthMerge operator *(double scaler, RelativeLengthMerge self) {
        double old = self.ActualPixels;
        self.Scaler *= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    public static RelativeLengthMerge operator /(RelativeLengthMerge self, double scaler) {
        double old = self.ActualPixels;
        self.Scaler /= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    ~RelativeLengthMerge() {
        foreach (RelativeLengthBase length in Children)
            length.RelativeLengthChanged -= Update;
    }
}

public sealed class RelativeLengthSub : RelativeLengthCollection {
    public static readonly RelativeLengthSub Empty = new();

    private double _actualPixels;

    public RelativeLengthSub() { EnsureMinuend(); }

    public RelativeLengthSub(RelativeLengthBase minuend, params RelativeLengthBase[] subtrahends) {
        EnsureMinuend(minuend);
        Add(subtrahends);
    }

    public RelativeLengthSub(RelativeLengthBase minuend, params ICollection<RelativeLengthBase>[] subtrahendsArray) {
        EnsureMinuend(minuend);
        foreach (ICollection<RelativeLengthBase> lengthCollection in subtrahendsArray) {
            Children.AddRange(lengthCollection);
            foreach (RelativeLengthBase length in lengthCollection) {
                _actualPixels -= length.ActualPixels;
                length.RelativeLengthChanged += Update;
            }
        }

        InvokeIfChanged(0, ActualPixels);
    }

    public RelativeLengthSub(RelativeLengthBase minuend, params IEnumerable<RelativeLengthBase>[] subtrahendsArray) {
        EnsureMinuend(minuend);
        foreach (IEnumerable<RelativeLengthBase> lengths in subtrahendsArray)
        foreach (RelativeLengthBase length in lengths) {
            Children.Add(length);
            _actualPixels += length.ActualPixels;
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(0, ActualPixels);
    }

    public RelativeLengthSub(RelativeLengthBase minuend, ICollection<RelativeLengthBase> subtrahends) {
        EnsureMinuend(minuend);
        AddRange(subtrahends);
    }

    public RelativeLengthSub(RelativeLengthBase minuend, IEnumerable<RelativeLengthBase> subtrahends) {
        EnsureMinuend(minuend);
        AddRange(subtrahends);
    }

    /// <summary>
    ///     The first element of Children is minuend, and others are subtrahends.
    /// </summary>
    public override List<RelativeLengthBase> Children { [Pure] get; } = [];

    public override double Scaler { [Pure] get; protected set; } = 1D;
    [Pure] public override double ActualPixels => _actualPixels * Scaler;

    [Pure] public override double Absolute() { return ActualPixels; }

    public override event IRelativeLength.RelativeLengthChangedHandler? RelativeLengthChanged;

    protected override void InvokeIfChanged(double oldActualPixels, double newActualPixels) {
        if (Math.Abs(oldActualPixels - newActualPixels) > 1e-5)
            RelativeLengthChanged?.Invoke(oldActualPixels, newActualPixels);
    }

    private void Update(double oldActualPixels, double newActualPixels) {
        double old = ActualPixels;
        _actualPixels -= oldActualPixels;
        _actualPixels += newActualPixels;
        InvokeIfChanged(old, ActualPixels);
    }

    private void EnsureMinuend(RelativeLengthBase? minuend = null) {
        double old = ActualPixels;
        if (Children.Count != 0)
            return;
        if (minuend is not null)
            minuend.RelativeLengthChanged += Update;
        else
            minuend = RelativeLength.Empty;
        Debug.Assert(_actualPixels == 0);
        Children.Add(minuend);
        InvokeIfChanged(old, ActualPixels);
    }

    public override void Add(params RelativeLengthBase[] lengths) {
        double old = ActualPixels;
        EnsureMinuend();
        foreach (RelativeLengthBase length in lengths) {
            _actualPixels -= length.ActualPixels;
            Children.Add(length);
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public void AddRange(ICollection<RelativeLengthBase> lengths) {
        double old = ActualPixels;
        EnsureMinuend();
        Children.AddRange(lengths);
        foreach (RelativeLengthBase length in lengths) {
            _actualPixels += length.ActualPixels;
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public void AddRange(IEnumerable<RelativeLengthBase> lengths) {
        double old = ActualPixels;
        EnsureMinuend();
        foreach (RelativeLengthBase length in lengths) {
            Children.Add(length);
            _actualPixels -= length.ActualPixels;
            length.RelativeLengthChanged += Update;
        }

        InvokeIfChanged(old, ActualPixels);
    }

    public override void Remove(params RelativeLengthBase[] removes) {
        double old = ActualPixels;
        foreach (RelativeLengthBase length in removes) {
            if (Children.Remove(length))
                _actualPixels += length.ActualPixels;
            length.RelativeLengthChanged -= Update;
        }

        EnsureMinuend();
        InvokeIfChanged(old, ActualPixels);
    }

    public static RelativeLengthSub operator -(RelativeLengthSub self, RelativeLengthBase subtrahend) {
        self.Add(subtrahend);
        return self;
    }

    public static RelativeLengthSub operator *(RelativeLengthSub self, double scaler) {
        double old = self.ActualPixels;
        self.Scaler *= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    public static RelativeLengthSub operator *(double scaler, RelativeLengthSub self) {
        double old = self.ActualPixels;
        self.Scaler *= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    public static RelativeLengthSub operator /(RelativeLengthSub self, double scaler) {
        double old = self.ActualPixels;
        self.Scaler /= scaler;
        self.InvokeIfChanged(old, self.ActualPixels);
        return self;
    }

    ~RelativeLengthSub() {
        foreach (RelativeLengthBase length in Children)
            length.RelativeLengthChanged -= Update;
    }
}

public sealed class RelativeLength : SingleRelativeLength {
    public static readonly RelativeLength Empty = new(0D);
    public static readonly RelativeLength PositiveInfinity = new(double.PositiveInfinity);
    public static readonly RelativeLength NegativeInfinity = new(double.NegativeInfinity);

    private readonly Visual? _target;

    private double _actualPixels;

    /// <summary>
    ///     Base class for relative values.
    /// </summary>
    /// <param name="value">Relative value.</param>
    /// <param name="unit">Relative unit.</param>
    /// <param name="target">The target control.</param>
    public RelativeLength(double value, Units unit = Units.Pixel, Visual? target = null) {
        Value = value;
        Unit = unit;
        _target = target;
        Initialize();
    }

    /// <summary>
    ///     Base class for relative values.
    /// </summary>
    /// <param name="value">The relative value.</param>
    /// <param name="unit">The relative unit.</param>
    /// <param name="target">The target control.</param>
    public RelativeLength(double value, string unit, Visual? target = null) {
        Value = value;
        Unit = Converters.StringToUnit(unit);
        _target = target;
        Initialize();
    }

    public override double Value { [Pure] get; protected set; }
    public override Units Unit { [Pure] get; }
    [Pure] public override double ActualPixels => Absolute();


    public override event IRelativeLength.RelativeLengthChangedHandler? RelativeLengthChanged;

    protected override void InvokeIfChanged(double oldActualPixels, double newActualPixels) {
        if (Math.Abs(oldActualPixels - newActualPixels) > 1e-5)
            RelativeLengthChanged?.Invoke(oldActualPixels, newActualPixels);
    }

    public override double Absolute() {
        if (double.IsNaN(_actualPixels) && !double.IsNaN(Value))
            Update();
        return _actualPixels;
    }

    [Pure] protected override Visual? GetTarget() { return _target; }

    [Pure]
    private Visual? GetSource(Visual? visual) {
        return Unit switch {
            Units.TemplatedParentWidth  => visual?.TemplatedParent as Visual,
            Units.TemplatedParentHeight => visual?.TemplatedParent as Visual,
            Units.LogicalParentWidth    => visual?.Parent as Visual,
            Units.LogicalParentHeight   => visual?.Parent as Visual,
            Units.VisualParentWidth     => visual?.GetVisualParent(),
            Units.VisualParentHeight    => visual?.GetVisualParent(),
            Units.SelfWidth             => visual,
            Units.SelfHeight            => visual,
            Units.FontSize              => visual,
            Units.ViewPortWidth         => TopLevel.GetTopLevel(visual),
            Units.ViewPortHeight        => TopLevel.GetTopLevel(visual),
            _                           => null
        };
    }

    private void Initialize() {
        if (_target is null)
            return;
        if (!_target.IsAttachedToVisualTree()) {
            _target.AttachedToVisualTree += UpdateOnAttachedToVisualTree;
        } else {
            if (Unit.IsRelative() && GetSource(_target) is { } source)
                source.PropertyChanged += Update;
        }

        Update();
    }

    private void UpdateOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e) {
        if (Unit.IsRelative() && GetSource(_target) is { } source)
            source.PropertyChanged += Update;
        Update();
        _target!.AttachedToVisualTree -= UpdateOnAttachedToVisualTree;
    }

    public void Update(object? sender = null, AvaloniaPropertyChangedEventArgs? e = null) {
        if (GetSource(_target) is not { } source)
            return;
        double oldActualPixels = _actualPixels;
        _actualPixels = Unit switch {
            Units.Pixel                 => Value,
            Units.Centimeter            => 96 / 2.54 * Value,
            Units.Millimeter            => 96 / 2.54 * Value / 1000,
            Units.Inch                  => 96 * Value,
            Units.TemplatedParentWidth  => source.Bounds.Width * Value / 100d,
            Units.TemplatedParentHeight => source.Bounds.Height * Value / 100d,
            Units.LogicalParentWidth    => source.Bounds.Width * Value / 100d,
            Units.LogicalParentHeight   => source.Bounds.Height * Value / 100d,
            Units.VisualParentWidth     => source.Bounds.Width * Value / 100d,
            Units.VisualParentHeight    => source.Bounds.Height * Value / 100d,
            Units.SelfWidth             => source.Bounds.Width * Value / 100d,
            Units.SelfHeight            => source.Bounds.Height * Value / 100d,
            Units.FontSize              => source.GetValue(TextElement.FontSizeProperty) * Value, // not percent
            Units.ViewPortWidth         => (source as TopLevel)!.ClientSize.Width * Value / 100d,
            Units.ViewPortHeight        => (source as TopLevel)!.ClientSize.Height * Value / 100d,
            _                           => throw new ArgumentOutOfRangeException($"{Unit} is not implemented by now.")
        };
        InvokeIfChanged(oldActualPixels, _actualPixels);
    }

    [Pure]
    public bool Equals(RelativeLength? other) {
        return other is not null && Math.Abs(Value - other.Value) < 0.001 && Unit == other.Unit;
    }


    /// <summary>
    ///     Parse a string to relative length.
    /// </summary>
    /// <param name="length">A string representing relative length.</param>
    /// <param name="target">The target control.</param>
    public static RelativeLength Parse(string length, Visual? target = null) {
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

    public static RelativeLength operator *(RelativeLength relativeLength, double scaler) {
        relativeLength.Value *= scaler;
        return relativeLength;
    }

    public static RelativeLength operator *(double scaler, RelativeLength relativeLength) {
        relativeLength.Value *= scaler;
        return relativeLength;
    }

    public static RelativeLength operator /(RelativeLength relativeLength, double scaler) {
        relativeLength.Value /= scaler;
        return relativeLength;
    }

    public static implicit operator RelativeLength(double value) { return new RelativeLength(value); }

    ~RelativeLength() {
        RelativeLengthChanged = null;
        if (GetSource(_target) is { } source)
            source.PropertyChanged -= Update;
    }
}

public readonly struct RelativeScaler(double scaler) {
    public readonly double Scaler = scaler;
    public double Value => Scaler * 100;
    public Units Unit => Units.Percent;

    public static RelativeScaler Parse(string scaler) {
        scaler = scaler.Trim();
        if (scaler[^1] != '%')
            throw new FormatException("Relative scaler must ends with '%'.");
        return new RelativeScaler(Convert.ToDouble(scaler[..^1]) / 100);
    }

    public override string ToString() { return $"{Value}%"; }

    public static implicit operator double(RelativeScaler scaler) { return scaler.Scaler; }

    public static implicit operator RelativeScaler(double scaler) { return new RelativeScaler(scaler); }
}

public sealed class RelativeConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        // ReSharper disable once InvertIf
        RelativeScaler para = parameter switch {
            RelativeScaler rs => rs,
            double d          => new RelativeScaler(d),
            string s          => RelativeScaler.Parse(s),
            _                 => throw new ArgumentException("Parameter must have a valid value")
        };

        return value switch {
            RelativeLengthBase relativeLength => relativeLength.Absolute() * para.Scaler,
            double length                     => length * para.Scaler,
            _                                 => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        // ReSharper disable once InvertIf
        if (parameter is not RelativeScaler para) {
            if (parameter is not double val)
                return null;
            para = new RelativeScaler(val);
        }

        return value switch {
            RelativeLengthBase relativeLength => relativeLength.Absolute() / para.Scaler,
            double length                     => length / para.Scaler,
            _                                 => null
        };
    }
}