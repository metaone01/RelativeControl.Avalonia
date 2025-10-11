using System;
using Avalonia;

namespace RelativeControl.Avalonia;

/// <summary>
///     Represents the radii of a rectangle's corners.
/// </summary>
public class RelativeCornerRadius : IRelative<CornerRadius>, IEquatable<RelativeCornerRadius> {
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

    public CornerRadius ActualCornerRadius =>
        new(TopLeft.ActualPixels, TopRight.ActualPixels, BottomRight.ActualPixels, BottomLeft.ActualPixels);


    /// <summary>
    ///     Gets a value indicating whether all relative corner radii are equal.
    /// </summary>
    public bool IsUniform => TopLeft.Equals(TopRight) && TopRight.Equals(BottomRight) && BottomLeft.Equals(BottomRight);

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

    public CornerRadius ActualValue => ActualCornerRadius;

    public event RelativeChangedEventHandler<CornerRadius>? RelativeChanged;

    public CornerRadius Absolute() {
        return new CornerRadius(TopLeft.Absolute(), TopRight.Absolute(), BottomRight.Absolute(), BottomLeft.Absolute());
    }

    public static RelativeCornerRadius operator +(RelativeCornerRadius left, RelativeCornerRadius right) {
        return new RelativeCornerRadius(
            left.TopLeft + right.TopLeft,
            left.TopRight + right.TopRight,
            left.BottomRight + right.BottomRight,
            left.BottomLeft + right.BottomLeft);
    }

    public static RelativeCornerRadius operator -(RelativeCornerRadius left, RelativeCornerRadius right) {
        return new RelativeCornerRadius(
            left.TopLeft - right.TopLeft,
            left.TopRight - right.TopRight,
            left.BottomRight - right.BottomRight,
            left.BottomLeft - right.BottomLeft);
    }

    public static RelativeCornerRadius operator *(RelativeCornerRadius left, double scale) {
        return new RelativeCornerRadius(
            left.TopLeft * scale,
            left.TopRight * scale,
            left.BottomRight * scale,
            left.BottomLeft * scale);
    }

    public static RelativeCornerRadius operator *(double scale, RelativeCornerRadius right) { return right * scale; }

    public static RelativeCornerRadius operator /(RelativeCornerRadius left, double scale) {
        return new RelativeCornerRadius(
            left.TopLeft / scale,
            left.TopRight / scale,
            left.BottomRight / scale,
            left.BottomLeft / scale);
    }

    private void Register() {
        TopLeft.RelativeChanged += UpdateTopLeft;
        TopRight.RelativeChanged += UpdateTopRight;
        BottomRight.RelativeChanged += UpdateBottomRight;
        BottomLeft.RelativeChanged += UpdateBottomLeft;
    }

    private void UpdateTopLeft(IRelative<double>? _, RelativeChangedEventArgs<double> args) {
        RelativeChanged?.Invoke(
            this,
            new RelativeChangedEventArgs<CornerRadius>(
                new CornerRadius(
                    args.OldValue,
                    TopRight.ActualPixels,
                    BottomRight.ActualPixels,
                    BottomLeft.ActualPixels),
                ActualCornerRadius));
    }

    private void UpdateTopRight(IRelative<double>? _, RelativeChangedEventArgs<double> args) {
        RelativeChanged?.Invoke(
            this,
            new RelativeChangedEventArgs<CornerRadius>(
                new CornerRadius(
                    TopLeft.ActualPixels,
                    args.OldValue,
                    BottomRight.ActualPixels,
                    BottomLeft.ActualPixels),
                ActualCornerRadius));
    }

    private void UpdateBottomRight(IRelative<double>? _, RelativeChangedEventArgs<double> args) {
        RelativeChanged?.Invoke(
            this,
            new RelativeChangedEventArgs<CornerRadius>(
                new CornerRadius(TopLeft.ActualPixels, TopRight.ActualPixels, args.OldValue, BottomLeft.ActualPixels),
                ActualCornerRadius));
    }

    private void UpdateBottomLeft(IRelative<double>? _, RelativeChangedEventArgs<double> args) {
        RelativeChanged?.Invoke(
            this,
            new RelativeChangedEventArgs<CornerRadius>(
                new CornerRadius(TopLeft.ActualPixels, TopRight.ActualPixels, BottomRight.ActualPixels, args.OldValue),
                ActualCornerRadius));
    }

    public override string ToString() { return $"{TopLeft} {TopRight} {BottomRight} {BottomLeft}"; }

    public static RelativeCornerRadius Parse(string s, Visual? target = null) {
        string[] vals = Splitters.Split(s, ',', ' ');
        return vals.Length switch {
            1 => new RelativeCornerRadius(RelativeLengthBase.Parse(vals[0], target)),
            4 => new RelativeCornerRadius(
                RelativeLengthBase.Parse(vals[0], target),
                RelativeLengthBase.Parse(vals[1], target),
                RelativeLengthBase.Parse(vals[2], target),
                RelativeLengthBase.Parse(vals[3], target)),
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

    public static explicit operator RelativeCornerRadius(CornerRadius value) {
        return new RelativeCornerRadius(
            (RelativeLength)value.TopLeft,
            (RelativeLength)value.TopRight,
            (RelativeLength)value.BottomRight,
            (RelativeLength)value.BottomLeft);
    }

    public static explicit operator CornerRadius(RelativeCornerRadius value) { return value.ActualCornerRadius; }

    ~RelativeCornerRadius() {
        TopLeft.RelativeChanged -= UpdateTopLeft;
        TopRight.RelativeChanged -= UpdateTopRight;
        BottomRight.RelativeChanged -= UpdateBottomRight;
        BottomLeft.RelativeChanged -= UpdateBottomLeft;
    }
}