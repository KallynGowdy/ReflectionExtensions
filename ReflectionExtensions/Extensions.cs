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
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }

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
        public static TReturn Invoke<TReturn>(this IGenericMethod method, object reference, object arguments)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }

            if (arguments == null)
            {
                return method.Invoke<TReturn>(reference, (object[])null);
            }
            else
            {
                //Match up each argument to a parameter
                IType argumentsType = arguments.GetType().Wrap();

                var argsAndParams = argumentsType.StorageMembers.Where(a => a.CanRead).GroupJoin(method.Parameters, a => a.Name, p => p.Name, (a, p) => new { Argument = a[arguments], Parameter = p.First() }).OrderBy(ap => ap.Parameter.Position);

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
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
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
        public static TReturn Invoke<TReturn>(this IGenericMethod method, object reference, object[] arguments, TReturn defaultValue)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }
            if (arguments.Length < method.GenericParameters.Count())
            {
                throw new ArgumentException("The given number of generic arguments must be greater than or equal to the number of generic arguments required.", "arguments");
            }

            Dictionary<IGenericParameter, Type> genericArgsNamesToTypes = new Dictionary<IGenericParameter, Type>();

            var paramsAndArgs = method.Parameters.Zip(arguments, (p, a) => new { Parameter = p, Argument = a });

            foreach (var paramArg in paramsAndArgs)
            {
                if (paramArg.Argument == null && paramArg.Parameter.ReturnType.IsStruct)
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
        /// <returns>Returns a <see cref="ReflectionExtensions.IType"/> object. This object is created if the current type has not been wrapped before, otherwise it is retrieved from the cache.</returns>
        [Pure]
        public static IType Wrap(this Type type)
        {
            IType t;
            if (type == null)
            {
                return null;
            }
            else if (ReflectedCache.Types.TryGetValue(type, out t))
            {
                return t;
            }
            else if (type.IsGenericType && type.ContainsGenericParameters)
            {
                t = new GenericTypeWrapper(type);
            }
            else
            {
                t = new NonGenericTypeWrapper(type);
            }
            ReflectedCache.Types.TryAdd(type, t);
            return t;
        }

        /// <summary>
        /// Wraps the given System.Reflection.PropertyInfo object into a new ReflectionExtensions.IProperty object.
        /// </summary>
        /// <param name="property">The property to wrap.</param>
        /// <returns>Returns a ReflectionExtensions.IProperty object. This object is retrieved from the cache if it has been wrapped before, otherwise it is a new IProperty object.</returns>
        [Pure]
        public static IProperty Wrap(this PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            IProperty prop;
            if (ReflectedCache.Properties.TryGetValue(property, out prop))
            {
                return prop;
            }
            prop = new PropertyWrapper(property);
            ReflectedCache.Properties.TryAdd(property, prop);
            return prop;
        }

        /// <summary>
        /// Wraps the given System.Reflection.MethodBase object into a new ReflectionExtensions.IMethod object.
        /// </summary>
        /// <param name="method">The method to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IMethod object.</returns>
        [Pure]
        public static IMethod Wrap(this MethodBase method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            IMethod m;
            if (ReflectedCache.Methods.TryGetValue(method, out m))
            {
                return m;
            }
            else if (method.IsGenericMethod && method is MethodInfo)
            {
                m = new GenericMethodWrapper((MethodInfo)method);
            }
            else
            {
                m = new NonGenericMethodWrapper(method);
            }
            ReflectedCache.Methods.TryAdd(method, m);
            return m;
        }

        /// <summary>
        /// Wraps the given System.Reflection.FieldInfo object into a new ReflectionExtensions.IField object.
        /// </summary>
        /// <param name="field">The field to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IField object.</returns>
        [Pure]
        public static IField Wrap(this FieldInfo field)
        {
            IField f;
            if (ReflectedCache.Fields.TryGetValue(field, out f))
            {
                return f;
            }
            f = new FieldWrapper(field);
            ReflectedCache.Fields.TryAdd(field, f);
            return f;
        }

        /// <summary>
        /// Wraps the given System.Reflection.ParameterInfo object into a new ReflectionExtensions.IParameter object.
        /// </summary>
        /// <param name="parameter">The parameter to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IParameter object.</returns>
        [Pure]
        public static IParameter Wrap(this ParameterInfo parameter)
        {
            IParameter p;
            if (ReflectedCache.Parameters.TryGetValue(parameter, out p))
            {
                return p;
            }
            p = new ParameterWrapper(parameter);
            ReflectedCache.Parameters.TryAdd(parameter, p);
            return p;
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
            if (methods == null)
            {
                throw new ArgumentNullException("methods");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
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
            if (methods == null)
            {
                throw new ArgumentNullException("methods");
            }
            if (parameterTypes == null)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            var enumerator = methods.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null && enumerator.Current.Parameters.SequenceEqual(parameterTypes, (p, t) => p.ReturnType.Equals(t.Wrap())))
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
            if (methods == null)
            {
                throw new ArgumentNullException("methods");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return methods.Where(m => m.Name.Equals(name)).SingleOrDefault(m => m.Parameters.SequenceEqual(parameterTypes, (p, t) => p.ReturnType.Equals(t.Wrap())));
        }
    }
}
