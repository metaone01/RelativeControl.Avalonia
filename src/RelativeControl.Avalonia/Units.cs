using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.VisualTree;

namespace RelativeControl.Avalonia;

public enum Units : ushort {
    // Absolute
    Pixel,      // px DEFAULT UNIT
    Centimeter, // cm
    Millimeter, // mm
    Inch,       // in

    // Relative
    TemplatedParentWidth,  // tpw
    TemplatedParentHeight, // tph
    LogicalParentWidth,    // lpw
    LogicalParentHeight,   // lph
    VisualParentWidth,     // vpw
    VisualParentHeight,    // vph
    SelfWidth,             // sw
    SelfHeight,            // sh
    FontSize,              // em
    ViewPortWidth,         // vw
    ViewPortHeight         // vh
}

public enum AbsoluteUnits : ushort {
    Pixel = Units.Pixel, Centimeter = Units.Centimeter, Millimeter = Units.Millimeter, Inch = Units.Inch
}

public enum RelativeUnits : ushort {
    TemplatedParentWidth = Units.TemplatedParentWidth,
    TemplatedParentHeight = Units.TemplatedParentHeight,
    LogicalParentWidth = Units.LogicalParentWidth,
    LogicalParentHeight = Units.LogicalParentHeight,
    VisualParentWidth = Units.VisualParentWidth,
    VisualParentHeight = Units.VisualParentHeight,
    SelfWidth = Units.SelfWidth,
    SelfHeight = Units.SelfHeight,
    FontSize = Units.FontSize,
    ViewPortWidth = Units.ViewPortWidth,
    ViewPortHeight = Units.ViewPortHeight
}

public static class Converters {
    public static string UnitToString(Units unit) {
        return unit switch {
            Units.Pixel                 => "px",
            Units.Centimeter            => "cm",
            Units.Millimeter            => "mm",
            Units.Inch                  => "in",
            Units.TemplatedParentWidth  => "tpw",
            Units.TemplatedParentHeight => "tph",
            Units.LogicalParentWidth    => "pw",
            Units.LogicalParentHeight   => "ph",
            Units.VisualParentWidth     => "vpw",
            Units.VisualParentHeight    => "vph",
            Units.SelfWidth             => "sw",
            Units.SelfHeight            => "sh",
            Units.FontSize              => "em",
            Units.ViewPortWidth         => "vw",
            Units.ViewPortHeight        => "vh",
            _                           => throw new ArgumentOutOfRangeException($"Unit {unit} not supported.")
        };
    }

    public static Units StringToUnit(string unit) {
        unit = unit.ToLowerInvariant();
        return unit switch {
            "px"          => Units.Pixel,
            "cm"          => Units.Centimeter,
            "mm"          => Units.Millimeter,
            "in"          => Units.Inch,
            "tpw"         => Units.TemplatedParentWidth,
            "tph"         => Units.TemplatedParentHeight,
            "lpw" or "pw" => Units.LogicalParentWidth,
            "lph" or "ph" => Units.LogicalParentHeight,
            "vpw"         => Units.VisualParentWidth,
            "vph"         => Units.VisualParentHeight,
            "sw"          => Units.SelfWidth,
            "sh"          => Units.SelfHeight,
            "em"          => Units.FontSize,
            "vw"          => Units.ViewPortWidth,
            "vh"          => Units.ViewPortHeight,
            _             => throw new ArgumentOutOfRangeException($"Unit {unit} not supported.")
        };
    }
}

public static class Extensions {
    public static bool IsAbsolute(this Units unit) { return Enum.IsDefined(typeof(AbsoluteUnits), (ushort)unit); }

    public static bool IsRelative(this Units unit) { return Enum.IsDefined(typeof(RelativeUnits), (ushort)unit); }

    public static bool Equals(this Units unit, AbsoluteUnits absoluteUnit) {
        return (ushort)unit == (ushort)absoluteUnit;
    }

    public static bool Equals(this Units unit, RelativeUnits relativeUnits) {
        return (ushort)unit == (ushort)relativeUnits;
    }

    public static bool Equals(this AbsoluteUnits absoluteUnit, Units unit) {
        return (ushort)absoluteUnit == (ushort)unit;
    }

    public static bool Equals(this RelativeUnits relativeUnit, Units unit) {
        return (ushort)relativeUnit == (ushort)unit;
    }

    public static bool Equals(this AbsoluteUnits absoluteUnit, RelativeUnits relativeUnits) { return false; }

    public static bool Equals(this RelativeUnits relativeUnit, AbsoluteUnits relativeUnits) { return false; }
}

public interface IRelativeLength {
    public double ActualPixels { get; }

    public static virtual bool operator <(IRelativeLength a, IRelativeLength b) {
        return a.ActualPixels < b.ActualPixels;
    }

