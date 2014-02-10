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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class around a MethodInfo object.
    /// </summary>
    public class MethodWrapper : IMethod
    {

        /// <summary>
        /// Creates a new wrapper around the given method.
        /// </summary>
        /// <param name="method">The method to augment.</param>
        public MethodWrapper(MethodInfo method)
        {
            this.WrappedMethod = method;
        }

        /// <summary>
        /// Gets the method that this object is wrapped around.
        /// </summary>
        public MethodInfo WrappedMethod
        {
            get;
            private set;
        }

        public bool IsVirtual
        {
            get { return WrappedMethod.IsVirtual; }
        }

        public bool IsAbstract
        {
            get { return WrappedMethod.IsAbstract; }
        }

        public bool IsFinal
        {
            get { return WrappedMethod.IsFinal; }
        }

        public AccessModifier Access
        {
            get
            {
                if (WrappedMethod.IsPublic)
                {
                    return AccessModifier.Public;
                }
                else if (WrappedMethod.IsPrivate)
                {
                    return AccessModifier.Private;
                }
                else if (WrappedMethod.IsFamily)
                {
                    return AccessModifier.Protected;
                }
                else if (WrappedMethod.IsFamilyAndAssembly)
                {
                    return AccessModifier.ProtectedAndInternal;
                }
                else
                {
                    return AccessModifier.ProtectedOrInternal;
                }
            }
        }

        public string Name
        {
            get { return WrappedMethod.Name; }
        }

        public Type ReturnType
        {
            get { return WrappedMethod.ReturnType; }
        }

        public Type EnclosingType
        {
            get { return WrappedMethod.ReflectedType; }
        }
    }
}
