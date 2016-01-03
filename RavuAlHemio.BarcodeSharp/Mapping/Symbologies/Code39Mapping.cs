using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RavuAlHemio.BarcodeSharp.Mapping.Symbologies
{
    /// <summary>
    /// Maps characters to Code39 bars.
    /// </summary>
    public class Code39Mapping : IOneDimensionalBarcodeMapping
    {
        internal static readonly ImmutableDictionary<char, ImmutableArray<bool>> StandardMappings;
        internal static readonly ImmutableDictionary<char, ImmutableArray<bool>> SpecialMappings;
        internal static readonly ImmutableDictionary<char, ImmutableArray<bool>> Mappings;

        static Code39Mapping()
        {
            var standardMappings = new Dictionary<char, ImmutableArray<bool>>
            {
                {'A', (new[] { true,  true, false,  true, false,  true, false, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'B', (new[] { true, false,  true,  true, false,  true, false, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'C', (new[] { true,  true, false,  true,  true, false,  true, false, false,  true, false,  true, false}).ToImmutableArray()},
                {'D', (new[] { true, false,  true, false,  true,  true, false, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'E', (new[] { true,  true, false,  true, false,  true,  true, false, false,  true, false,  true, false}).ToImmutableArray()},
                {'F', (new[] { true, false,  true,  true, false,  true,  true, false, false,  true, false,  true, false}).ToImmutableArray()},
                {'G', (new[] { true, false,  true, false,  true, false, false,  true,  true, false,  true,  true, false}).ToImmutableArray()},
                {'H', (new[] { true,  true, false,  true, false,  true, false, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'I', (new[] { true, false,  true,  true, false,  true, false, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'J', (new[] { true, false,  true, false,  true,  true, false, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'K', (new[] { true,  true, false,  true, false,  true, false,  true, false, false,  true,  true, false}).ToImmutableArray()},
                {'L', (new[] { true, false,  true,  true, false,  true, false,  true, false, false,  true,  true, false}).ToImmutableArray()},
                {'M', (new[] { true,  true, false,  true,  true, false,  true, false,  true, false, false,  true, false}).ToImmutableArray()},
                {'N', (new[] { true, false,  true, false,  true,  true, false,  true, false, false,  true,  true, false}).ToImmutableArray()},
                {'O', (new[] { true,  true, false,  true, false,  true,  true, false,  true, false, false,  true, false}).ToImmutableArray()},
                {'P', (new[] { true, false,  true,  true, false,  true,  true, false,  true, false, false,  true, false}).ToImmutableArray()},
                {'Q', (new[] { true, false,  true, false,  true, false,  true,  true, false, false,  true,  true, false}).ToImmutableArray()},
                {'R', (new[] { true,  true, false,  true, false,  true, false,  true,  true, false, false,  true, false}).ToImmutableArray()},
                {'S', (new[] { true, false,  true,  true, false,  true, false,  true,  true, false, false,  true, false}).ToImmutableArray()},
                {'T', (new[] { true, false,  true, false,  true,  true, false,  true,  true, false, false,  true, false}).ToImmutableArray()},
                {'U', (new[] { true,  true, false, false,  true, false,  true, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'V', (new[] { true, false, false,  true,  true, false,  true, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'W', (new[] { true,  true, false, false,  true,  true, false,  true, false,  true, false,  true, false}).ToImmutableArray()},
                {'X', (new[] { true, false, false,  true, false,  true,  true, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'Y', (new[] { true,  true, false, false,  true, false,  true,  true, false,  true, false,  true, false}).ToImmutableArray()},
                {'Z', (new[] { true, false, false,  true,  true, false,  true,  true, false,  true, false,  true, false}).ToImmutableArray()},
                {'0', (new[] { true, false,  true, false, false,  true,  true, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'1', (new[] { true,  true, false,  true, false, false,  true, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'2', (new[] { true, false,  true,  true, false, false,  true, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'3', (new[] { true,  true, false,  true,  true, false, false,  true, false,  true, false,  true, false}).ToImmutableArray()},
                {'4', (new[] { true, false,  true, false, false,  true,  true, false,  true, false,  true,  true, false}).ToImmutableArray()},
                {'5', (new[] { true,  true, false,  true, false, false,  true,  true, false,  true, false,  true, false}).ToImmutableArray()},
                {'6', (new[] { true, false,  true,  true, false, false,  true,  true, false,  true, false,  true, false}).ToImmutableArray()},
                {'7', (new[] { true, false,  true, false, false,  true, false,  true,  true, false,  true,  true, false}).ToImmutableArray()},
                {'8', (new[] { true,  true, false,  true, false, false,  true, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'9', (new[] { true, false,  true,  true, false, false,  true, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'\u0002', (new[] { true, false, false,  true, false,  true,  true, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'\u0003', (new[] { true, false, false,  true, false,  true,  true, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {' ', (new[] { true, false, false,  true,  true, false,  true, false,  true,  true, false,  true, false}).ToImmutableArray()},
                {'-', (new[] { true, false, false,  true, false,  true, false,  true,  true, false,  true,  true, false}).ToImmutableArray()},
                {'.', (new[] { true,  true, false, false,  true, false,  true, false,  true,  true, false,  true, false}).ToImmutableArray()},
            };
            StandardMappings = standardMappings.ToImmutableDictionary();

            var specialMappings = new Dictionary<char, ImmutableArray<bool>>
            {
                {'$', (new[] { true, false, false,  true, false, false,  true, false, false,  true, false,  true, false}).ToImmutableArray()},
                {'%', (new[] { true, false,  true, false, false,  true, false, false,  true, false, false,  true, false}).ToImmutableArray()},
                {'/', (new[] { true, false, false,  true, false, false,  true, false,  true, false, false,  true, false}).ToImmutableArray()},
                {'+', (new[] { true, false, false,  true, false,  true, false, false,  true, false, false,  true, false}).ToImmutableArray()},
            };
            SpecialMappings = specialMappings.ToImmutableDictionary();

            Mappings = standardMappings.FollowedBy(specialMappings).ToImmutableDictionary();
        }

        public bool IsEncodable(string stringToEncode)
        {
            return stringToEncode
                .OfType<char>()
                .All(c => Mappings.ContainsKey(c));
        }

        /// <summary>
        /// Encodes the given string as a Code39 barcode.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <param name="addStartStop">Whether to wrap the data in start (<value>U+0002</value>) and stop (<value>U+0003</value>) characters.</param>
        /// <param name="unencodableSubstitute">The character with which to substitute unencodable characters, or <c>null</c> to throw
        /// an exception instead.</param>
        /// <returns>The encoded barcode as a list of booleans where <value>true</value> is on and <value>false</value> is off.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="unencodableSubstitute"/> is <c>null</c> and an unencodable
        /// character is encountered in <paramref name="stringToEncode"/>.</exception>
        public ImmutableArray<bool> EncodeString(string stringToEncode, bool addStartStop = true, char? unencodableSubstitute = null)
        {
            var actualStringToEncode = stringToEncode;
            if (addStartStop)
            {
                actualStringToEncode = "\u0002" + stringToEncode + "\u0003";
            }
            if (unencodableSubstitute.HasValue && !Mappings.ContainsKey(unencodableSubstitute.Value))
            {
                throw new ArgumentException("the substitute character is not encodable", nameof(unencodableSubstitute));
            }
            
            try
            {
                if (unencodableSubstitute.HasValue)
                {
                    return actualStringToEncode
                        .OfType<char>()
                        .SelectMany(c => Mappings.ContainsKey(c) ? Mappings[c] : Mappings[unencodableSubstitute.Value])
                        .ToImmutableArray();
                }
                else
                {
                    return actualStringToEncode
                        .OfType<char>()
                        .SelectMany(c => Mappings[c])
                        .ToImmutableArray();
                }
            }
            catch (KeyNotFoundException exc)
            {
                throw new ArgumentException("string to encode contains an unencodable character", nameof(stringToEncode), exc);
            }
        }
    }
}