    public static virtual bool operator >(IRelativeLength a, IRelativeLength b) {
        return a.ActualPixels > b.ActualPixels;
    }

    public static virtual bool operator <(double a, IRelativeLength b) { return a < b.ActualPixels; }

    public static virtual bool operator >(double a, IRelativeLength b) { return a > b.ActualPixels; }

    public static virtual bool operator <(IRelativeLength a, double b) { return a.ActualPixels < b; }

    public static virtual bool operator >(IRelativeLength a, double b) { return a.ActualPixels > b; }
}

public sealed class RelativeLengthMerge : IRelativeLength, IEquatable<RelativeLengthMerge> {
    private double _actualPixels;

    public RelativeLengthMerge(params List<RelativeLength> subLengths) {
        Children = subLengths;
        foreach (RelativeLength length in Children) {
            _actualPixels                  += length.ActualPixels;
            length.OnRelativeLengthChanged += Update;
        }
    }

    public RelativeLengthMerge(params List<RelativeLength>[] lengthLists) {
        foreach (List<RelativeLength> lengthList in lengthLists) {
            Children.AddRange(lengthList);
            foreach (RelativeLength length in lengthList) {
                _actualPixels                  += length.ActualPixels;
                length.OnRelativeLengthChanged += Update;
            }
        }
    }

    public RelativeLengthMerge(IEnumerable<RelativeLength> lengths) {
        foreach (RelativeLength length in lengths) {
            Children.Add(length);
            _actualPixels                  += length.ActualPixels;
            length.OnRelativeLengthChanged += Update;
        }
    }

    public List<RelativeLength> Children { get; private set; } = [];

    public double Multiplier { get; private set; } = 1;

    public bool Equals(RelativeLengthMerge? other) {
        return other is not null && Children == other.Children && Math.Abs(Multiplier - other.Multiplier) < 1e-5;
    }

    public double ActualPixels => _actualPixels * Multiplier;

    private void Update(double oldActualPixels, double newActualPixels) {
        _actualPixels -= oldActualPixels;
        _actualPixels += newActualPixels;
    }

    public void Add(RelativeLength right) { Children.Add(right); }

    public void Add(RelativeLengthMerge right) { Children.AddRange(right.Children); }

    public void Remove(RelativeLength right) { Children.Remove(right); }

    public void Remove(RelativeLengthMerge right) { Children = Children.Except(right.Children).ToList(); }

    public override bool Equals(object? obj) { return obj is RelativeLengthMerge other && Equals(other); }
    public override int GetHashCode() { return HashCode.Combine(this); }

    public static bool operator ==(RelativeLengthMerge self, RelativeLengthMerge other) { return self.Equals(other); }

    public static bool operator !=(RelativeLengthMerge self, RelativeLengthMerge other) { return !self.Equals(other); }

    public static RelativeLengthMerge operator *(RelativeLengthMerge self, double multiplier) {
        self.Multiplier *= multiplier;
        return self;
    }

    public static RelativeLengthMerge operator *(double multiplier, RelativeLengthMerge self) {
        self.Multiplier *= multiplier;
        return self;
    }

    public static RelativeLengthMerge operator /(RelativeLengthMerge self, double multiplier) {
        self.Multiplier /= multiplier;
        return self;
    }

    ~RelativeLengthMerge() {
        foreach (RelativeLength length in Children)
            length.OnRelativeLengthChanged -= Update;
    }
}

public class RelativeLength : IRelativeLength {
    public delegate void RelativeLengthChanged(double oldActualPixels, double newActualPixels);

    public static readonly RelativeLength Empty = new(double.NaN);
    public readonly Units Unit;
    private double _actualPixels = double.NaN;
    private Visual? _target;

    /// <summary>
    ///     Base class for relative values.
    /// </summary>
    /// <param name="length">A string representing relative length.</param>
    /// <param name="target">The target control.</param>
    public RelativeLength(string length, Visual? target = null) {
        length = length.Trim();
        if (char.IsNumber(length[^1])) {
            Value = Convert.ToDouble(length);
            Unit  = Units.Pixel;
            return;
        }

        int i = length.Length - 2;
        while (!(char.IsNumber(length[i]) || length[i] == '.'))
            --i;

        Value   = Convert.ToDouble(length[..(i + 1)]);
        Unit    = Converters.StringToUnit(length[(i + 1)..]);
        _target = target;
        Initialize();
    }

    /// <summary>
    ///     Base class for relative values.
    /// </summary>
    /// <param name="value">Relative value.</param>
    /// <param name="unit">Relative unit.</param>
    /// <param name="target">The target control.</param>
    public RelativeLength(double value, Units unit = Units.Pixel, Visual? target = null) {
        Value   = value;
        Unit    = unit;
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
        Value   = value;
        Unit    = Converters.StringToUnit(unit);
        _target = target;
        Initialize();
    }

