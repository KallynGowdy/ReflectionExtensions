﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    internal static class Util
    {
        /// <summary>
        /// Throws a new System.ArgumentNullException if the given object is null.
        /// </summary>
        /// <param name="obj">The object to check for validity.</param>
        /// <param name="paramName">The name of the parameter that is being checked.</param>
        internal static void ThrowIfNull(this object obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Filters the first sequence by comparing it to the second sequence using a given filter.
        /// </summary>
        /// <typeparam name="TFirst">The type of the objects in the first enumerable list.</typeparam>
        /// <typeparam name="TSecond">The type of the objects in the second enumerable list.</typeparam>
        /// <param name="first">The list that should be filtered.</param>
        /// <param name="second">The list that should be used to filter the first.</param>
        /// <param name="filter">A function that, given and object from the first list and an object from the second, returns whether the element should be included in the output.</param>
        /// <returns>Returns an enumerable list of <typeparamref name="TFirst"/> objects that was filtered element-by-element using the given function.</returns>
        internal static IEnumerable<TFirst> WhereSequence<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> filter)
        {
            IEnumerator<TFirst> fEnumerator = first.GetEnumerator();
            IEnumerator<TSecond> sEnumerator = second.GetEnumerator();

            while (fEnumerator.MoveNext() && sEnumerator.MoveNext())
            {
                if (filter(fEnumerator.Current, sEnumerator.Current))
                {
                    yield return fEnumerator.Current;
                }
            }
        }

        /// <summary>
        /// Determines if the two given sequences are equal using the given comparer.
        /// Equality is determined in-order, that is two lists are considered equal if the first elements of each list are equal and if the second elements are equal and so on.
        /// </summary>
        /// <typeparam name="TFirst">The type of the objects in the first sequence.</typeparam>
        /// <typeparam name="TSecond">The type of the objects in the second sequence.</typeparam>
        /// <param name="first">An enumerable list of objects of the type <typeparamref name="TFirst"/> that should be compared to the second.</param>
        /// <param name="second">An enumerable list of objects of the type <typeparamref name="TSecond"/> that should be compared to the first.</param>
        /// <param name="comparer">A function that, given an object from the first list and an object from the second determines if the two should be considered equal.</param>
        /// <returns></returns>
        internal static bool SequenceEqual<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> comparer)
        {
            IEnumerator<TFirst> fEnumerator = first.GetEnumerator();
            IEnumerator<TSecond> sEnumerator = second.GetEnumerator();
            if (first.Count() == second.Count())
            {
                while (fEnumerator.MoveNext() && sEnumerator.MoveNext())
                {
                    if (!comparer(fEnumerator.Current, sEnumerator.Current))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calculates the hash code of the given values.
        /// </summary>
        /// <param name="values">The values to calculate the hash code of.</param>
        /// <returns></returns>
        internal static int HashCode(params object[] values)
        {
            unchecked
            {
                int hash = 17;

                foreach (object val in values)
                {
                    hash = hash * 23 + val.GetHashCode();
                }

                return hash;
            }
        }
    }
}