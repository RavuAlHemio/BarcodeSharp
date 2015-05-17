using System;
using System.Collections.Generic;

namespace RavuAlHemio.BarcodeSharp
{
    internal static class BarcodeSharpUtils
    {
        public static IEnumerable<T> FollowedBy<T>(this IEnumerable<T> first, IEnumerable<T> other)
        {
            foreach (var firstElement in first)
            {
                yield return firstElement;
            }
            foreach (var otherElement in other)
            {
                yield return otherElement;
            }
        }

        /// <summary>
        /// Returns an Enumerable of indices where a sublist appears in the list, comparing elements using a custom
        /// comparison function.
        /// </summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <param name="haystack">The list to search for occurrences of <paramref name="needle"/>.</param>
        /// <param name="needle">The sublist for whose occurrences in the list to search.</param>
        /// <param name="equalityComparer">A function used to compare two list values for equality.</param>
        /// <returns>The indices at which <paramref name="needle"/> appears in the list.</returns>
        public static IEnumerable<int> SublistFoundIndices<T>(this IList<T> haystack, IList<T> needle, Func<T, T, bool> equalityComparer)
        {
            if (needle.Count > haystack.Count)
            {
                // nope
                yield break;
            }

            for (int i = 0; i < haystack.Count - needle.Count + 1; ++i)
            {
                bool matched = true;
                // ReSharper disable once LoopCanBeConvertedToQuery
                for (int j = 0; j < needle.Count; ++j)
                {
                    if (!equalityComparer(haystack[i + j], needle[j]))
                    {
                        matched = false;
                        break;
                    }
                }

                if (matched)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Returns an Enumerable of indices where a sublist appears in the list, comparing elements using
        /// <see cref="Object.Equals(Object)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the list.</typeparam>
        /// <param name="haystack">The list to search for occurrences of <paramref name="needle"/>.</param>
        /// <param name="needle">The sublist for whose occurrences in the list to search.</param>
        /// <returns>The indices at which <paramref name="needle"/> appears in the list.</returns>
        public static IEnumerable<int> SublistFoundIndices<T>(this IList<T> haystack, IList<T> needle)
        {
            return SublistFoundIndices(haystack, needle, (l, r) => l.Equals(r));
        }
    }
}
