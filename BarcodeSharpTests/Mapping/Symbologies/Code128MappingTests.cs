using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using FluentAssertions;
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
            checksum.Should().Be(54);
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

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcodePJJ123C);
        }

        [Fact]
        public void NaiveEncoding12345678()
        {
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

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode12345678);
        }

        [Fact]
        public void NaiveEncodingOneTabTwoTabThree()
        {
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

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void Optimized12345678()
        {
            var barcode12345678 = new[]
            {
                // start C = 105
                true, true, false, true, false, false, true, true, true, false, false,

                // C(12) = 12
                true, false, true, true, false, false, true, true, true, false, false,

                // C(34) = 34
                true, false, false, false, true, false, true, true, false, false, false,

                // C(56) = 56
                true, true, true, false, false, false, true, false, true, true, false,

                // C(78) = 78
                true, true, false, false, false, false, true, false, true, false, false,

                // check character = 47
                true, false, false, false, true, true, true, false, true, true, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "12345678";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode12345678);
        }

        [Fact]
        public void OptimizedOneTabTwoTabThree()
        {
            var barcode = new[]
            {
                // start B = 104
                true, true, false, true, false, false, true, false, false, false, false,

                // B(O) = 47
                true, false, false, false, true, true, true, false, true, true, false,

                // B(n) = 78
                true, true, false, false, false, false, true, false, true, false, false,

                // B(e) = 69
                true, false, true, true, false, false, true, false, false, false, false,

                // B(ShiftA) = 98
                true, true, true, true, false, true, false, false, false, true, false,

                // A(\t) = 73
                true, false, false, false, false, true, true, false, true, false, false,

                // B(T) = 52
                true, true, false, true, true, true, false, false, false, true, false,

                // B(w) = 87
                true, true, true, true, false, false, true, false, true, false, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(ShiftA) = 98
                true, true, true, true, false, true, false, false, false, true, false,

                // A(\t) = 73
                true, false, false, false, false, true, true, false, true, false, false,

                // B(T) = 52
                true, true, false, true, true, true, false, false, false, true, false,

                // B(h) = 72
                true, false, false, true, true, false, false, false, false, true, false,

                // B(r) = 82
                true, false, false, true, false, false, true, true, true, true, false,

                // B(e) = 69
                true, false, true, true, false, false, true, false, false, false, false,

                // B(e) = 69
                true, false, true, true, false, false, true, false, false, false, false,

                // check character = 81
                true, false, false, true, false, true, true, true, true, false, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "One\tTwo\tThree";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void OptimizedLol9Lol()
        {
            // single digit: switch pointless
            var barcode = new[]
            {
                // start B = 104
                true, true, false, true, false, false, true, false, false, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // B(9) = 25
                true, true, true, false, false, true, false, true, true, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // check character = 6
                true, false, false, true, true, false, false, true, false, false, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "Lol9Lol";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void OptimizedLol98Lol()
        {
            // two digits: switch there and back has too much overhead
            var barcode = new[]
            {
                // start B = 104
                true, true, false, true, false, false, true, false, false, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // B(9) = 25
                true, true, true, false, false, true, false, true, true, false, false,

                // B(8) = 24
                true, true, true, false, true, false, false, true, true, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // check character = 16
                true, false, false, true, true, true, false, true, true, false, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "Lol98Lol";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void OptimizedLol987Lol()
        {
            // three digits: switch there and back has too much overhead
            var barcode = new[]
            {
                // start B = 104
                true, true, false, true, false, false, true, false, false, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // B(9) = 25
                true, true, true, false, false, true, false, true, true, false, false,

                // B(8) = 24
                true, true, true, false, true, false, false, true, true, false, false,

                // B(7) = 23
                true, true, true, false, true, true, false, true, true, true, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // check character = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "Lol987Lol";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void OptimizedLol9876Lol()
        {
            // four digits: switching makes as much sense as not
            var barcode = new[]
            {
                // start B = 104
                true, true, false, true, false, false, true, false, false, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // B(9) = 25
                true, true, true, false, false, true, false, true, true, false, false,

                // B(8) = 24
                true, true, true, false, true, false, false, true, true, false, false,

                // B(7) = 23
                true, true, true, false, true, true, false, true, true, true, false,

                // B(6) = 22
                true, true, false, false, true, true, true, false, true, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // check character = 88
                true, true, true, true, false, false, true, false, false, true, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "Lol9876Lol";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void OptimizedLol98765Lol()
        {
            // five digits: switching makes as much sense as not
            var barcode = new[]
            {
                // start B = 104
                true, true, false, true, false, false, true, false, false, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // B(9) = 25
                true, true, true, false, false, true, false, true, true, false, false,

                // B(8) = 24
                true, true, true, false, true, false, false, true, true, false, false,

                // B(7) = 23
                true, true, true, false, true, true, false, true, true, true, false,

                // B(6) = 22
                true, true, false, false, true, true, true, false, true, false, false,

                // B(5) = 21
                true, true, false, true, true, true, false, false, true, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // check character = 43
                true, false, true, true, false, false, false, true, true, true, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "Lol98765Lol";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void OptimizedLol987654Lol()
        {
            // six digits: switching makes sense
            var barcode = new[]
            {
                // start B = 104
                true, true, false, true, false, false, true, false, false, false, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // B(SwitchC) = 99
                true, false, true, true, true, false, true, true, true, true, false,

                // C(98) = 98
                true, true, true, true, false, true, false, false, false, true, false,

                // C(76) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // C(54) = 54
                true, true, true, false, true, false, true, true, false, false, false,

                // C(SwitchB) = 100
                true, false, true, true, true, true, false, true, true, true, false,

                // B(L) = 44
                true, false, false, false, true, true, false, true, true, true, false,

                // B(o) = 79
                true, false, false, false, true, true, true, true, false, true, false,

                // B(l) = 76
                true, true, false, false, true, false, true, false, false, false, false,

                // check character = 29
                true, true, true, false, false, true, true, false, false, true, false,

                // stop = 106
                true, true, false, false, false, true, true, true, false, true, false,

                // final bar
                true, true
            };

            string s = "Lol987654Lol";
            var c128 = new Code128Mapping
            {
                Optimize = true
            };

            c128.IsEncodable(s).Should().BeTrue();

            ImmutableArray<bool> bars = c128.EncodeString(s);
            bars.Should().Equal(barcode);
        }

        [Fact]
        public void OptimizationEarlyStopFuzz()
        {
            var optimized128 = new Code128Mapping { Optimize = true };
            var exhaustive128 = new Code128Mapping { Optimize = true, ForceExhaustiveOptimization = true };
            var mappableCharSet = new HashSet<char>();
            
            mappableCharSet.UnionWith(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_");
            mappableCharSet.UnionWith("\u0000\u0001\u0002\u0003\u0004\u0005\u0006\u0007\u0008\u0009\u000A\u000B\u000C\u000D\u000E\u000F");
            mappableCharSet.UnionWith("\u0010\u0011\u0012\u0013\u0014\u0015\u0016\u0017\u0018\u0019\u001A\u001B\u001C\u001D\u001E\u001F");
            mappableCharSet.UnionWith("`abcdefghijklmnopqrstuvwxyz{|}~\u007F");

            var mappableCharList = new List<char>(mappableCharSet);
            var rnd = new Random();

            int length = rnd.Next(19, 21);
            var buf = new StringBuilder(length);
            for (int character = 0; character < length; ++character)
            {
                buf.Append(mappableCharList[rnd.Next(mappableCharList.Count)]);
            }

            string encodeMe = buf.ToString();
            string hexy = string.Join(" ", encodeMe.Select(c => ((int)c).ToString("x2")));

            ImmutableArray<bool> optimizedBars = optimized128.EncodeString(encodeMe);
            ImmutableArray<bool> exhaustiveBars = exhaustive128.EncodeString(encodeMe);

            optimizedBars.Should().Equal(exhaustiveBars, $"because the optimized Code128 representation of 0x'{hexy}' should match the exhaustive one");
        }
    }
}
