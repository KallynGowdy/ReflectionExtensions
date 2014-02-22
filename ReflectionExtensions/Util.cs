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
        [ContractArgumentValidator]
        internal static void ThrowIfNull(this object obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
            Contract.EndContractBlock();
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
            Contract.Requires<ArgumentNullException>(first != null, "first");
            Contract.Requires<ArgumentNullException>(second != null, "second");
            Contract.Requires<ArgumentNullException>(filter != null, "filter");

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
            Contract.Requires<ArgumentNullException>(first != null, "first");
            Contract.Requires<ArgumentNullException>(second != null, "second");
            Contract.Requires<ArgumentNullException>(comparer != null, "comparer");

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
            Contract.Requires<ArgumentNullException>(values != null, "values");
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

        /// <summary>
        /// Gets the access modifier that describes the access that other classes have to the given member.
        /// </summary>
        /// <param name="member">The member to get the access modifiers for.</param>
        /// <returns>Returns the access modifiers that the member has.</returns>
        internal static AccessModifier GetAccessModifiers(this MethodBase member)
        {
            Contract.Requires<ArgumentNullException>(member != null, "member");
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
    }
}
