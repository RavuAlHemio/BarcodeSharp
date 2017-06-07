using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace RavuAlHemio.BarcodeSharp
{
    public static class BarcodeUtils1D
    {
        /// <summary>
        /// Collects adjacent same-colored bars in the given barcode and returns their color and total width.
        /// </summary>
        /// <param name="bars">
        /// The bars of the barcode, as obtained from <see cref="IOneDimensionalBarcodeMapping.EncodeString"/>.
        /// </param>
        /// <returns>
        /// Tuples whose first item is the color of the bar and whose second item is the total width of that
        /// bar.
        /// </returns>
        public static IEnumerable<Tuple<bool, int>> BarWidths(ImmutableArray<bool> bars)
        {
            bool previousBar = false;
            int currentCount = 0;
            foreach (bool bar in bars)
            {
                if (bar == previousBar)
                {
                    ++currentCount;
                }
                else
                {
                    if (currentCount != 0)
                    {
                        yield return Tuple.Create(previousBar, currentCount);
                    }
                    previousBar = bar;
                    currentCount = 1;
                }
            }

            if (currentCount != 0)
            {
                yield return Tuple.Create(previousBar, currentCount);
            }
        }
    }
}
