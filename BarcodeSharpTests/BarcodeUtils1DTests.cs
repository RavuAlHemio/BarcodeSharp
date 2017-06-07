using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using RavuAlHemio.BarcodeSharp;
using Xunit;

namespace BarcodeSharpTests
{
    public class BarcodeUtils1DTests
    {
        [Fact]
        public void TestEmptyBarWidths()
        {
            List<Tuple<bool, int>> barWidths = BarcodeUtils1D.BarWidths(ImmutableArray<bool>.Empty)
                .ToList();
            barWidths.Should().BeEmpty();
        }

        [Theory]
        [InlineData(true), InlineData(false)]
        public void TestSingleBarWidths(bool value)
        {
            List<Tuple<bool, int>> barWidths = BarcodeUtils1D.BarWidths(ImmutableArray.Create(value))
                .ToList();
            barWidths.Should().HaveCount(1);
            barWidths[0].Should().Be(Tuple.Create(value, 1));
        }

        [Theory]
        [InlineData(true, false), InlineData(false, true)]
        public void TestZebraBarWidths(bool oneValue, bool otherValue)
        {
            ImmutableArray<bool> bars = ImmutableArray.Create(
                oneValue, otherValue, oneValue, otherValue, oneValue, otherValue
            );
            List<Tuple<bool, int>> barWidths = BarcodeUtils1D.BarWidths(bars).ToList();

            barWidths.Should().HaveCount(6);
            barWidths[0].Should().Be(Tuple.Create(oneValue, 1));
            barWidths[1].Should().Be(Tuple.Create(otherValue, 1));
            barWidths[2].Should().Be(Tuple.Create(oneValue, 1));
            barWidths[3].Should().Be(Tuple.Create(otherValue, 1));
            barWidths[4].Should().Be(Tuple.Create(oneValue, 1));
            barWidths[5].Should().Be(Tuple.Create(otherValue, 1));
        }

        [Theory]
        [InlineData(true, false), InlineData(false, true)]
        public void TestDoubleZebraBarWidths(bool oneValue, bool otherValue)
        {
            ImmutableArray<bool> bars = ImmutableArray.Create(
                oneValue, oneValue, otherValue, otherValue,
                oneValue, oneValue, otherValue, otherValue,
                oneValue, oneValue, otherValue, otherValue
            );
            List<Tuple<bool, int>> barWidths = BarcodeUtils1D.BarWidths(bars).ToList();

            barWidths.Should().HaveCount(6);
            barWidths[0].Should().Be(Tuple.Create(oneValue, 2));
            barWidths[1].Should().Be(Tuple.Create(otherValue, 2));
            barWidths[2].Should().Be(Tuple.Create(oneValue, 2));
            barWidths[3].Should().Be(Tuple.Create(otherValue, 2));
            barWidths[4].Should().Be(Tuple.Create(oneValue, 2));
            barWidths[5].Should().Be(Tuple.Create(otherValue, 2));
        }

        [Fact]
        public void TestWikipediaSampleBarcode()
        {
            // sample barcode (UPC): 0 36000 29145 2
            ImmutableArray<bool> bars = ImmutableArray.Create(
                // start
                true, false, true,

                // left 0
                false, false, false, true, true, false, true,

                // left 3
                false, true, true, true, true, false, true,

                // left 6
                false, true, false, true, true, true, true,

                // left 0 x3
                false, false, false, true, true, false, true,
                false, false, false, true, true, false, true,
                false, false, false, true, true, false, true,

                // middle
                false, true, false, true, false,

                // right 2
                true, true, false, true, true, false, false,

                // right 9
                true, true, true, false, true, false, false,

                // right 1
                true, true, false, false, true, true, false,

                // right 4
                true, false, true, true, true, false, false,

                // right 5
                true, false, false, true, true, true, false,

                // right 2
                true, true, false, true, true, false, false,

                // stop
                true, false, true
            );
            ImmutableArray<Tuple<bool, int>> expectedBarWidths = ImmutableArray.Create(
                // start
                Tuple.Create( true, 1), Tuple.Create(false, 1), Tuple.Create( true, 1),

                // left 0
                Tuple.Create(false, 3), Tuple.Create( true, 2), Tuple.Create(false, 1), Tuple.Create( true, 1),

                // left 3
                Tuple.Create(false, 1), Tuple.Create( true, 4), Tuple.Create(false, 1), Tuple.Create( true, 1),

                // left 6
                Tuple.Create(false, 1), Tuple.Create( true, 1), Tuple.Create(false, 1), Tuple.Create( true, 4),

                // left 0 x3
                Tuple.Create(false, 3), Tuple.Create( true, 2), Tuple.Create(false, 1), Tuple.Create( true, 1),
                Tuple.Create(false, 3), Tuple.Create(true, 2), Tuple.Create(false, 1), Tuple.Create(true, 1),
                Tuple.Create(false, 3), Tuple.Create(true, 2), Tuple.Create(false, 1), Tuple.Create(true, 1),

                // middle
                Tuple.Create(false, 1), Tuple.Create( true, 1), Tuple.Create(false, 1), Tuple.Create(true, 1), Tuple.Create(false, 1),

                // right 2
                Tuple.Create( true, 2), Tuple.Create(false, 1), Tuple.Create( true, 2), Tuple.Create(false, 2),

                // right 9
                Tuple.Create( true, 3), Tuple.Create(false, 1), Tuple.Create( true, 1), Tuple.Create(false, 2),

                // right 1
                Tuple.Create( true, 2), Tuple.Create(false, 2), Tuple.Create( true, 2), Tuple.Create(false, 1),

                // right 4
                Tuple.Create( true, 1), Tuple.Create(false, 1), Tuple.Create( true, 3), Tuple.Create(false, 2),

                // right 5
                Tuple.Create( true, 1), Tuple.Create(false, 2), Tuple.Create( true, 3), Tuple.Create(false, 1),

                // right 2
                Tuple.Create( true, 2), Tuple.Create(false, 1), Tuple.Create( true, 2), Tuple.Create(false, 2),

                // stop
                Tuple.Create( true, 1), Tuple.Create(false, 1), Tuple.Create( true, 1)
            );
            List<Tuple<bool, int>> actualBarWidths = BarcodeUtils1D.BarWidths(bars).ToList();

            actualBarWidths.Should().Equal(expectedBarWidths);
        }
    }
}
