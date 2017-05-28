using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using RavuAlHemio.BarcodeSharp;
using RavuAlHemio.BarcodeSharp.Mapping.Symbologies;
using Xunit;

namespace BarcodeSharpTests.Mapping.Symbologies
{
    public class Code39MappingTests
    {
        [Fact]
        public void AllMappableCharsMapToSequences13ElementsLong()
        {
            Assert.All(Code39Mapping.Mappings.Values, seq => Assert.Equal(13, seq.Length));
        }

        [Fact]
        public void AllMappableCharsMapToSequencesStartingWithOnElement()
        {
            Assert.All(Code39Mapping.Mappings.Values, seq => Assert.True(seq[0]));
        }

        [Fact]
        public void AllMappableCharsMapToSequencesEndingWithOffElement()
        {
            Assert.All(Code39Mapping.Mappings.Values, seq => Assert.False(seq[seq.Length - 1]));
        }

        [Fact]
        public void NoMappableCharMapsToSequenceWith3ConsecutiveOnElements()
        {
            Assert.All(Code39Mapping.Mappings.Values, seq => Assert.Empty(seq.SublistFoundIndices(new[] { true, true, true })));
        }

        [Fact]
        public void NoMappableCharMapsToSequenceWith3ConsecutiveOffElements()
        {
            Assert.All(Code39Mapping.Mappings.Values, seq => Assert.Empty(seq.SublistFoundIndices(new[] { false, false, false })));
        }

        [Fact]
        public void AllNormallyMappableCharsMapToSequencesWith1SubsequenceOf2ConsecutiveOffElements()
        {
            Assert.All(Code39Mapping.StandardMappings.Values, seq => Assert.Equal(1, seq.SublistFoundIndices(new[] { false, false }).Count()));
        }

        [Fact]
        public void AllNormallyMappableCharsMapToSequencesWith2SubsequencesOf2ConsecutiveOnElements()
        {
            Assert.All(Code39Mapping.StandardMappings.Values, seq => Assert.Equal(2, seq.SublistFoundIndices(new[] { true, true }).Count()));
        }

        [Fact]
        public void AllSpeciallyMappableCharsMapToSequencesWith3OnOffOffSubsequences()
        {
            Assert.All(Code39Mapping.SpecialMappings.Values, seq => Assert.Equal(3, seq.SublistFoundIndices(new[] { true, false, false }).Count()));
        }

        [Fact]
        public void NoSpeciallyMappableCharMapsToSequenceWith2ConsecutiveOnElements()
        {
            Assert.All(Code39Mapping.SpecialMappings.Values, seq => Assert.Empty(seq.SublistFoundIndices(new[] { true, true })));
        }

        [Fact]
        public void NoTwoMappableCharactersExceptStartAndStopMapToTheSameSequence()
        {
            // remove stop, keep start
            var mapTargets = Code39Mapping.Mappings
                .Where(m => m.Key != BarcodeSharpConstants.StopCharacter)
                .Select(m => m.Value)
                .ToImmutableArray();

            for (int i = 0; i < mapTargets.Length; ++i)
            {
                for (int j = i + 1; j < mapTargets.Length; ++j)
                {
                    Assert.NotEqual((IEnumerable<bool>)mapTargets[i], (IEnumerable<bool>)mapTargets[j]);
                }
            }
        }

        [Fact]
        public void EncodeLOL()
        {
            var barcodeLOL = new[]
            {
                // start
                true, false, false, true, false, true, true, false, true, true, false, true, false,

                // L
                true, false, true, true, false, true, false, true, false, false, true, true, false,

                // O
                true, true, false, true, false, true, true, false, true, false, false, true, false,

                // L
                true, false, true, true, false, true, false, true, false, false, true, true, false,

                // stop
                true, false, false, true, false, true, true, false, true, true, false, true, false,
            };

            var mapping = new Code39Mapping();
            var encoded = mapping.EncodeString("LOL", true);

            Assert.Equal((IEnumerable<bool>)barcodeLOL, (IEnumerable<bool>)encoded);
        }

        [Fact]
        public void EncodeHHGG42()
        {
            var barcodeHHGG42 = new[]
            {
                // start
                true, false, false, true, false, true, true, false, true, true, false, true, false,

                // H
                true, true, false, true, false, true, false, false, true, true, false, true, false,

                // H
                true, true, false, true, false, true, false, false, true, true, false, true, false,

                // G
                true, false, true, false, true, false, false, true, true, false, true, true, false,

                // G
                true, false, true, false, true, false, false, true, true, false, true, true, false,

                // 4
                true, false, true, false, false, true, true, false, true, false, true, true, false,

                // 2
                true, false, true, true, false, false, true, false, true, false, true, true, false,

                // stop
                true, false, false, true, false, true, true, false, true, true, false, true, false,
            };

            var mapping = new Code39Mapping();
            var encoded = mapping.EncodeString("HHGG42", true);

            Assert.Equal((IEnumerable<bool>)barcodeHHGG42, (IEnumerable<bool>)encoded);
        }

        [Fact]
        public void EncodeUnencodableThrow()
        {
            var mapping = new Code39Mapping();

            Assert.Throws<ArgumentException>(() => mapping.EncodeString("\u0CA0_\u0CA0", true));
        }

        [Fact]
        public void EncodeUnencodableWithSubstitute()
        {
            var barcodeDotDotDot = new[]
            {
                // start
                true, false, false, true, false, true, true, false, true, true, false, true, false,

                // .
                true, true, false, false, true, false, true, false, true, true, false, true, false,

                // .
                true, true, false, false, true, false, true, false, true, true, false, true, false,

                // .
                true, true, false, false, true, false, true, false, true, true, false, true, false,

                // stop
                true, false, false, true, false, true, true, false, true, true, false, true, false,
            };

            var mapping = new Code39Mapping();
            var encoded = mapping.EncodeString("\u0CA0_\u0CA0", true, '.');

            Assert.Equal((IEnumerable<bool>)barcodeDotDotDot, (IEnumerable<bool>)encoded);
        }

        [Fact]
        public void ThrowWhenSubstituteIsUnencodable()
        {
            var mapping = new Code39Mapping();

            // the barcode is valid but the substitute is unencodable
            Assert.Throws<ArgumentException>(() => mapping.EncodeString("123", true, '_'));
        }
    }
}
