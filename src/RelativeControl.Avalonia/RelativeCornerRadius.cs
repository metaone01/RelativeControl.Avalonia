using System;
using System.Globalization;
using Avalonia;

namespace RelativeControl.Avalonia {
    /// <summary>
    /// Represents the radii of a rectangle's corners.
    /// </summary>
    public readonly struct RelativeCornerRadius : IEquatable<RelativeCornerRadius> {
        public RelativeCornerRadius(RelativeLength uniformRadius) {
            TopLeft = TopRight = BottomLeft = BottomRight = uniformRadius;
        }

        public RelativeCornerRadius(RelativeLength top, RelativeLength bottom) {
            TopLeft    = TopRight    = top;
            BottomLeft = BottomRight = bottom;
        }

        public RelativeCornerRadius(
            RelativeLength topLeft,
            RelativeLength topRight,
            RelativeLength bottomRight,
            RelativeLength bottomLeft) {
            TopLeft     = topLeft;
            TopRight    = topRight;
            BottomRight = bottomRight;
            BottomLeft  = bottomLeft;
        }

        /// <summary>
        /// Relative radius of the top left corner.
        /// </summary>
        public readonly RelativeLength TopLeft;

        /// <summary>
        /// Relative radius of the top right corner.
        /// </summary>
        public readonly RelativeLength TopRight;

        /// <summary>
        /// Relative radius of the bottom right corner.
        /// </summary>
        public readonly RelativeLength BottomRight;

        /// <summary>
        /// Relative radius of the bottom left corner.
        /// </summary>
        public readonly RelativeLength BottomLeft;

        /// <summary>
        /// Gets a value indicating whether all relative corner radii are equal.
        /// </summary>
        public bool IsUniform =>
            TopLeft.Equals(TopRight) && BottomLeft.Equals(BottomRight) && TopRight.Equals(BottomRight);

        public CornerRadius Absolute() =>
            new(TopLeft.ActualPixels, TopRight.ActualPixels, BottomRight.ActualPixels, BottomLeft.ActualPixels);


        public RelativeCornerRadius WithTarget(Visual? target) {
            TopLeft.SetTarget(target);
            TopRight.SetTarget(target);
            BottomRight.SetTarget(target);
            BottomLeft.SetTarget(target);
            return this;
        }

        /// <summary>
        /// Returns a boolean indicating whether the corner radius is equal to the other given relative corner radius.
        /// </summary>
        /// <param name="other">The other relative corner radius to test equality against.</param>
        /// <returns>True if this relative corner radius is equal to other; False otherwise.</returns>
        public bool Equals(RelativeCornerRadius other) {
            return TopLeft == other.TopLeft &&
                   TopRight == other.TopRight &&
                   BottomRight == other.BottomRight &&
                   BottomLeft == other.BottomLeft;
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this relative corner radius instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this corner radius; False otherwise.</returns>
        public override bool Equals(object? obj) { return obj is RelativeCornerRadius other && Equals(other); }

        public override int GetHashCode() { return HashCode.Combine(TopLeft, TopRight, BottomLeft, BottomRight); }

        public override string ToString() {
            return FormattableString.Invariant($"{TopLeft},{TopRight},{BottomRight},{BottomLeft}");
        }

        public static RelativeCornerRadius Parse(string s) {
            const string exceptionMessage = "Invalid RelativeCornerRadius.";

            using SpanStringTokenizer tokenizer = new(s, CultureInfo.InvariantCulture, exceptionMessage);
            if (tokenizer.TryReadDouble(out var a)) {
                if (tokenizer.TryReadDouble(out var b)) {
                    if (tokenizer.TryReadDouble(out var c))
                        return new RelativeCornerRadius(a, b, c, tokenizer.ReadDouble());
                    return new RelativeCornerRadius(a, b);
                }

                return new RelativeCornerRadius(a);
            }

            throw new FormatException(exceptionMessage);
        }

        public static bool operator ==(RelativeCornerRadius left, RelativeCornerRadius right) {
            return left.Equals(right);
        }

        public static bool operator !=(RelativeCornerRadius left, RelativeCornerRadius right) {
            return !left.Equals(right);
        }
    }
}