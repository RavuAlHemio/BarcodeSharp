using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace RavuAlHemio.BarcodeSharp.Mapping.Symbologies
{
    /// <summary>
    /// Maps characters to Interleaved 2 of 5 bars.
    /// </summary>
    public class Interleaved2Of5Mapping : IOneDimensionalBarcodeMapping
    {
        internal static readonly ImmutableDictionary<char, ImmutableArray<bool>> StandardMappings;
        internal static readonly ImmutableDictionary<char, ImmutableArray<bool>> MappingWidths;

        static Interleaved2Of5Mapping()
        {
            var standardMappings = new Dictionary<char, ImmutableArray<bool>>
            {
                {'\uE000', (new[] { true, false,  true, false}).ToImmutableArray()},
                {'\uE001', (new[] { true,  true, false,  true}).ToImmutableArray()},
            };
            StandardMappings = standardMappings.ToImmutableDictionary();

            var mappingWidths = new Dictionary<char, ImmutableArray<bool>>
            {
                {'0', (new[] {false, false,  true,  true, false}).ToImmutableArray()},
                {'1', (new[] { true, false, false, false,  true}).ToImmutableArray()},
                {'2', (new[] {false,  true, false, false,  true}).ToImmutableArray()},
                {'3', (new[] { true,  true, false, false, false}).ToImmutableArray()},
                {'4', (new[] {false, false,  true, false,  true}).ToImmutableArray()},
                {'5', (new[] { true, false,  true, false, false}).ToImmutableArray()},
                {'6', (new[] {false,  true,  true, false, false}).ToImmutableArray()},
                {'7', (new[] {false, false, false,  true,  true}).ToImmutableArray()},
                {'8', (new[] { true, false, false,  true, false}).ToImmutableArray()},
                {'9', (new[] {false,  true, false,  true, false}).ToImmutableArray()},
            };
            MappingWidths = mappingWidths.ToImmutableDictionary();
        }

        private bool IsEncodable(char charToEncode)
        {
            return StandardMappings.ContainsKey (charToEncode) || MappingWidths.ContainsKey (charToEncode);
        }

        public bool IsEncodable(string stringToEncode)
        {
            return stringToEncode
                .OfType<char>()
                .All(IsEncodable);
        }

        /// <summary>
        /// Encodes the given string as a Code39 barcode.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <param name="addStartStop">Whether to wrap the data in start (<value>U+E000</value>) and stop (<value>U+E001</value>) characters.</param>
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
                actualStringToEncode = "\uE000" + stringToEncode + "\uE001";
            }
            if (unencodableSubstitute.HasValue)
            {
                if (!IsEncodable(unencodableSubstitute.Value))
                {
                    throw new ArgumentException("the substitute character is not encodable", nameof(unencodableSubstitute));
                }

                actualStringToEncode = new string(
                    actualStringToEncode
                        .OfType<char>()
                        .Select(c => IsEncodable(c) ? c : unencodableSubstitute.Value)
                        .ToArray()
                );
            }
            if (actualStringToEncode.Length % 2 != 0)
            {
                throw new ArgumentException("only an even number of characters can be encoded", nameof(stringToEncode));
            }
            if (!IsEncodable(actualStringToEncode))
            {
                throw new ArgumentException("string cannot be encoded using this symbology", nameof(stringToEncode));
            }

            var ret = ImmutableArray.CreateBuilder<bool>();
            int i = 0;
            while (i < actualStringToEncode.Length)
            {
                char c = actualStringToEncode[i];

                Debug.Assert(StandardMappings.ContainsKey(c) || MappingWidths.ContainsKey(c));

                if (StandardMappings.ContainsKey(c))
                {
                    // just add it
                    ret.AddRange(StandardMappings[c]);

                    // one step forward
                    ++i;
                }
                else if (MappingWidths.ContainsKey(c))
                {
                    Debug.Assert(actualStringToEncode.Length > i + 1);
                    char c2 = actualStringToEncode[i + 1];

                    if (!MappingWidths.ContainsKey(c2))
                    {
                        // e.g. "<START>0<STOP>12<STOP>"
                        throw new ArgumentException($"width-mapped character '{c}' must be followed by another width-mapped character, not '{c2}'", nameof(stringToEncode));
                    }

                    var barWidths = MappingWidths[c];
                    var spaceWidths = MappingWidths[c2];
                    var pairWidths = barWidths.Zip(spaceWidths, Tuple.Create);
                    foreach (var barAndSpace in pairWidths)
                    {
                        // at least a thin bar
                        ret.Add(true);

                        if (barAndSpace.Item1)
                        {
                            // thick bar
                            ret.Add(true);
                        }

                        // at least a thin space
                        ret.Add(false);

                        if (barAndSpace.Item2)
                        {
                            // thin bar
                            ret.Add(false);
                        }
                    }

                    // two steps forward
                    i += 2;
                }
            }

            return ret.ToImmutableArray();
        }
    }
}
