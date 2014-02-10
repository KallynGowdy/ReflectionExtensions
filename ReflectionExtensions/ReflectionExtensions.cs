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

        public static IProperty Wrap(this PropertyInfo property)
        {
            return new PropertyWrapper(property);
        }

        public static IMethod Wrap(this MethodInfo method)
        {
            return new MethodWrapper(method);
        }

        public static IField Wrap(this FieldInfo field)
        {
            return new FieldWrapper(field);
        }

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
            else if (member is MethodInfo)
            {
                return new MethodWrapper((MethodInfo)member);
            }
            return null;
        }
    }
}
