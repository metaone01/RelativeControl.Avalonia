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
    public RelativeThickness(RelativeLength uniformLength) { Left = Top = Right = Bottom = uniformLength; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeThickness" /> structure.
    /// </summary>
    /// <param name="horizontal">The thickness on the left and right.</param>
    /// <param name="vertical">The thickness on the top and bottom.</param>
    public RelativeThickness(RelativeLength horizontal, RelativeLength vertical) {
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
    public RelativeThickness(RelativeLength left, RelativeLength top, RelativeLength right, RelativeLength bottom) {
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


    public RelativeThickness WithTarget(Visual? target) {
        Left.SetTarget(target);
        Top.SetTarget(target);
        Right.SetTarget(target);
        Bottom.SetTarget(target);
        return this;
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
    /// <returns>The <see cref="RelativeThickness" />.</returns>
    public static RelativeThickness Parse(string s) {
        const string exceptionMessage = "Invalid relative thickness.";

        using SpanStringTokenizer tokenizer = new(s, CultureInfo.InvariantCulture, exceptionMessage);
        if (tokenizer.TryReadDouble(out double a)) {
            if (tokenizer.TryReadDouble(out double b)) {
                if (tokenizer.TryReadDouble(out double c))
                    return new RelativeThickness(a, b, c, tokenizer.ReadDouble());
                return new RelativeThickness(a, b);
            }

            return new RelativeThickness(a);
        }

        throw new FormatException(exceptionMessage);
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