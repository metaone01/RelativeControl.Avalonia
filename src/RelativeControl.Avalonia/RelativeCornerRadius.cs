using System;
using Avalonia;

namespace RelativeControl.Avalonia;

/// <summary>
///     Represents the radii of a rectangle's corners.
/// </summary>
public class RelativeCornerRadius : IEquatable<RelativeCornerRadius> {
    public delegate void RelativeCornerRadiusChangedHandler(CornerRadius newCornerRadius);

    public static readonly RelativeCornerRadius Empty = new(RelativeLength.Empty);

    /// <summary>
    ///     Relative radius of the bottom left corner.
    /// </summary>
    public readonly RelativeLengthBase BottomLeft;

    /// <summary>
    ///     Relative radius of the bottom right corner.
    /// </summary>
    public readonly RelativeLengthBase BottomRight;

    /// <summary>
    ///     Relative radius of the top left corner.
    /// </summary>
    public readonly RelativeLengthBase TopLeft;

    /// <summary>
    ///     Relative radius of the top right corner.
    /// </summary>
    public readonly RelativeLengthBase TopRight;

    public RelativeCornerRadius(RelativeLengthBase uniformRadius) {
        TopLeft = TopRight = BottomLeft = BottomRight = uniformRadius;
        Register();
    }

    public RelativeCornerRadius(
        RelativeLengthBase topLeft,
        RelativeLengthBase topRight,
        RelativeLengthBase bottomRight,
        RelativeLengthBase bottomLeft) {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
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

    public event RelativeCornerRadiusChangedHandler? RelativeCornerRadiusChanged;

    public CornerRadius Absolute() {
        double topLeft = double.IsNaN(TopLeft.ActualPixels) ? 0 : TopLeft.ActualPixels;
        double topRight = double.IsNaN(TopRight.ActualPixels) ? 0 : TopRight.ActualPixels;
        double bottomRight = double.IsNaN(BottomRight.ActualPixels) ? 0 : BottomRight.ActualPixels;
        double bottomLeft = double.IsNaN(BottomLeft.ActualPixels) ? 0 : BottomLeft.ActualPixels;
        return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
    }

    public void Register() {
        TopLeft.RelativeLengthChanged += (_, _) => { RelativeCornerRadiusChanged?.Invoke(Absolute()); };
        TopRight.RelativeLengthChanged += (_, _) => { RelativeCornerRadiusChanged?.Invoke(Absolute()); };
        BottomRight.RelativeLengthChanged += (_, _) => { RelativeCornerRadiusChanged?.Invoke(Absolute()); };
        BottomLeft.RelativeLengthChanged += (_, _) => { RelativeCornerRadiusChanged?.Invoke(Absolute()); };
    }

    public override string ToString() { return $"{TopLeft} {TopRight} {BottomRight} {BottomLeft}"; }

    public static RelativeCornerRadius Parse(string s, Visual? target = null) {
        string[] vals = s.Trim().Split(' ');
        return vals.Length switch {
            1 => new RelativeCornerRadius(RelativeLength.Parse(vals[0], target)),
            4 => new RelativeCornerRadius(
                RelativeLength.Parse(vals[0], target),
                RelativeLength.Parse(vals[1], target),
                RelativeLength.Parse(vals[2], target),
                RelativeLength.Parse(vals[3], target)),
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