using System;
using System.Globalization;
using Avalonia;

namespace RelativeControl.Avalonia;

public readonly struct RelativeThickness : IEquatable<RelativeThickness> {
    /// <summary>
    ///     The relative thickness on the left.
    /// </summary>
    public readonly RelativeLength Left;

    /// <summary>
    ///     The relative thickness on the top.
    /// </summary>
    public readonly RelativeLength Top;

    /// <summary>
    ///     The relative thickness on the right.
    /// </summary>
    public readonly RelativeLength Right;

    /// <summary>
    ///     The relative thickness on the bottom.
    /// </summary>
    public readonly RelativeLength Bottom;


    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeThickness" /> structure.
    /// </summary>
    /// <param name="uniformLength">The length that should be applied to all sides.</param>
    /// <param name="target">The relative target.</param>
    public RelativeThickness(RelativeLength uniformLength,Visual? target = null) {
        uniformLength.SetTarget(target);
        Left = Top = Right = Bottom = uniformLength;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeThickness" /> structure.
    /// </summary>
    /// <param name="horizontal">The thickness on the left and right.</param>
    /// <param name="vertical">The thickness on the top and bottom.</param>
    /// <param name="target">The relative target.</param>
    public RelativeThickness(RelativeLength horizontal, RelativeLength vertical, Visual? target = null) {
        horizontal.SetTarget(target);
        vertical.SetTarget(target);
        Left = Right  = horizontal;
        Top  = Bottom = vertical;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeThickness" /> structure.
    /// </summary>
    /// <param name="left">The relative thickness on the left.</param>
    /// <param name="top">The relative thickness on the top.</param>
    /// <param name="right">The relative thickness on the right.</param>
    /// <param name="bottom">The relative thickness on the bottom.</param>
    /// <param name="target">The relative target.</param>
    public RelativeThickness(
        RelativeLength left,
        RelativeLength top,
        RelativeLength right,
        RelativeLength bottom,
        Visual? target = null) {
        left.SetTarget(target);
        top.SetTarget(target);
        right.SetTarget(target);
        bottom.SetTarget(target);
        Left   = left;
        Top    = top;
        Right  = right;
        Bottom = bottom;
    }

    /// <summary>
    ///     Gets a value indicating whether all sides are equal.
    /// </summary>
    public bool IsUniform => Left == Right && Top == Bottom && Left == Top;

    public Thickness Absolute() {
        return new Thickness(Left.ActualPixels, Top.ActualPixels, Right.ActualPixels, Bottom.ActualPixels);
    }


    public void SetTarget(Visual? target) {
        Left.SetTarget(target);
        Top.SetTarget(target);
        Right.SetTarget(target);
        Bottom.SetTarget(target);
    }

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
    ///     Subtracts two RelativeThicknesses.
    /// </summary>
    /// <param name="a">The first relative thickness.</param>
    /// <param name="b">The second relative thickness.</param>
    /// <returns>The equality.</returns>
    public static RelativeThickness operator -(RelativeThickness a, RelativeThickness b) {
        return new RelativeThickness(a.Left - b.Left, a.Top - b.Top, a.Right - b.Right, a.Bottom - b.Bottom);
    }

    /// <summary>
    ///     Multiplies a RelativeThickness to a scalar.
    /// </summary>
    /// <param name="a">The relative thickness.</param>
    /// <param name="b">The scalar.</param>
    /// <returns>The equality.</returns>
    public static RelativeThickness operator *(RelativeThickness a, double b) {
        return new RelativeThickness(a.Left * b, a.Top * b, a.Right * b, a.Bottom * b);
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
    ///     Parses a <see cref="RelativeThickness" /> string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <param name="target">The relative target.</param>
    /// <returns>The <see cref="RelativeThickness" />.</returns>
    public static RelativeThickness Parse(string s, Visual? target = null) {
        string[] vals = s.Trim().Split(' ');
        return vals.Length switch {
            1 => new RelativeThickness(new RelativeLength(vals[0], target)),
            2 => new RelativeThickness(new RelativeLength(vals[0], target), new RelativeLength(vals[1], target)),
            4 => new RelativeThickness(
                new RelativeLength(vals[0], target),
                new RelativeLength(vals[1], target),
                new RelativeLength(vals[2], target),
                new RelativeLength(vals[3], target)),
            _ => throw new FormatException(
                string.Format(CultureInfo.InvariantCulture, "Invalid relative thickness: '{0}'", s))
        };
    }

    /// <summary>
    ///     Returns a boolean indicating whether the relative thickness is equal to the other given point.
    /// </summary>
    /// <param name="other">The other relative thickness to test equality against.</param>
    /// <returns>True if this relative thickness is equal to other; False otherwise.</returns>
    public bool Equals(RelativeThickness other) {
        return Left == other.Left && Top == other.Top && Right == other.Right && Bottom == other.Bottom;
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
    public override string ToString() { return FormattableString.Invariant($"{Left},{Top},{Right},{Bottom}"); }

    public Thickness GetAbsolute() {
        return new Thickness(Left.ActualPixels, Top.ActualPixels, Right.ActualPixels, Bottom.ActualPixels);
    }

    /// <summary>
    ///     Deconstructor the RelativeThickness into its left, top, right and bottom relative thickness values.
    /// </summary>
    /// <param name="left">The relative thickness on the left.</param>
    /// <param name="top">The relative thickness on the top.</param>
    /// <param name="right">The relative thickness on the right.</param>
    /// <param name="bottom">The relative thickness on the bottom.</param>
    public void Deconstruct(
        out RelativeLength left,
        out RelativeLength top,
        out RelativeLength right,
        out RelativeLength bottom) {
        left   = Left;
        top    = Top;
        right  = Right;
        bottom = Bottom;
    }

    public static implicit operator RelativeThickness(Thickness thickness) {
        return new RelativeThickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
    }

    public static explicit operator Thickness(RelativeThickness relative) {
        return new Thickness(
            relative.Left.ActualPixels,
            relative.Top.ActualPixels,
            relative.Right.ActualPixels,
            relative.Bottom.ActualPixels);
    }
}