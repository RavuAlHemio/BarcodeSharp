using System.Collections.Immutable;

namespace RavuAlHemio.BarcodeSharp.Mapping
{
    /// <summary>
    /// This interface represents a mapping for a one-dimensional barcode which can map a string into an array of
    /// booleans (representing the color of each bar).
    /// </summary>
    public interface IOneDimensionalBarcodeMapping
    {
        /// <summary>
        /// Returns whether the given string is encodable by this mapping.
        /// </summary>
        /// <param name="stringToEncode">The string to check for encodability.</param>
        /// <returns>Whether <paramref name="stringToEncode"/> can be encoded using this mapping.</returns>
        bool IsEncodable(string stringToEncode);

        /// <summary>
        /// Encodes the given string using this mapping.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <param name="addStartStop">Whether to wrap the data in start (<c>U+0002</c>) and stop (<c>U+0003</c>)
        /// characters if the barcode symbology requires it.</param>
        /// <param name="unencodableSubstitute">The character with which to substitute unencodable characters, or
        /// <c>null</c> to throw an exception instead.</param>
        /// <returns>The encoded barcode as a list of booleans where <c>true</c> is on and <c>false</c> is off.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="unencodableSubstitute"/> is <c>null</c> and
        /// an unencodable character is encountered in <paramref name="stringToEncode"/>.</exception>
        ImmutableArray<bool> EncodeString(string stringToEncode, bool addStartStop = true, char? unencodableSubstitute = null);
    }
}
