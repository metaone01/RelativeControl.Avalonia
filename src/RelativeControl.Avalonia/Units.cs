using System;

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
    ViewPortHeight,        // vh
    Percent                // %
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
    ViewPortHeight = Units.ViewPortHeight,
    Percent = Units.Percent
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
            Units.Percent               => "%",
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
            "%"           => Units.Percent,
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