using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using RavuAlHemio.BarcodeSharp;
using RavuAlHemio.BarcodeSharp.Mapping.Symbologies;
using Xunit;

namespace BarcodeSharpTests.Mapping.Symbologies
{
    public class Code128MappingTests
    {
        [Fact]
        public void CheckDigitCalculationPJJ123C()
        {
            // (Wikipedia example)

            ImmutableList<int> values = ImmutableList.Create(
                103, // start A
                 48, // P
                 42, // J
                 42, // J
                 17, // 1
                 18, // 2
                 19, // 3
                 35  // C
            );

            var c128 = new Code128Mapping();
            int checksum = c128.CalculateChecksum(values);
            Assert.Equal(54, checksum);
        }

        [Fact]
        public void NaiveEncodingPJJ123C()
        {
            // (Wikipedia example)
            var barcodePJJ123C = new[]
            {
                // start A = 103
                true, true, false, true, false, false, false, false, true, false, false,

                // A(P) = 48
                true, true, true, false, true, true, true, false, true, true, false,

                // A(J) = 42 (x2)
                true, false, true, true, false, true, true, true, false, false, false,
                true, false, true, true, false, true, true, true, false, false, false,

                // A(1) = 17
                true, false, false, true, true, true, false, false, true, true, false,

                // A(2) = 18
                true, true, false, false, true, true, true, false, false, true, false,

                // A(3) = 19
                true, true, false, false, true, false, true, true, true, false, false,

                // A(C) = 35
                true, false, false, false, true, false, false, false, true, true, false,

                // check character = 54
                true, true, true, false, true, false, true, true, false, false, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "PJJ123C";
            var c128 = new Code128Mapping
            {
                Optimize = false
            };

            Assert.True(c128.IsEncodable(s));

            ImmutableArray<bool> bars = c128.EncodeString(s);
            Assert.Equal((IEnumerable<bool>)barcodePJJ123C, bars);
        }

        [Fact]
        public void NaiveEncoding12345678()
        {
            // (Wikipedia example)
            var barcode12345678 = new[]
            {
                // start A = 103
                true, true, false, true, false, false, false, false, true, false, false,

                // A(1) = 17
                true, false, false, true, true, true, false, false, true, true, false,

                // A(2) = 18
                true, true, false, false, true, true, true, false, false, true, false,

                // A(3) = 19
                true, true, false, false, true, false, true, true, true, false, false,

                // A(4) = 20
                true, true, false, false, true, false, false, true, true, true, false,

                // A(5) = 21
                true, true, false, true, true, true, false, false, true, false, false,

                // A(6) = 22
                true, true, false, false, true, true, true, false, true, false, false,

                // A(7) = 23
                true, true, true, false, true, true, false, true, true, true, false,

                // A(8) = 24
                true, true, true, false, true, false, false, true, true, false, false,

                // check character = 59
                true, true, true, false, false, false, true, true, false, true, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "12345678";
            var c128 = new Code128Mapping
            {
                Optimize = false
            };

            Assert.True(c128.IsEncodable(s));

            ImmutableArray<bool> bars = c128.EncodeString(s);
            Assert.Equal((IEnumerable<bool>)barcode12345678, bars);
        }

        [Fact]
        public void NaiveEncodingOneTabTwoTabThree()
        {
            // (Wikipedia example)
            var barcode = new[]
            {
                // start A = 103
                true, true, false, true, false, false, false, false, true, false, false,

                // A(O) = 47
                true, false, false, false, true, true, true, false, true, true, false,

                // A(SwitchB) = 100
                true, false, true, true, true, true, false, true, true, true, false,

                // B(n) = 78
                true, true, false, false, false, false, true, false, true, false, false,

                // B(e) = 69
                true, false, true, true, false, false, true, false, false, false, false,

                // B(SwitchA) = 101
                true, true, true, false, true, false, true, true, true, true, false,

                // A(\t) = 73
                true, false, false, false, false, true, true, false, true, false, false,

                // A(T) = 52
                true, true, false, true, true, true, false, false, false, true, false,

                // A(SwitchB) = 100
                true, false, true, true, true, true, false, true, true, true, false,

                // B(w) = 87
                true, true, true, true, false, false, true, false, true, false, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(SwitchA) = 101
                true, true, true, false, true, false, true, true, true, true, false,

                // A(\t) = 73
                true, false, false, false, false, true, true, false, true, false, false,

                // A(T) = 52
                true, true, false, true, true, true, false, false, false, true, false,

                // A(SwitchB) = 100
                true, false, true, true, true, true, false, true, true, true, false,

                // B(h) = 72
                true, false, false, true, true, false, false, false, false, true, false,

                // B(r) = 82
                true, false, false, true, false, false, true, true, true, true, false,

                // B(e) = 69
                true, false, true, true, false, false, true, false, false, false, false,

                // B(e) = 69
                true, false, true, true, false, false, true, false, false, false, false,

                // check character = 20
                true, true, false, false, true, false, false, true, true, true, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "One\tTwo\tThree";
            var c128 = new Code128Mapping
            {
                Optimize = false
            };

            Assert.True(c128.IsEncodable(s));

            ImmutableArray<bool> bars = c128.EncodeString(s);
            Assert.Equal((IEnumerable<bool>)barcode, bars);
        }
    }
}
