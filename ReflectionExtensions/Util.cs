using System;
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

        internal static bool SequenceEqual<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, bool> comparer)
        {
            IEnumerator<TFirst> fEnumerator = first.GetEnumerator();
            IEnumerator<TSecond> sEnumerator = second.GetEnumerator();

            while (fEnumerator.MoveNext() && sEnumerator.MoveNext())
            {
                if (!comparer(fEnumerator.Current, sEnumerator.Current))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
