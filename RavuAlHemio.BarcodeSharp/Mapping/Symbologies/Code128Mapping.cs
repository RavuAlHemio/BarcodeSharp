using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace RavuAlHemio.BarcodeSharp.Mapping.Symbologies
{
    /// <summary>
    /// Maps characters to Code39 bars.
    /// </summary>
    public class Code128Mapping : IOneDimensionalBarcodeMapping
    {
        internal delegate Tuple<int, string> PrefixEncodeFunction(string prefix);
        internal delegate ImmutableList<int> VariantFunction(string stringToEncode, int maxCount, bool justSwitched);

        internal const char StartA = '\uE011';
        internal const char StartB = '\uE012';
        internal const char StartC = '\uE013';
        internal const char SwitchA = '\uE014';
        internal const char SwitchB = '\uE015';
        internal const char SwitchC = '\uE016';
        internal const char ShiftA = '\uE017';
        internal const char ShiftB = '\uE018';
        // there is no ShiftC (it would be pointless), but let's keep U+E019 unused for periodicity
        internal const char Func1 = '\uE01A';
        internal const char Func2 = '\uE01B';
        internal const char Func3 = '\uE01C';
        internal const char Func4 = '\uE01D';
        internal const char NotEncoded = '\uFFFE';

        internal static readonly ImmutableArray<bool> FinalBar;
        internal static readonly ImmutableArray<ImmutableArray<bool>> ValueSymbols;
        internal static readonly ImmutableDictionary<char, int> CoreMapping;
        internal static readonly ImmutableDictionary<char, int> Mapping128A;
        internal static readonly ImmutableDictionary<char, int> Mapping128B;
        internal static readonly ImmutableDictionary<char, int> SpecialMapping128C;

        /// <summary>
        /// Whether <see cref="EncodeString" /> should add the final bar at the end of the barcode. The default, and
        /// recommended, setting is <c>true</c>.
        /// </summary>
        public bool AddFinalBar { get; set; }

        /// <summary>
        /// Whether <see cref="EncodeString" /> should append the check symbol to the end of the data, before the stop
        /// symbol. The default, and recommended, setting is <c>true</c>.
        /// </summary>
        public bool AddCheckSymbol { get; set; }

        /// <summary>
        /// Whether <see cref="EncodeString" /> should try to find the most efficient encoding available.
        /// </summary>
        public bool Optimize { get; set; }

        internal bool ForceExhaustiveOptimization { get; set; }

        static Code128Mapping()
        {
            FinalBar = IAC(true, true);
            ValueSymbols = IAC(
                /* 000 */ IAC( true,  true, false,  true,  true, false, false,  true,  true, false, false),
                /* 001 */ IAC( true,  true, false, false,  true,  true, false,  true,  true, false, false),
                /* 002 */ IAC( true,  true, false, false,  true,  true, false, false,  true,  true, false),
                /* 003 */ IAC( true, false, false,  true, false, false,  true,  true, false, false, false),
                /* 004 */ IAC( true, false, false,  true, false, false, false,  true,  true, false, false),
                /* 005 */ IAC( true, false, false, false,  true, false, false,  true,  true, false, false),
                /* 006 */ IAC( true, false, false,  true,  true, false, false,  true, false, false, false),
                /* 007 */ IAC( true, false, false,  true,  true, false, false, false,  true, false, false),
                /* 008 */ IAC( true, false, false, false,  true,  true, false, false,  true, false, false),
                /* 009 */ IAC( true,  true, false, false,  true, false, false,  true, false, false, false),
                /* 010 */ IAC( true,  true, false, false,  true, false, false, false,  true, false, false),
                /* 011 */ IAC( true,  true, false, false, false,  true, false, false,  true, false, false),
                /* 012 */ IAC( true, false,  true,  true, false, false,  true,  true,  true, false, false),
                /* 013 */ IAC( true, false, false,  true,  true, false,  true,  true,  true, false, false),
                /* 014 */ IAC( true, false, false,  true,  true, false, false,  true,  true,  true, false),
                /* 015 */ IAC( true, false,  true,  true,  true, false, false,  true,  true, false, false),
                /* 016 */ IAC( true, false, false,  true,  true,  true, false,  true,  true, false, false),
                /* 017 */ IAC( true, false, false,  true,  true,  true, false, false,  true,  true, false),
                /* 018 */ IAC( true,  true, false, false,  true,  true,  true, false, false,  true, false),
                /* 019 */ IAC( true,  true, false, false,  true, false,  true,  true,  true, false, false),
                /* 020 */ IAC( true,  true, false, false,  true, false, false,  true,  true,  true, false),
                /* 021 */ IAC( true,  true, false,  true,  true,  true, false, false,  true, false, false),
                /* 022 */ IAC( true,  true, false, false,  true,  true,  true, false,  true, false, false),
                /* 023 */ IAC( true,  true,  true, false,  true,  true, false,  true,  true,  true, false),
                /* 024 */ IAC( true,  true,  true, false,  true, false, false,  true,  true, false, false),
                /* 025 */ IAC( true,  true,  true, false, false,  true, false,  true,  true, false, false),
                /* 026 */ IAC( true,  true,  true, false, false,  true, false, false,  true,  true, false),
                /* 027 */ IAC( true,  true,  true, false,  true,  true, false, false,  true, false, false),
                /* 028 */ IAC( true,  true,  true, false, false,  true,  true, false,  true, false, false),
                /* 029 */ IAC( true,  true,  true, false, false,  true,  true, false, false,  true, false),
                /* 030 */ IAC( true,  true, false,  true,  true, false,  true,  true, false, false, false),
                /* 031 */ IAC( true,  true, false,  true,  true, false, false, false,  true,  true, false),
                /* 032 */ IAC( true,  true, false, false, false,  true,  true, false,  true,  true, false),
                /* 033 */ IAC( true, false,  true, false, false, false,  true,  true, false, false, false),
                /* 034 */ IAC( true, false, false, false,  true, false,  true,  true, false, false, false),
                /* 035 */ IAC( true, false, false, false,  true, false, false, false,  true,  true, false),
                /* 036 */ IAC( true, false,  true,  true, false, false, false,  true, false, false, false),
                /* 037 */ IAC( true, false, false, false,  true,  true, false,  true, false, false, false),
                /* 038 */ IAC( true, false, false, false,  true,  true, false, false, false,  true, false),
                /* 039 */ IAC( true,  true, false,  true, false, false, false,  true, false, false, false),
                /* 040 */ IAC( true,  true, false, false, false,  true, false,  true, false, false, false),
                /* 041 */ IAC( true,  true, false, false, false,  true, false, false, false,  true, false),
                /* 042 */ IAC( true, false,  true,  true, false,  true,  true,  true, false, false, false),
                /* 043 */ IAC( true, false,  true,  true, false, false, false,  true,  true,  true, false),
                /* 044 */ IAC( true, false, false, false,  true,  true, false,  true,  true,  true, false),
                /* 045 */ IAC( true, false,  true,  true,  true, false,  true,  true, false, false, false),
                /* 046 */ IAC( true, false,  true,  true,  true, false, false, false,  true,  true, false),
                /* 047 */ IAC( true, false, false, false,  true,  true,  true, false,  true,  true, false),
                /* 048 */ IAC( true,  true,  true, false,  true,  true,  true, false,  true,  true, false),
                /* 049 */ IAC( true,  true, false,  true, false, false, false,  true,  true,  true, false),
                /* 050 */ IAC( true,  true, false, false, false,  true, false,  true,  true,  true, false),
                /* 051 */ IAC( true,  true, false,  true,  true,  true, false,  true, false, false, false),
                /* 052 */ IAC( true,  true, false,  true,  true,  true, false, false, false,  true, false),
                /* 053 */ IAC( true,  true, false,  true,  true,  true, false,  true,  true,  true, false),
                /* 054 */ IAC( true,  true,  true, false,  true, false,  true,  true, false, false, false),
                /* 055 */ IAC( true,  true,  true, false,  true, false, false, false,  true,  true, false),
                /* 056 */ IAC( true,  true,  true, false, false, false,  true, false,  true,  true, false),
                /* 057 */ IAC( true,  true,  true, false,  true,  true, false,  true, false, false, false),
                /* 058 */ IAC( true,  true,  true, false,  true,  true, false, false, false,  true, false),
                /* 059 */ IAC( true,  true,  true, false, false, false,  true,  true, false,  true, false),
                /* 060 */ IAC( true,  true,  true, false,  true,  true,  true,  true, false,  true, false),
                /* 061 */ IAC( true,  true, false, false,  true, false, false, false, false,  true, false),
                /* 062 */ IAC( true,  true,  true,  true, false, false, false,  true, false,  true, false),
                /* 063 */ IAC( true, false,  true, false, false,  true,  true, false, false, false, false),
                /* 064 */ IAC( true, false,  true, false, false, false, false,  true,  true, false, false),
                /* 065 */ IAC( true, false, false,  true, false,  true,  true, false, false, false, false),
                /* 066 */ IAC( true, false, false,  true, false, false, false, false,  true,  true, false),
                /* 067 */ IAC( true, false, false, false, false,  true, false,  true,  true, false, false),
                /* 068 */ IAC( true, false, false, false, false,  true, false, false,  true,  true, false),
                /* 069 */ IAC( true, false,  true,  true, false, false,  true, false, false, false, false),
                /* 070 */ IAC( true, false,  true,  true, false, false, false, false,  true, false, false),
                /* 071 */ IAC( true, false, false,  true,  true, false,  true, false, false, false, false),
                /* 072 */ IAC( true, false, false,  true,  true, false, false, false, false,  true, false),
                /* 073 */ IAC( true, false, false, false, false,  true,  true, false,  true, false, false),
                /* 074 */ IAC( true, false, false, false, false,  true,  true, false, false,  true, false),
                /* 075 */ IAC( true,  true, false, false, false, false,  true, false, false,  true, false),
                /* 076 */ IAC( true,  true, false, false,  true, false,  true, false, false, false, false),
                /* 077 */ IAC( true,  true,  true,  true, false,  true,  true,  true, false,  true, false),
                /* 078 */ IAC( true,  true, false, false, false, false,  true, false,  true, false, false),
                /* 079 */ IAC( true, false, false, false,  true,  true,  true,  true, false,  true, false),
                /* 080 */ IAC( true, false,  true, false, false,  true,  true,  true,  true, false, false),
                /* 081 */ IAC( true, false, false,  true, false,  true,  true,  true,  true, false, false),
                /* 082 */ IAC( true, false, false,  true, false, false,  true,  true,  true,  true, false),
                /* 083 */ IAC( true, false,  true,  true,  true,  true, false, false,  true, false, false),
                /* 084 */ IAC( true, false, false,  true,  true,  true,  true, false,  true, false, false),
                /* 085 */ IAC( true, false, false,  true,  true,  true,  true, false, false,  true, false),
                /* 086 */ IAC( true,  true,  true,  true, false,  true, false, false,  true, false, false),
                /* 087 */ IAC( true,  true,  true,  true, false, false,  true, false,  true, false, false),
                /* 088 */ IAC( true,  true,  true,  true, false, false,  true, false, false,  true, false),
                /* 089 */ IAC( true,  true, false,  true,  true, false,  true,  true,  true,  true, false),
                /* 090 */ IAC( true,  true, false,  true,  true,  true,  true, false,  true,  true, false),
                /* 091 */ IAC( true,  true,  true,  true, false,  true,  true, false,  true,  true, false),
                /* 092 */ IAC( true, false,  true, false,  true,  true,  true,  true, false, false, false),
                /* 093 */ IAC( true, false,  true, false, false, false,  true,  true,  true,  true, false),
                /* 094 */ IAC( true, false, false, false,  true, false,  true,  true,  true,  true, false),
                /* 095 */ IAC( true, false,  true,  true,  true,  true, false,  true, false, false, false),
                /* 096 */ IAC( true, false,  true,  true,  true,  true, false, false, false,  true, false),
                /* 097 */ IAC( true,  true,  true,  true, false,  true, false,  true, false, false, false),
                /* 098 */ IAC( true,  true,  true,  true, false,  true, false, false, false,  true, false),
                /* 099 */ IAC( true, false,  true,  true,  true, false,  true,  true,  true,  true, false),
                /* 100 */ IAC( true, false,  true,  true,  true,  true, false,  true,  true,  true, false),
                /* 101 */ IAC( true,  true,  true, false,  true, false,  true,  true,  true,  true, false),
                /* 102 */ IAC( true,  true,  true,  true, false,  true, false,  true,  true,  true, false),
                /* 103 */ IAC( true,  true, false,  true, false, false, false, false,  true, false, false),
                /* 104 */ IAC( true,  true, false,  true, false, false,  true, false, false, false, false),
                /* 105 */ IAC( true,  true, false,  true, false, false,  true,  true,  true, false, false),
                /* 106 */ IAC( true,  true, false, false, false,  true,  true,  true, false,  true, false)
            );

            CoreMapping = new Dictionary<char, int>
            {
                [StartA] = 103,
                [StartB] = 104,
                [StartC] = 105,
                [BarcodeSharpConstants.StopCharacter] = 106
            }.ToImmutableDictionary();

            var mappings = new char[]
            {
                     ' ',      ' ', NotEncoded, // 0
                     '!',      '!', NotEncoded,
                     '"',      '"', NotEncoded,
                     '#',      '#', NotEncoded,
                     '$',      '$', NotEncoded,
                     '%',      '%', NotEncoded,
                     '&',      '&', NotEncoded,
                    '\'',     '\'', NotEncoded,
                     '(',      '(', NotEncoded,
                     ')',      ')', NotEncoded,
                     '*',      '*', NotEncoded, // 10
                     '+',      '+', NotEncoded,
                     ',',      ',', NotEncoded,
                     '-',      '-', NotEncoded,
                     '.',      '.', NotEncoded,
                     '/',      '/', NotEncoded,
                     '0',      '0', NotEncoded,
                     '1',      '1', NotEncoded,
                     '2',      '2', NotEncoded,
                     '3',      '3', NotEncoded,
                     '4',      '4', NotEncoded, // 20
                     '5',      '5', NotEncoded,
                     '6',      '6', NotEncoded,
                     '7',      '7', NotEncoded,
                     '8',      '8', NotEncoded,
                     '9',      '9', NotEncoded,
                     ':',      ':', NotEncoded,
                     ';',      ';', NotEncoded,
                     '<',      '<', NotEncoded,
                     '=',      '=', NotEncoded,
                     '>',      '>', NotEncoded, // 30
                     '?',      '?', NotEncoded,
                     '@',      '@', NotEncoded,
                     'A',      'A', NotEncoded,
                     'B',      'B', NotEncoded,
                     'C',      'C', NotEncoded,
                     'D',      'D', NotEncoded,
                     'E',      'E', NotEncoded,
                     'F',      'F', NotEncoded,
                     'G',      'G', NotEncoded,
                     'H',      'H', NotEncoded, // 40
                     'I',      'I', NotEncoded,
                     'J',      'J', NotEncoded,
                     'K',      'K', NotEncoded,
                     'L',      'L', NotEncoded,
                     'M',      'M', NotEncoded,
                     'N',      'N', NotEncoded,
                     'O',      'O', NotEncoded,
                     'P',      'P', NotEncoded,
                     'Q',      'Q', NotEncoded,
                     'R',      'R', NotEncoded, // 50
                     'S',      'S', NotEncoded,
                     'T',      'T', NotEncoded,
                     'U',      'U', NotEncoded,
                     'V',      'V', NotEncoded,
                     'W',      'W', NotEncoded,
                     'X',      'X', NotEncoded,
                     'Y',      'Y', NotEncoded,
                     'Z',      'Z', NotEncoded,
                     '[',      '[', NotEncoded,
                    '\\',     '\\', NotEncoded, // 60
                     ']',      ']', NotEncoded,
                     '^',      '^', NotEncoded,
                     '_',      '_', NotEncoded,
                '\u0000',      '`', NotEncoded,
                '\u0001',      'a', NotEncoded,
                '\u0002',      'b', NotEncoded,
                '\u0003',      'c', NotEncoded,
                '\u0004',      'd', NotEncoded,
                '\u0005',      'e', NotEncoded,
                '\u0006',      'f', NotEncoded, // 70
                '\u0007',      'g', NotEncoded,
                '\u0008',      'h', NotEncoded,
                '\u0009',      'i', NotEncoded,
                '\u000A',      'j', NotEncoded,
                '\u000B',      'k', NotEncoded,
                '\u000C',      'l', NotEncoded,
                '\u000D',      'm', NotEncoded,
                '\u000E',      'n', NotEncoded,
                '\u000F',      'o', NotEncoded,
                '\u0010',      'p', NotEncoded, // 80
                '\u0011',      'q', NotEncoded,
                '\u0012',      'r', NotEncoded,
                '\u0013',      's', NotEncoded,
                '\u0014',      't', NotEncoded,
                '\u0015',      'u', NotEncoded,
                '\u0016',      'v', NotEncoded,
                '\u0017',      'w', NotEncoded,
                '\u0018',      'x', NotEncoded,
                '\u0019',      'y', NotEncoded,
                '\u001A',      'z', NotEncoded, // 90
                '\u001B',      '{', NotEncoded,
                '\u001C',      '|', NotEncoded,
                '\u001D',      '}', NotEncoded,
                '\u001E',      '~', NotEncoded,
                '\u001F', '\u007F', NotEncoded,
                   Func3,    Func3, NotEncoded,
                   Func2,    Func2, NotEncoded,
                  ShiftB,   ShiftA, NotEncoded,
                 SwitchC,  SwitchC, NotEncoded,
                 SwitchB,    Func4,    SwitchB, // 100
                   Func4,  SwitchA,    SwitchA,
                   Func1,    Func1,      Func1
            };

            Mapping128A = mappings
                .Where((_, i) => i % 3 == 0)  // take every 3rd item starting at 0
                .Select((m, i) => new KeyValuePair<char, int>(m, i))
                .ToImmutableDictionary();
            Mapping128B = mappings
                .Where((_, i) => i % 3 == 1)  // take every 3rd item starting at 1
                .Select((m, i) => new KeyValuePair<char, int>(m, i))
                .ToImmutableDictionary();
            SpecialMapping128C = mappings
                .Where((_, i) => i % 3 == 2)  // take every 3rd item starting at 2
                .Select((m, i) => new KeyValuePair<char, int>(m, i))
                .Where(kvp => kvp.Key != NotEncoded)  // remove the unencoded elements
                .ToImmutableDictionary();

            // all characters encodable by code C are also encodable by A or B
            Debug.Assert(SpecialMapping128C.Keys.All(k => Mapping128A.ContainsKey(k) || Mapping128B.ContainsKey(k)));
        }

        public Code128Mapping()
        {
            AddCheckSymbol = true;
            AddFinalBar = true;
            Optimize = true;
        }

        private static ImmutableArray<T> IAC<T>(params T[] elements)
        {
            return ImmutableArray.Create(elements);
        }

        public bool IsEncodable(string stringToEncode)
        {
            return stringToEncode
                .OfType<char>()
                .All(IsEncodable);
        }

        public bool IsEncodable(char c)
        {
            // no need to check code C because all characters in code C are also in code A or code B
            return CoreMapping.ContainsKey(c) || Mapping128A.ContainsKey(c) || Mapping128B.ContainsKey(c);
        }

        /// <summary>
        /// Encodes the given string as a Code128 barcode.
        /// </summary>
        /// <param name="stringToEncode">The string to encode.</param>
        /// <param name="unencodableSubstitute">The character with which to substitute unencodable characters, or <c>null</c> to throw
        /// an exception instead.</param>
        /// <returns>The encoded barcode as a list of booleans where <value>true</value> is on and <value>false</value> is off.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="unencodableSubstitute"/> is <c>null</c> and an unencodable
        /// character is encountered in <paramref name="stringToEncode"/>.</exception>
        public virtual ImmutableArray<bool> EncodeString(string stringToEncode, char? unencodableSubstitute = null)
        {
            string actualStringToEncode = stringToEncode;
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
            if (!IsEncodable(actualStringToEncode))
            {
                throw new ArgumentException("the string is not encodable using this symbology", nameof(actualStringToEncode));
            }

            // start out with the naïve encoding
            ImmutableList<int> values = EncodeNaively(actualStringToEncode);

            if (Optimize)
            {
                values = FindOptimalEncoding(actualStringToEncode, values.Count);
            }

            if (AddCheckSymbol)
            {
                int checksum = CalculateChecksum(values);
                values = values.Add(checksum);
            }

            // add the stop symbol
            values = values.Add(CoreMapping[BarcodeSharpConstants.StopCharacter]);

            ImmutableArray<bool>.Builder barBuilder = ImmutableArray.CreateBuilder<bool>();
            barBuilder.AddRange(
                values
                    .SelectMany(v => ValueSymbols[v])
                    .ToList()
            );

            if (AddFinalBar)
            {
                barBuilder.AddRange(FinalBar);
            }

            return barBuilder.ToImmutable();
        }

        protected static ImmutableList<int> EncodeNaively(string stringToEncode)
        {
            // handle the degenerate case
            if (stringToEncode.Length == 0)
            {
                return ImmutableList.Create(CoreMapping[StartA]);
            }

            ImmutableList<int>.Builder builder = ImmutableList.CreateBuilder<int>();
            bool mappingA;

            Debug.Assert(stringToEncode.Length > 0);
            if (Mapping128A.ContainsKey(stringToEncode[0]))
            {
                mappingA = true;
            }
            else
            {
                Debug.Assert(Mapping128B.ContainsKey(stringToEncode[0]));
                mappingA = false;
            }

            builder.Add(CoreMapping[mappingA ? StartA : StartB]);

            ImmutableDictionary<char, int> mapping = mappingA ? Mapping128A : Mapping128B;
            foreach (char c in stringToEncode)
            {
                if (!mapping.ContainsKey(c))
                {
                    // switch over
                    if (mappingA)
                    {
                        builder.Add(mapping[SwitchB]);
                        mapping = Mapping128B;
                    }
                    else
                    {
                        builder.Add(mapping[SwitchA]);
                        mapping = Mapping128A;
                    }
                    mappingA = !mappingA;

                    // that one didn't contain it so this one must (we checked for full encodability previously)
                    Debug.Assert(mapping.ContainsKey(c));
                }
                builder.Add(mapping[c]);
            }

            return builder.ToImmutable();
        }

        protected internal virtual ImmutableList<int> FindOptimalEncoding(string stringToEncode, int naiveCount)
        {
            // handle the degenerate case
            if (stringToEncode.Length == 0)
            {
                return ImmutableList.Create(CoreMapping[StartA]);
            }

            ImmutableList<int> bestVariant = AttemptVariants(
                stringToEncode,
                naiveCount,
                CreateVariantTuple(AttemptEncodingA, CoreMapping[StartA]),
                CreateVariantTuple(AttemptEncodingB, CoreMapping[StartB]),
                CreateVariantTuple(AttemptEncodingC, CoreMapping[StartC])
            );
            Debug.Assert(bestVariant != null);
            return bestVariant;
        }

        internal ImmutableList<int> AttemptVariantEncoding(PrefixEncodeFunction variantEncodePrefix,
                Tuple<VariantFunction, int>[] otherVariantsAndSwitchValues,
                Tuple<PrefixEncodeFunction, int> shiftableVariantAndShiftValue,
                string stringToEncode, int maxCount, bool justSwitched
        )
        {
            if (ForceExhaustiveOptimization)
            {
                maxCount = int.MaxValue;
            }

            if (stringToEncode.Length == 0)
            {
                // done!
                return ImmutableList<int>.Empty;
            }

            if (maxCount <= 0)
            {
                // we already found a better solution
                return null;
            }

            ImmutableList<int> bestVariant = null;

            // try encoding
            Tuple<int, string> thisVariantEncoded = variantEncodePrefix(stringToEncode);
            if (thisVariantEncoded != null)
            {
                // the current variant succeeded in encoding the prefix
                string restString = thisVariantEncoded.Item2;
                Debug.Assert(restString.Length < stringToEncode.Length);

                // descend!
                ImmutableList<int> rest = AttemptVariantEncoding(
                    variantEncodePrefix,
                    otherVariantsAndSwitchValues,
                    shiftableVariantAndShiftValue,
                    restString,
                    maxCount - 1,
                    justSwitched: false
                );
                if (rest != null)
                {
                    // the rest, prefixed with the returned value
                    bestVariant = rest.Insert(0, thisVariantEncoded.Item1);
                }
            }

            if (justSwitched)
            {
                // no sense switching twice or shifting right after a switch
                return bestVariant;
            }

            // try switching to one of the other two variants
            ImmutableList<int> bestSwitchVariant = AttemptVariants(
                stringToEncode,
                maxCount,
                otherVariantsAndSwitchValues
            );
            if (bestSwitchVariant != null)
            {
                if (bestVariant == null || bestVariant.Count > bestSwitchVariant.Count)
                {
                    // this one's more efficient
                    bestVariant = bestSwitchVariant;
                }
            }

            if (shiftableVariantAndShiftValue != null)
            {
                // try shifting instead of outright switching
                Tuple<int, string> shiftedResult
                    = shiftableVariantAndShiftValue.Item1.Invoke(stringToEncode);

                if (shiftedResult != null)
                {
                    int shiftedPrefixValue = shiftedResult.Item1;
                    string restAfterShift = shiftedResult.Item2;

                    ImmutableList<int> restMe = AttemptVariantEncoding(
                        variantEncodePrefix,
                        otherVariantsAndSwitchValues,
                        shiftableVariantAndShiftValue,
                        restAfterShift,
                        maxCount - 2,
                        justSwitched: false
                    );
                    if (restMe != null)
                    {
                        ImmutableList<int>.Builder shiftedVariantBuilder = ImmutableList.CreateBuilder<int>();
                        shiftedVariantBuilder.Add(shiftableVariantAndShiftValue.Item2);
                        shiftedVariantBuilder.Add(shiftedPrefixValue);
                        shiftedVariantBuilder.AddRange(restMe);

                        if (bestVariant == null || bestVariant.Count > shiftedVariantBuilder.Count)
                        {
                            bestVariant = shiftedVariantBuilder.ToImmutable();
                        }
                    }
                }
            }

            // done
            return bestVariant;
        }

        internal ImmutableList<int> AttemptEncodingA(string stringToEncode, int maxCount,
                bool justSwitched)
        {
            return AttemptVariantEncoding(
                PrefixEncodeA,
                new[] {
                    CreateVariantTuple(AttemptEncodingB, Mapping128A[SwitchB]),
                    CreateVariantTuple(AttemptEncodingC, Mapping128A[SwitchC])
                },
                CreateShiftTuple(PrefixEncodeB, Mapping128A[ShiftB]),
                stringToEncode, maxCount, justSwitched
            );
        }

        internal ImmutableList<int> AttemptEncodingB(string stringToEncode, int maxCount,
                bool justSwitched)
        {
            return AttemptVariantEncoding(
                PrefixEncodeB,
                new[] {
                    CreateVariantTuple(AttemptEncodingA, Mapping128B[SwitchA]),
                    CreateVariantTuple(AttemptEncodingC, Mapping128B[SwitchC])
                },
                CreateShiftTuple(PrefixEncodeA, Mapping128B[ShiftA]),
                stringToEncode, maxCount, justSwitched
            );
        }

        internal ImmutableList<int> AttemptEncodingC(string stringToEncode, int maxCount,
                bool justSwitched)
        {
            return AttemptVariantEncoding(
                PrefixEncodeC,
                new[] {
                    CreateVariantTuple(AttemptEncodingA, SpecialMapping128C[SwitchA]),
                    CreateVariantTuple(AttemptEncodingB, SpecialMapping128C[SwitchB])
                },
                null,  // no shifting from B
                stringToEncode, maxCount, justSwitched
            );
        }

        internal Tuple<int, string> PrefixEncodeA(string prefix)
        {
            Debug.Assert(prefix.Length > 0);
            if (Mapping128A.ContainsKey(prefix[0]))
            {
                return Tuple.Create(Mapping128A[prefix[0]], prefix.Substring(1));
            }

            return null;
        }

        internal Tuple<int, string> PrefixEncodeB(string prefix)
        {
            Debug.Assert(prefix.Length > 0);
            if (Mapping128B.ContainsKey(prefix[0]))
            {
                return Tuple.Create(Mapping128B[prefix[0]], prefix.Substring(1));
            }

            return null;
        }

        internal Tuple<int, string> PrefixEncodeC(string prefix)
        {
            Debug.Assert(prefix.Length > 0);

            if (prefix.Length > 1)
            {
                if (prefix[0] >= '0' && prefix[0] <= '9' && prefix[1] >= '0' && prefix[1] <= '9')
                {
                    int tens = prefix[0] - '0';
                    int ones = prefix[1] - '0';

                    return Tuple.Create(
                        10 * tens + ones,
                        prefix.Substring(2)
                    );
                }
            }

            if (SpecialMapping128C.ContainsKey(prefix[0]))
            {
                return Tuple.Create(SpecialMapping128C[prefix[0]], prefix.Substring(1));
            }

            return null;
        }

        internal static ImmutableList<int> AttemptVariants(string stringToEncode, int maxCount,
                params Tuple<VariantFunction, int>[] variantMethodsAndSwitchValues)
        {
            ImmutableList<int> bestVariant = null;

            foreach (var variantMethodAndSwitchValue in variantMethodsAndSwitchValues)
            {
                VariantFunction variantMethod = variantMethodAndSwitchValue.Item1;
                int switchValue = variantMethodAndSwitchValue.Item2;

                ImmutableList<int> variant = variantMethod(
                    stringToEncode,
                    (bestVariant?.Count ?? maxCount) - 1,
                    justSwitched: true
                );
                if (variant != null && (bestVariant == null || bestVariant.Count - 1 > variant.Count))
                {
                    bestVariant = variant.Insert(0, switchValue);
                }
            }

            return bestVariant;
        }

        internal static Tuple<VariantFunction, int> CreateVariantTuple(VariantFunction variantMethod, int switchValue)
        {
            return Tuple.Create(variantMethod, switchValue);
        }

        internal static Tuple<PrefixEncodeFunction, int> CreateShiftTuple(PrefixEncodeFunction prefixMethod, int shiftValue)
        {
            return Tuple.Create(prefixMethod, shiftValue);
        }

        protected internal static bool IsPrefixInCode128C(string str)
        {
            if (str.Length < 2)
            {
                return false;
            }

            if (str[0] >= '0' && str[0] <= '9' && str[1] >= '0' && str[1] <= '9')
            {
                return true;
            }

            if (SpecialMapping128C.ContainsKey(str[1]))
            {
                return true;
            }

            return false;
        }

        protected internal virtual int CalculateChecksum(ImmutableList<int> values)
        {
            Debug.Assert(values.Count > 0);
            Debug.Assert(
                values[0] == CoreMapping[StartA]
                || values[0] == CoreMapping[StartB]
                || values[0] == CoreMapping[StartC]
            );
            Debug.Assert(values.Last() != CoreMapping[BarcodeSharpConstants.StopCharacter]);

            // yes, the start symbol and the first data symbol both have weight 1
            int sum = (values[0] * 1) % 103;
            for (int i = 1; i < values.Count; ++i)
            {
                sum = (sum + values[i] * i) % 103;
            }

            return sum;
        }
    }
}
