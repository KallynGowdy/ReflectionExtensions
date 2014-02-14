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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class that contains a set of extension helper methods for use in reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Wraps the given type in a new ReflectionExtensions.IType object that provides usefull functionality.
        /// </summary>
        /// <param name="type">The type to wrap.</param>
        /// <returns>Returns a new ReflectionExtensions.IType object.</returns>
        public static IType Wrap(this Type type)
        {
            return new TypeWrapper(type);
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
            return new MethodWrapper(method);
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
                return new FieldWrapper((FieldInfo) member);
            }
            else if (member is PropertyInfo)
            {
                return new PropertyWrapper((PropertyInfo)member);
            }
            else if (member is MethodBase)
            {
                return new MethodWrapper((MethodBase)member);
            }
            return null;
        }

        /// <summary>
        /// Gets a method from the type that has the given name with the given argument.
        /// </summary>
        /// <typeparam name="T1">The type of the first argument that the method takes.</typeparam>
        /// <param name="type">The type that the method should be retrieved from.</param>
        /// <param name="name">The case-sensitive name of the method to retreive.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the given type or string is null.</exception>
        /// <returns>Returns a ReflectionExtensions.IMethod object that represents the retrieved method. Returns null if the method could not be
        /// found.</returns>
        public static IMethod GetMethod<T1>(this IType type, string name)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            else if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            return GetMethod(type, name, new[] { typeof(T1) });
        }

        private static IMethod GetMethod(IType type, string name, Type[] parameterTypes)
        {
            foreach (var method in type.GetMethods(name))
            {
                var parameters = method.Parameters.ToArray();
                if (parameters.Length == parameterTypes.Length)
                {
                    bool good = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (!parameters[i].ReturnType.IsAssignableFrom(parameterTypes[i]))
                        {
                            good = false;
                            break;
                        }
                    }
                    if (good)
                    {
                        return method;
                    }
                }
            }
            return null;
        }
    }
}
