using System;
using Avalonia;

namespace RelativeControl.Avalonia;

/// <summary>
///     Represents the radii of a rectangle's corners.
/// </summary>
public class RelativeCornerRadius : IEquatable<RelativeCornerRadius> {
    public delegate void RelativeCornerRadiusChanged(CornerRadius newCornerRadius);

    public static readonly RelativeCornerRadius Empty = new(RelativeLength.Empty);

    /// <summary>
    ///     Relative radius of the bottom left corner.
    /// </summary>
    public readonly RelativeLength BottomLeft;

    /// <summary>
    ///     Relative radius of the bottom right corner.
    /// </summary>
    public readonly RelativeLength BottomRight;

    /// <summary>
    ///     Relative radius of the top left corner.
    /// </summary>
    public readonly RelativeLength TopLeft;

    /// <summary>
    ///     Relative radius of the top right corner.
    /// </summary>
    public readonly RelativeLength TopRight;

    public RelativeCornerRadius(RelativeLength uniformRadius, Visual? target = null) {
        TopLeft = TopRight = BottomLeft = BottomRight = uniformRadius;
        SetTarget(target);
        Register();
    }


    public RelativeCornerRadius(RelativeLength top, RelativeLength bottom, Visual? target = null) {
        TopLeft    = TopRight    = top;
        BottomLeft = BottomRight = bottom;
        SetTarget(target);
        Register();
    }

    public RelativeCornerRadius(
        RelativeLength topLeft,
        RelativeLength topRight,
        RelativeLength bottomRight,
        RelativeLength bottomLeft,
        Visual? target = null) {
        TopLeft     = topLeft;
        TopRight    = topRight;
        BottomRight = bottomRight;
        BottomLeft  = bottomLeft;
        SetTarget(target);
        Register();
    }


    /// <summary>
    ///     Gets a value indicating whether all relative corner radii are equal.
    /// </summary>
    public bool IsUniform => TopLeft.Equals(TopRight) && BottomLeft.Equals(BottomRight) && TopRight.Equals(BottomRight);

    /// <summary>
    ///     Returns a boolean indicating whether the corner radius is equal to the other given relative corner radius.
    /// </summary>
    /// <param name="other">The other relative corner radius to test equality against.</param>
    /// <returns>True if this relative corner radius is equal to other; False otherwise.</returns>
    public bool Equals(RelativeCornerRadius? other) {
        return TopLeft == other?.TopLeft &&
               TopRight == other.TopRight &&
               BottomRight == other.BottomRight &&
               BottomLeft == other.BottomLeft;
    }

    public event RelativeCornerRadiusChanged? OnRelativeCornerRadiusChanged;

    public CornerRadius Absolute() {
        return new CornerRadius(TopLeft.Absolute(), TopRight.Absolute(), BottomRight.Absolute(), BottomLeft.Absolute());
    }

    /// <summary>
    ///     Set another target for this relative corner radius.
    /// </summary>
    /// <param name="target">The new target.</param>
    public void SetTarget(Visual? target) {
        TopLeft.SetTarget(target);
        TopRight.SetTarget(target);
        BottomRight.SetTarget(target);
        BottomLeft.SetTarget(target);
    }

    public void Register() {
        TopLeft.OnRelativeLengthChanged     += (_, _) => { OnRelativeCornerRadiusChanged?.Invoke(Absolute()); };
        TopRight.OnRelativeLengthChanged    += (_, _) => { OnRelativeCornerRadiusChanged?.Invoke(Absolute()); };
        BottomRight.OnRelativeLengthChanged += (_, _) => { OnRelativeCornerRadiusChanged?.Invoke(Absolute()); };
        BottomLeft.OnRelativeLengthChanged  += (_, _) => { OnRelativeCornerRadiusChanged?.Invoke(Absolute()); };
    }

    public override string ToString() { return $"{TopLeft} {TopRight} {BottomRight} {BottomLeft}"; }

    public static RelativeCornerRadius Parse(string s, Visual? target = null) {
        string[] vals = s.Trim().Split(' ');
        return vals.Length switch {
            1 => new RelativeCornerRadius(new RelativeLength(vals[0], target)),
            2 => new RelativeCornerRadius(new RelativeLength(vals[0], target), new RelativeLength(vals[1], target)),
            4 => new RelativeCornerRadius(
                new RelativeLength(vals[0], target),
                new RelativeLength(vals[1], target),
                new RelativeLength(vals[2], target),
                new RelativeLength(vals[3], target)),
            _ => throw new FormatException($"Invalid relative corner radius: '{s}'")
        };
    }

    /// <summary>
    ///     Returns a boolean indicating whether the given Object is equal to this relative corner radius instance.
    /// </summary>
    /// <param name="obj">The Object to compare against.</param>
    /// <returns>True if the Object is equal to this corner radius; False otherwise.</returns>
    public override bool Equals(object? obj) { return obj is RelativeCornerRadius other && Equals(other); }

    public override int GetHashCode() { return HashCode.Combine(TopLeft, TopRight, BottomRight, BottomLeft); }

    public static bool operator ==(RelativeCornerRadius left, RelativeCornerRadius right) { return left.Equals(right); }

    public static bool operator !=(RelativeCornerRadius left, RelativeCornerRadius right) {
        return !left.Equals(right);
    }

    public static RelativeCornerRadius operator *(RelativeCornerRadius left, double scaler) {
        return new RelativeCornerRadius(
            left.TopLeft * scaler,
            left.TopRight * scaler,
            left.BottomRight * scaler,
            left.BottomLeft * scaler);
    }

    public static RelativeCornerRadius operator /(RelativeCornerRadius left, double scaler) {
        return new RelativeCornerRadius(
            left.TopLeft / scaler,
            left.TopRight / scaler,
            left.BottomRight / scaler,
            left.BottomLeft / scaler);
    }

    public static implicit operator string(RelativeCornerRadius value) { return value.ToString(); }
}