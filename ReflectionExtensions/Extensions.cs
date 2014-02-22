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
    /// <summary>
    /// Defines a class that contains a set of extension helper methods for use in reflection.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Wraps the given type in a new ReflectionExtensions.IType object that provides useful functionality.
        /// </summary>
        /// <param name="type">The type to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IType object.</returns>
        public static IType Wrap(this Type type)
        {
            if (type == null)
            {
                return null;
            }
            else
            {
                return new TypeWrapper(type);
            }
        }

        /// <summary>
        /// Wraps the given System.Reflection.PropertyInfo object into a new ReflectionExtensions.IProperty object.
        /// </summary>
        /// <param name="property">The property to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IProperty object.</returns>
        public static IProperty Wrap(this PropertyInfo property)
        {
            return new PropertyWrapper(property);
        }

        /// <summary>
        /// Wraps the given System.Reflection.MethodBase object into a new ReflectionExtensions.IMethod object.
        /// </summary>
        /// <param name="method">The method to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IMethod object.</returns>
        public static IMethod Wrap(this MethodBase method)
        {
            Contract.Requires(method != null, "method");
            if (method.IsGenericMethod && method is MethodInfo)
            {
                return new GenericMethodWrapper((MethodInfo)method);
            }
            else
            {
                return new NonGenericMethodWrapper(method);
            }
        }

        /// <summary>
        /// Wraps the given System.Reflection.FieldInfo object into a new ReflectionExtensions.IField object.
        /// </summary>
        /// <param name="field">The field to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IField object.</returns>
        public static IField Wrap(this FieldInfo field)
        {
            return new FieldWrapper(field);
        }

        /// <summary>
        /// Wraps the given System.Reflection.ParameterInfo object into a new ReflectionExtensions.IParameter object.
        /// </summary>
        /// <param name="parameter">The parameter to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IParameter object.</returns>
        public static IParameter Wrap(this ParameterInfo parameter)
        {
            return new PrameterWrapper(parameter);
        }

        /// <summary>
        /// Wraps the given System.Reflection.MemberInfo object into a new ReflectionExtensions.IMember object.
        /// </summary>
        /// <param name="member">The member to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IMember object.</returns>
        public static IMember Wrap(this MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).Wrap();
            }
            else if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).Wrap();
            }
            else if (member is MethodBase)
            {
                return ((MethodBase)member).Wrap();
            }
            return null;
        }

        /// <summary>
        /// Gets a method from the type that has the given name with arguments of the given types.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument that the method takes.</typeparam>
        /// <param name="type">The type that the method should be retrieved from.</param>
        /// <param name="name">The case-sensitive name of the method to retreive.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the given type or string is null.</exception>
        /// <returns>Returns a ReflectionExtensions.IMethod object that represents the retrieved method. Returns null if the method could not be
        /// found.</returns>
        public static IMethod GetMethod<T1>(this IType type, string name)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Requires<ArgumentNullException>(name != null, "name");

            return GetMethod(type, name, new[] { typeof(T1) });
        }

        /// <summary>
        /// Gets a method from the type that has the given name with arguments of the given types.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument that the method takes.</typeparam>
        /// <typeparam name="T2">The type of the second argument that the method takes.</typeparam>
        /// <param name="type">The type that the method should be retrieved from.</param>
        /// <param name="name">The case-sensitive name of the method to retrieve.</param>
        /// <exception cref="System.ArgumentNullException">Thrown </exception>
        /// <returns>Returns a ReflectionExtensions.IMethod object that represents the retrieved method. Returns null if the method could not be found.</returns>
        public static IMethod GetMethod<T1, T2>(this IType type, string name)
        {
            Contract.Requires<ArgumentNullException>(type != null, "The given type must not be null");
            Contract.Requires<ArgumentNullException>(name != null, "The given name must not be null");

            return GetMethod(type, name, new[] { typeof(T1), typeof(T2) });
        }

        /// <summary>
        /// Gets a list of methods whose names equal the given name.
        /// </summary>
        /// <param name="name">The case-sensitive name of the methods to retrive.</param>
        /// <param name="methods">The list of methods to filter by name.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the given list is null or if the given name is null.</exception>
        /// <returns>Returns all of the methods from the current list whose name equals the given name.</returns>
        public static IEnumerable<IMethod> WithName(this IEnumerable<IMethod> methods, string name)
        {
            Contract.Requires(methods != null);
            Contract.Requires(name != null);

            var enumerator = methods.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null && enumerator.Current.Name.Equals(name))
                {
                    yield return enumerator.Current;
                }
            }
        }

        private static IMethod GetMethod(IType type, string name, Type[] parameterTypes)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Requires<ArgumentNullException>(name != null, "name");

            return type.Methods.Where(m => m.Name.Equals(name)).SingleOrDefault(m => m.Parameters.SequenceEqual(parameterTypes, (p, t) => p.ReturnType.Equals(t)));
        }
    }
}
