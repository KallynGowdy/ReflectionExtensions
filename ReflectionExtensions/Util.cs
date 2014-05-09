// Copyright 2014 Kallyn Gowdy
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an internal static class that provides serveral utility methods.
    /// </summary>
    internal static class Util
    {
        /// <summary>
        /// Filters the first sequence by comparing it to the second sequence using a given filter.
        /// </summary>
        /// <typeparam name="TFirst">The type of the objects in the first enumerable list.</typeparam>
        /// <typeparam name="TSecond">The type of the objects in the second enumerable list.</typeparam>
        /// <param name="first">The list that should be filtered.</param>
        /// <param name="second">The list that should be used to filter the first.</param>
        /// <param name="filter">A function that, given and object from the first list and an object from the second, returns whether the element should be included in the output.</param>
        /// <returns>Returns an enumerable list of <typeparamref name="TFirst"/> objects that was filtered element-by-element using the given function.</returns>
        [Pure]
        internal static IEnumerable<TFirst> WhereSequence<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> filter)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

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
        /// <returns>Returns whether the two sequences are equal as determined by the given comparer.</returns>
        [Pure]
        internal static bool SequenceEqual<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> comparer)
        {
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }

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
        /// <param name="primeNumber">The prime number that should be used as the starting point for the hash code.</param>
        /// <param name="values">The values to calculate the hash code of.</param>
        /// <returns></returns>
        [Pure]
        internal static int HashCode(int primeNumber, params object[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            unchecked
            {
                int hash = primeNumber;

                foreach (object val in values)
                {
                    if (val != null)
                        hash = hash * primeNumber + val.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Gets the access modifier that describes the access that other classes have to the given member.
        /// </summary>
        /// <param name="member">The member to get the access modifiers for.</param>
        /// <returns>Returns the access modifiers that the member has.</returns>
        [Pure]
        internal static AccessModifier GetAccessModifiers(this MethodBase member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }
            if (member.IsPublic)
            {
                return AccessModifier.Public;
            }
            else if (member.IsPrivate)
            {
                return AccessModifier.Private;
            }
            else if (member.IsFamily)
            {
                return AccessModifier.Protected;
            }
            else if (member.IsFamilyAndAssembly)
            {
                return AccessModifier.ProtectedAndInternal;
            }
            else if (member.IsAssembly)
            {
                return AccessModifier.Internal;
            }
            else
            {
                return AccessModifier.ProtectedOrInternal;
            }
        }

        /// <summary>
        /// Gets the access modifiers that are present on the given type.
        /// </summary>
        /// <param name="type">The type for which the access modifiers should be retrieved.</param>
        /// <returns>Returns the access modifiers that the member has.</returns>
        [Pure]
        internal static AccessModifier GetAccessModifiers(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (type.IsNested)
            {
                if (type.IsNestedPublic)
                {
                    return AccessModifier.Public;
                }
                else if (type.IsNestedPrivate)
                {
                    return AccessModifier.Private;
                }
                else if (type.IsNestedFamily)
                {
                    return AccessModifier.Protected;
                }
                else if (type.IsNestedFamANDAssem)
                {
                    return AccessModifier.ProtectedAndInternal;
                }
                else if (type.IsNestedAssembly)
                {
                    return AccessModifier.Internal;
                }
                else
                {
                    return AccessModifier.ProtectedOrInternal;
                }
            }
            else
            {
                return type.IsPublic ? AccessModifier.Public : AccessModifier.Internal;
            }
        }
    }
}