    public double Value { get; private set; }
    public double ActualPixels => Absolute();
    public event RelativeLengthChanged? OnRelativeLengthChanged;

    public double Absolute() {
        if (double.IsNaN(_actualPixels) && !double.IsNaN(Value))
            Update();
        return _actualPixels;
    }

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

    /// <summary>
    ///     Set another target for this relative length.
    /// </summary>
    /// <param name="target">The new target.</param>
    public void SetTarget(Visual? target) {
        if (GetSource(_target) is { } oldSource)
            oldSource.PropertyChanged -= Update;
        _target = target;
        if (Unit.IsRelative() && GetSource(_target) is { } newSource)
            newSource.PropertyChanged += Update;
        Initialize();
    }

    private void Initialize() {
        if (_target is null)
            return;
        if (!_target.IsAttachedToVisualTree()) {
            _target.AttachedToVisualTree += UpdateOnAttachedToVisualTree;
        } else {
            if (Unit.IsRelative() && GetSource(_target) is { } source)
                source.PropertyChanged += Update;
            Update();
        }
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
            Units.TemplatedParentWidth  => source.Bounds.Width * Value,
            Units.TemplatedParentHeight => source.Bounds.Height * Value,
            Units.LogicalParentWidth    => source.Bounds.Width * Value,
            Units.LogicalParentHeight   => source.Bounds.Height * Value,
            Units.VisualParentWidth     => source.Bounds.Width * Value,
            Units.VisualParentHeight    => source.Bounds.Height * Value,
            Units.SelfWidth             => source.Bounds.Width * Value,
            Units.SelfHeight            => source.Bounds.Height * Value,
            Units.FontSize              => source.Styles.GetValue(TextElement.FontSizeProperty) * Value,
            Units.ViewPortWidth         => (source as TopLevel)!.ClientSize.Width * Value,
            Units.ViewPortHeight        => (source as TopLevel)!.ClientSize.Height * Value,
            _                           => throw new ArgumentOutOfRangeException($"{Unit} is not implemented by now.")
        };
        OnRelativeLengthChanged?.Invoke(oldActualPixels, _actualPixels);
    }

    public static RelativeLengthMerge Merge(RelativeLength left, RelativeLength right) {
        if (left.Unit.Equals(right.Unit))
            return left._target == right._target ?
                new RelativeLengthMerge(new RelativeLength(left.Value + right.Value, left.Unit, left._target)) :
                new RelativeLengthMerge(left, right);

        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLengthMerge(new RelativeLength(left._actualPixels + right._actualPixels));
        return new RelativeLengthMerge(left, right);
    }

    public static RelativeLength Min(RelativeLength left, RelativeLength right) {
        if (left._actualPixels is double.NaN)
            return right;
        if (right._actualPixels is double.NaN)
            return left;
        return left._actualPixels < right._actualPixels ? left : right;
    }

    public static RelativeLength Max(RelativeLength left, RelativeLength right) {
        if (left._actualPixels is double.NaN)
            return right;
        if (right._actualPixels is double.NaN)
            return left;
        return left._actualPixels > right._actualPixels ? left : right;
    }

    public bool Equals(RelativeLength? other) {
        return other is not null && Math.Abs(Value - other.Value) < 0.001 && Unit == other.Unit;
    }

    public override string ToString() { return $"{Value}{Converters.UnitToString(Unit)}"; }

    ~RelativeLength() {
        if (GetSource(_target) is { } source)
            source.PropertyChanged -= Update;
    }

    public static RelativeLength operator +(RelativeLength left, RelativeLength right) {
        if (left.Unit.Equals(right.Unit)) {
            if (left._target == right._target)
                return new RelativeLength(left.Value + right.Value, left.Unit, left._target);
            throw new InvalidOperationException("Cannot add two relative lengths with different targets.");
        }

        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLength(left._actualPixels + right._actualPixels);

        throw new InvalidOperationException("Cannot add relative lengths with different relative Units");
    }

    public static RelativeLength operator -(RelativeLength left, RelativeLength right) {
        if (left.Unit.Equals(right.Unit)) {
            if (left._target == right._target)
                return new RelativeLength(left.Value - right.Value, left.Unit, left._target);
            throw new InvalidOperationException("Cannot subtract two relative lengths with different targets.");
        }

        if (left.Unit.IsAbsolute() && right.Unit.IsAbsolute())
            return new RelativeLength(left._actualPixels - right._actualPixels);

        throw new InvalidOperationException("Cannot subtract relative lengths with different targets.");
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

    public static implicit operator string(RelativeLength value) { return value.ToString(); }
}