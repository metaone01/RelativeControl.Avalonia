using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Avalonia;
using Vector = Avalonia.Vector;

namespace RelativeControl.Avalonia;

/// <summary>
///     Defines a size.
/// </summary>
public class RelativeSize : IEquatable<RelativeSize> {
    public delegate void RelativeSizeChanged(Size newSize);

    /// <summary>
    ///     A size representing infinity.
    /// </summary>
    public static readonly RelativeSize Infinity = new(double.PositiveInfinity, double.PositiveInfinity);

    public static readonly RelativeSize Empty = new(RelativeLength.Empty, RelativeLength.Empty);

    /// <summary>
    ///     The height.
    /// </summary>
    public readonly RelativeLength Height;

    /// <summary>
    ///     The width.
    /// </summary>
    public readonly RelativeLength Width;

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeSize" /> structure.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="target">The target control.</param>
    public RelativeSize(RelativeLength width, RelativeLength height, Visual? target = null) {
        Width  = width;
        Height = height;
        SetTarget(target);
        Register();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="RelativeSize" /> structure.
    /// </summary>
    /// <param name="vector2">The vector to take values from.</param>
    public RelativeSize(Vector2 vector2) : this(vector2.X, vector2.Y) { }

    /// <summary>
    ///     Gets the aspect ratio of the size.
    /// </summary>
    public double AspectRatio => Width.ActualPixels / Height.ActualPixels;

    /// <summary>
    ///     Returns a boolean indicating whether the size is equal to the other given size (bitwise).
    /// </summary>
    /// <param name="other">The other size to test equality against.</param>
    /// <returns>True if this size is equal to other; False otherwise.</returns>
    public bool Equals(RelativeSize? other) { return Width == other?.Width && Height == other.Height; }

    public event RelativeSizeChanged? OnRelativeSizeChanged;

    public void SetTarget(Visual? target) {
        Width.SetTarget(target);
        Height.SetTarget(target);
    }


    private void Register() {
        Width.OnRelativeLengthChanged  += (_, _) => { OnRelativeSizeChanged?.Invoke(Absolute()); };
        Height.OnRelativeLengthChanged += (_, _) => { OnRelativeSizeChanged?.Invoke(Absolute()); };
    }

    public Size Absolute() { return new Size(Width.ActualPixels, Height.ActualPixels); }

    /// <summary>
    ///     Returns the string representation of the size.
    /// </summary>
    /// <returns>The string representation of the size.</returns>
    public override string ToString() { return $"{Width}, {Height}"; }


    /// <summary>
    ///     Constrains the size.
    /// </summary>
    /// <param name="constraint">The size to constrain to.</param>
    /// <returns>The constrained size.</returns>
    public RelativeSize Constrain(RelativeSize constraint) {
        return new RelativeSize(
            RelativeLength.Min(Width, constraint.Width),
            RelativeLength.Min(Height, constraint.Height));
    }

    /// <summary>
    ///     Deflates the size by a <see cref="Thickness" />.
    /// </summary>
    /// <param name="thickness">The thickness.</param>
    /// <returns>The deflated size.</returns>
    /// <remarks>The deflated size cannot be less than 0.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RelativeSize Deflate(RelativeThickness thickness) {
        return new RelativeSize(Width - thickness.Left - thickness.Right, Height - thickness.Top - thickness.Bottom);
    }

    /// <summary>
    ///     Inflates the size by a <see cref="Thickness" />.
    /// </summary>
    /// <param name="thickness">The thickness.</param>
    /// <returns>The inflated size.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public RelativeSize Inflate(RelativeThickness thickness) {
        return new RelativeSize(Width + thickness.Left + thickness.Right, Height + thickness.Top + thickness.Bottom);
    }

    /// <summary>
    ///     Parses a <see cref="Size" /> string.
    /// </summary>
    /// <param name="s">The string.</param>
    /// <returns>The <see cref="Size" />.</returns>
    public static RelativeSize Parse(string s) {
        string[] vars = s.Trim().Split(' ');
        if (vars.Length != 2)
            throw new FormatException($"Invalid relative size: {s}");
        return new RelativeSize(new RelativeLength(vars[0]), new RelativeLength(vars[1]));
    }

    /// <summary>
    ///     Deconstructs the size into its Width and Height values.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public void Deconstruct(out RelativeLength width, out RelativeLength height) {
        width  = Width;
        height = Height;
    }

    /// <summary>
    ///     Checks for equality between a size and an object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///     True if <paramref name="obj" /> is a size that equals the current size.
    /// </returns>
    public override bool Equals(object? obj) { return obj is RelativeSize other && Equals(other); }

    /// <summary>
    ///     Returns a hash code for a <see cref="Size" />.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode() { return HashCode.Combine(Width, Height); }

    /// <summary>
    ///     Checks for equality between two <see cref="RelativeSize" />s.
    /// </summary>
    /// <param name="left">The first size.</param>
    /// <param name="right">The second size.</param>
    /// <returns>True if the sizes are equal; otherwise false.</returns>
    public static bool operator ==(RelativeSize left, RelativeSize right) { return left.Equals(right); }

    /// <summary>
    ///     Checks for inequality between two <see cref="RelativeSize" />s.
    /// </summary>
    /// <param name="left">The first size.</param>
    /// <param name="right">The second size.</param>
    /// <returns>True if the sizes are unequal; otherwise false.</returns>
    public static bool operator !=(RelativeSize left, RelativeSize right) { return !(left == right); }

    public static RelativeSize operator +(RelativeSize size, RelativeSize toAdd) {
        return new RelativeSize(size.Width + toAdd.Width, size.Height + toAdd.Height);
    }

    public static RelativeSize operator -(RelativeSize size, RelativeSize toSubtract) {
        return new RelativeSize(size.Width - toSubtract.Width, size.Height - toSubtract.Height);
    }

    /// <summary>
    ///     Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static RelativeSize operator *(RelativeSize size, Vector scale) {
        return new RelativeSize(size.Width * scale.X, size.Height * scale.Y);
    }

    /// <summary>
    ///     Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static RelativeSize operator *(RelativeSize size, double scale) {
        return new RelativeSize(size.Width * scale, size.Height * scale);
    }

    /// <summary>
    ///     Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static RelativeSize operator /(RelativeSize size, Vector scale) {
        return new RelativeSize(size.Width / scale.X, size.Height / scale.Y);
    }

    /// <summary>
    ///     Scales a size.
    /// </summary>
    /// <param name="size">The size</param>
    /// <param name="scale">The scaling factor.</param>
    /// <returns>The scaled size.</returns>
    public static RelativeSize operator /(RelativeSize size, double scale) {
        return new RelativeSize(size.Width / scale, size.Height / scale);
    }
}