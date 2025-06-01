using System;
using Avalonia;

namespace RelativeControl.Avalonia;

public class RelativeThickness : IEquatable<RelativeThickness> {
    public delegate void RelativeThicknessChangedHandler(Thickness newThickness);

    public static readonly RelativeThickness Empty = new(RelativeLength.Empty);

    /// <summary>
    ///     The relative thickness on the bottom.
    /// </summary>
    public readonly RelativeLengthBase Bottom;

    /// <summary>
    ///     The relative thickness on the left.
    /// </summary>
    public readonly RelativeLengthBase Left;

    /// <summary>
    ///     The relative thickness on the right.
    /// </summary>
    public readonly RelativeLengthBase Right;

    /// <summary>
    ///     The relative thickness on the top.
    /// </summary>
    public readonly RelativeLengthBase Top;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeThickness" /> structure.
    /// </summary>
    /// <param name="uniformLength">The length that should be applied to all sides.</param>
    public RelativeThickness(RelativeLengthBase uniformLength) {
        Left = Top = Right = Bottom = uniformLength;
        Register();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeThickness" /> structure.
    /// </summary>
    /// <param name="horizontal">The thickness on the left and right.</param>
    /// <param name="vertical">The thickness on the top and bottom.</param>
    public RelativeThickness(RelativeLengthBase horizontal, RelativeLengthBase vertical) {
        Left = Right = horizontal;
        Top = Bottom = vertical;
        Register();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeThickness" /> structure.
    /// </summary>
    /// <param name="left">The relative thickness on the left.</param>
    /// <param name="top">The relative thickness on the top.</param>
    /// <param name="right">The relative thickness on the right.</param>
    /// <param name="bottom">The relative thickness on the bottom.</param>
    public RelativeThickness(
        RelativeLengthBase left,
        RelativeLengthBase top,
        RelativeLengthBase right,
        RelativeLengthBase bottom) {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
        Register();
    }

    /// <summary>
    ///     Gets a value indicating whether all sides are equal.
    /// </summary>
    public bool IsUniform => Left == Right && Top == Bottom && Left == Top;

    /// <summary>
    ///     Returns a boolean indicating whether the relative thickness is equal to the other given point.
    /// </summary>
    /// <param name="other">The other relative thickness to test equality against.</param>
    /// <returns>True if this relative thickness is equal to other; False otherwise.</returns>
    public bool Equals(RelativeThickness? other) {
        return Left == other?.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
    }


    public event RelativeThicknessChangedHandler? RelativeThicknessChanged;

    private void Register() {
        Left.RelativeLengthChanged += (_, _) => { RelativeThicknessChanged?.Invoke(Absolute()); };
        Top.RelativeLengthChanged += (_, _) => { RelativeThicknessChanged?.Invoke(Absolute()); };
        Right.RelativeLengthChanged += (_, _) => { RelativeThicknessChanged?.Invoke(Absolute()); };
        Bottom.RelativeLengthChanged += (_, _) => { RelativeThicknessChanged?.Invoke(Absolute()); };
    }

    public Thickness Absolute() {
        return new Thickness(Left.ActualPixels, Top.ActualPixels, Right.ActualPixels, Bottom.ActualPixels);
    }


    /// <summary>
    ///     Parses a <see cref="RelativeThickness" /> string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="target">The relative target.</param>
    /// <returns>The <see cref="RelativeThickness" />.</returns>
    public static RelativeThickness Parse(string s, Visual? target = null) {
        string[] vals = s.Trim().Split(' ');
        return vals.Length switch {
            1 => new RelativeThickness(RelativeLength.Parse(vals[0], target)),
            2 => new RelativeThickness(RelativeLength.Parse(vals[0], target), RelativeLength.Parse(vals[1], target)),
            4 => new RelativeThickness(
                RelativeLength.Parse(vals[0], target),
                RelativeLength.Parse(vals[1], target),
                RelativeLength.Parse(vals[2], target),
                RelativeLength.Parse(vals[3], target)),
            _ => throw new FormatException($"Invalid relative thickness: '{s}'")
        };
    }

    /// <summary>
    ///     Deconstruct the RelativeThickness into its left, top, right and bottom relative thickness values.
    /// </summary>
    /// <param name="left">The relative thickness on the left.</param>
    /// <param name="top">The relative thickness on the top.</param>
    /// <param name="right">The relative thickness on the right.</param>
    /// <param name="bottom">The relative thickness on the bottom.</param>
    public void Deconstruct(
        out RelativeLengthBase left,
        out RelativeLengthBase top,
        out RelativeLengthBase right,
        out RelativeLengthBase bottom) {
        left = Left;
        top = Top;
        right = Right;
        bottom = Bottom;
    }

    /// <summary>
    ///     Checks for equality between a RelativeThickness and an object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///     True if <paramref name="obj" /> is a relative size that equals the current relative size.
    /// </returns>
    public override bool Equals(object? obj) { return obj is RelativeThickness other && Equals(other); }

    /// <summary>
    ///     Returns a hash code for a <see cref="Thickness" />.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() { return HashCode.Combine(Left, Top, Right, Bottom); }

    /// <summary>
    ///     Returns the string representation of the relative thickness.
    /// </summary>
    /// <returns>The string representation of the relative thickness.</returns>
    public override string ToString() { return $"{Left} {Top} {Right} {Bottom}"; }

    /// <summary>
    ///     Compares two RelativeThicknesses.
    /// </summary>
    /// <param name="a">The first relative thickness.</param>
    /// <param name="b">The second relative thickness.</param>
    /// <returns>The equality.</returns>
    public static bool operator ==(RelativeThickness a, RelativeThickness b) { return a.Equals(b); }

    /// <summary>
    ///     Compares two RelativeThicknesses.
    /// </summary>
    /// <param name="a">The first relative thickness.</param>
    /// <param name="b">The second relative thickness.</param>
    /// <returns>The inequality.</returns>
    public static bool operator !=(RelativeThickness a, RelativeThickness b) { return !a.Equals(b); }

    /// <summary>
    ///     Adds two RelativeThicknesses.
    /// </summary>
    /// <param name="a">The first relative thickness.</param>
    /// <param name="b">The second relative thickness.</param>
    /// <returns>The equality.</returns>
    public static RelativeThickness operator +(RelativeThickness a, RelativeThickness b) {
        return new RelativeThickness(a.Left + b.Left, a.Top + b.Top, a.Right + b.Right, a.Bottom + b.Bottom);
    }

    /// <summary>
    ///     Adds a RelativeThickness to a RelativeSize.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <param name="thickness">The thickness.</param>
    /// <returns>The equality.</returns>
    public static RelativeSize operator +(Size size, RelativeThickness thickness) {
        return new RelativeSize(
            size.Width + thickness.Left + thickness.Right,
            size.Height + thickness.Top + thickness.Bottom);
    }

    /// <summary>
    ///     Subtracts two RelativeThicknesses.
    /// </summary>
    /// <param name="a">The first relative thickness.</param>
    /// <param name="b">The second relative thickness.</param>
    /// <returns>The equality.</returns>
    public static RelativeThickness operator -(RelativeThickness a, RelativeThickness b) {
        return new RelativeThickness(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);
    }

    /// <summary>
    ///     Subtracts a RelativeThickness from a RelativeSize.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <param name="thickness">The thickness.</param>
    /// <returns>The equality.</returns>
    public static RelativeSize operator -(RelativeSize size, RelativeThickness thickness) {
        return new RelativeSize(
            size.Width - (thickness.Left + thickness.Right),
            size.Height - (thickness.Top + thickness.Bottom));
    }

    /// <summary>
    ///     Multiplies a RelativeThickness to a scalar.
    /// </summary>
    /// <param name="left">The relative thickness.</param>
    /// <param name="scaler">The scalar.</param>
    /// <returns>The equality.</returns>
    public static RelativeThickness operator *(RelativeThickness left, double scaler) {
        return new RelativeThickness(left.Left * scaler, left.Top * scaler, left.Right * scaler, left.Bottom * scaler);
    }


    /// <summary>
    ///     Divides a RelativeThickness by a scalar.
    /// </summary>
    /// <param name="left">The relative thickness.</param>
    /// <param name="scaler">The scalar.</param>
    /// <returns>The equality.</returns>
    public static RelativeThickness operator /(RelativeThickness left, double scaler) {
        return new RelativeThickness(left.Left / scaler, left.Top / scaler, left.Right / scaler, left.Bottom / scaler);
    }

    public static implicit operator RelativeThickness(Thickness thickness) {
        return new RelativeThickness(
            (RelativeLength)thickness.Left,
            (RelativeLength)thickness.Top,
            (RelativeLength)thickness.Right,
            (RelativeLength)thickness.Bottom);
    }

    public static explicit operator Thickness(RelativeThickness relative) { return relative.Absolute(); }

    public static implicit operator string(RelativeThickness value) { return value.ToString(); }
}