using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using RavuAlHemio.BarcodeSharp.Mapping.Symbologies;
using Xunit;

namespace BarcodeSharpTests.Mapping.Symbologies
{
    public class Interleaved2Of5MappingTests
    {
        [Fact]
        public void AllMappingWidthsAre5ElementsLong()
        {
            Assert.All(Interleaved2Of5Mapping.MappingWidths.Values, seq => Assert.Equal(5, seq.Length));
        }

        [Fact]
        public void AllMappingWidthsContain3NarrowElements()
        {
            Assert.All(Interleaved2Of5Mapping.MappingWidths.Values, seq => Assert.Equal(3, seq.Count(b => b == false)));
        }

        [Fact]
        public void AllMappingWidthsContain2WideElements()
        {
            Assert.All(Interleaved2Of5Mapping.MappingWidths.Values, seq => Assert.Equal(2, seq.Count(b => b == true)));
        }

        [Fact]
        public void NoTwoDigitsMapToTheSameWidth()
        {
            // remove stop, keep start
            var mapTargets = Interleaved2Of5Mapping.MappingWidths.Values.ToImmutableArray();

            for (int i = 0; i < mapTargets.Length; ++i)
            {
                for (int j = i + 1; j < mapTargets.Length; ++j)
                {
                    Assert.NotEqual((IEnumerable<bool>)mapTargets[i], (IEnumerable<bool>)mapTargets[j]);
                }
            }
        }

        [Fact]
        public void Encode1337()
        {
            var barcode1337 = new[]
            {
                // start
                true, false, true, false,

                // 1 in bars, 3 in spaces
                true, true, false, false, true, false, false, true, false, true, false, true, true, false,

                // 3 in bars, 7 in spaces
                true, true, false, true, true, false, true, false, true, false, false, true, false, false,

                // stop
                true, true, false, true,
            };

            var mapping = new Interleaved2Of5Mapping
            {
                AddStartStop = true
            };
            var encoded = mapping.EncodeString("1337");

            Assert.Equal((IEnumerable<bool>)barcode1337, (IEnumerable<bool>)encoded);
        }

        [Fact]
        public void EncodeWrongLengthThrow()
        {
            var mapping = new Interleaved2Of5Mapping
            {
                AddStartStop = true
            };

            Assert.Throws<ArgumentException>(() => mapping.EncodeString("123"));
        }

        [Fact]
        public void EncodeUnencodableThrow()
        {
            var mapping = new Interleaved2Of5Mapping
            {
                AddStartStop = true
            };

            Assert.Throws<ArgumentException>(() => mapping.EncodeString("\u0CA0__\u0CA0"));
        }

        [Fact]
        public void EncodeUnencodableWithSubstitute()
        {
            var barcodeZeroZero = new[]
            {
                // start
                true, false, true, false,

                // 0 in bars, 0 in spaces
                true, false, true, false, true, true, false, false, true, true, false, false, true, false,

                // stop
                true, true, false, true,
            };

            var mapping = new Interleaved2Of5Mapping
            {
                AddStartStop = true
            };
            var encoded = mapping.EncodeString("AB", '0');

            Assert.Equal((IEnumerable<bool>)barcodeZeroZero, (IEnumerable<bool>)encoded);
        }

        [Fact]
        public void ThrowWhenSubstituteIsUnencodable()
        {
            var mapping = new Interleaved2Of5Mapping
            {
                AddStartStop = true
            };

            // the barcode is valid but the substitute is unencodable
            Assert.Throws<ArgumentException>(() => mapping.EncodeString("12", '!'));
        }
    }
}
