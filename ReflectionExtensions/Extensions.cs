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
        /// Invokes this method using the given object's members as arugments and infering the generic arguments from those.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">An object whose member's names match the names of the parameters.</param>
        /// <returns>
        /// Returns the value returned from the method as an object.
        /// </returns>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if one of the given generic arguments does not match a contstraint.</exception>
        public static object Invoke(this IGenericMethod method, object reference, object arguments)
        {
            Contract.Requires(method != null);
            Contract.Requires(reference != null);

            return method.Invoke<object>(reference, arguments);
        }

        /// <summary>
        /// Invokes this method using the given object's members as arugments and infering the generic arguments from those.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">An object whose member's names match the names of the parameters.</param>
        /// <returns>
        /// Returns the value returned from the method as an object.
        /// </returns>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if one of the given generic arguments does not match a contstraint.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static TReturn Invoke<TReturn>(this IGenericMethod method, object reference, object arguments)
        {
            Contract.Requires(method != null);
            Contract.Requires(reference != null);

            if (arguments == null)
            {
                return method.Invoke<TReturn>(reference, (object[])null);
            }
            else
            {
                //Match up each argument to a parameter
                IType argumentsType = arguments.GetType().Wrap();

                var argsAndParams = argumentsType.StorageMembers.Where(a => a.CanRead).GroupJoin(method.Parameters, a => a.Name, p => p.Name, (a, p) => new {Argument=a[arguments], Parameter = p.First()}).OrderBy(ap => ap.Parameter.Position);

                return method.Invoke<TReturn>(reference, argsAndParams.Select(a => a.Argument).ToArray(), default(TReturn));
            }
        }

        /// <summary>
        /// Invokes this method using the given objects as arugments and infering the generic arguments from the given arguments.
        /// </summary>
        /// <param name="method">The method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods in signature.</param>
        /// <returns>
        /// Returns the value returned from the method as an object.
        /// </returns>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if one of the given generic arguments does not match a contstraint.</exception>
        public static object Invoke(this IGenericMethod method, object reference, object[] arguments)
        {
            Contract.Requires(method != null);
            Contract.Requires(reference != null);
            Contract.Requires(arguments != null);
            return method.Invoke<object>(reference, arguments, null);
        }

        /// <summary>
        /// Invokes this method using the given objects as arugments and infering the generic arguments from the given arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="method">The method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods in signature.</param>
        /// <param name="defaultValue">The value to return if the returned value from the invocation was void or null.</param>
        /// <returns>
        /// Returns the value returned from the method cast into the given type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference is null.</exception>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentException">Thrown if one of the given generic arguments does not match a contstraint.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2")]
        public static TReturn Invoke<TReturn>(this IGenericMethod method, object reference, object[] arguments, TReturn defaultValue)
        {
            Contract.Requires(method != null);
            Contract.Requires(reference != null);
            Contract.Requires(arguments != null);
            Contract.Requires(arguments.Length >= method.GenericParameters.Count());
            
            Dictionary<IGenericParameter, Type> genericArgsNamesToTypes = new Dictionary<IGenericParameter, Type>();

            var paramsAndArgs = method.Parameters.Zip(arguments, (p, a) => new { Parameter = p, Argument = a });

            foreach (var paramArg in paramsAndArgs)
            {
                if (paramArg.Argument == null && paramArg.Parameter.ReturnType.IsValueType)
                {
                    throw new ArgumentNullException(paramArg.Parameter.Name);
                }
                else
                {
                    IGenericParameter genericParam = method.GenericParameters.FirstOrDefault(p => p.Name.Equals(paramArg.Parameter.ReturnType.Name));
                    if (genericParam != null)
                    {
                        if (!genericArgsNamesToTypes.ContainsKey(genericParam))
                        {
                            genericArgsNamesToTypes.Add(genericParam, paramArg.Argument.GetType());
                        }
                    }
                }
            }
            return method.Invoke<TReturn>(reference, genericArgsNamesToTypes.OrderBy(a => a.Key.Position).Select(a => a.Value).ToArray(), arguments, defaultValue);
        }

        /// <summary>
        /// Wraps the given type in a new ReflectionExtensions.IType object that provides useful functionality.
        /// </summary>
        /// <param name="type">The type to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IType object.</returns>
        [Pure]
        public static IType Wrap(this Type type)
        {
            if (type == null)
            {
                return null;
            }
            else if (type.IsGenericType && type.ContainsGenericParameters)
            {
                return new GenericTypeWrapper(type);
            }
            else
            {
                return new NonGenericTypeWrapper(type);
            }
        }

        /// <summary>
        /// Wraps the given System.Reflection.PropertyInfo object into a new ReflectionExtensions.IProperty object.
        /// </summary>
        /// <param name="property">The property to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IProperty object.</returns>
        [Pure]
        public static IProperty Wrap(this PropertyInfo property)
        {
            return new PropertyWrapper(property);
        }

        /// <summary>
        /// Wraps the given System.Reflection.MethodBase object into a new ReflectionExtensions.IMethod object.
        /// </summary>
        /// <param name="method">The method to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IMethod object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [Pure]
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
        [Pure]
        public static IField Wrap(this FieldInfo field)
        {
            return new FieldWrapper(field);
        }

        /// <summary>
        /// Wraps the given System.Reflection.ParameterInfo object into a new ReflectionExtensions.IParameter object.
        /// </summary>
        /// <param name="parameter">The parameter to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IParameter object.</returns>
        [Pure]
        public static IParameter Wrap(this ParameterInfo parameter)
        {
            return new ParameterWrapper(parameter);
        }

        /// <summary>
        /// Wraps the given System.Reflection.MemberInfo object into a new ReflectionExtensions.IMember object.
        /// </summary>
        /// <param name="member">The member to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IMember object.</returns>
        [Pure]
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
        /// Gets a list of methods whose names equal the given name.
        /// </summary>
        /// <param name="name">The case-sensitive name of the methods to retrive.</param>
        /// <param name="methods">The list of methods to filter by name.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the given list is null or if the given name is null.</exception>
        /// <returns>Returns all of the methods from the current list whose name equals the given name.</returns>
        [Pure]
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

        /// <summary>
        /// Gets a list of methods from the current list of methods whose parameter's types match the given list of parameter types.
        /// </summary>
        /// <param name="methods">The list of methods to search through.</param>
        /// <param name="parameterTypes">A list of Types that define what the parameter types are required.</param>
        /// <returns>Returns an enumerable list of methods that contain the given signature.</returns>
        [Pure]
        public static IEnumerable<IMethod> WithParameters(this IEnumerable<IMethod> methods, params Type[] parameterTypes)
        {
            Contract.Requires(methods != null);
            Contract.Requires(parameterTypes != null);

            var enumerator = methods.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null && enumerator.Current.Parameters.SequenceEqual(parameterTypes, (p, t) => p.ReturnType.Equals(t)))
                {
                    yield return enumerator.Current;
                }
            }
        }

        /// <summary>
        /// Gets a method from the given type that has the given name and signature.
        /// </summary>
        /// <param name="methods">The methods to retrieve the method from.</param>
        /// <param name="name">The name of the method to retrieve.</param>
        /// <param name="parameterTypes">A list of types that match the required types of the parameters of the method to retrieve.</param>
        /// <returns>Returns the method that contains the specified signature.</returns>
        [Pure]
        public static IMethod WithSignature(this IEnumerable<IMethod> methods, string name, params Type[] parameterTypes)
        {
            Contract.Requires(methods != null, "type");
            Contract.Requires(name != null, "name");

            return methods.Where(m => m.Name.Equals(name)).SingleOrDefault(m => m.Parameters.SequenceEqual(parameterTypes, (p, t) => p.ReturnType.Equals(t)));
        }
    }
}
