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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for a method that belongs to a type.
    /// </summary>
    /// <remarks>
    /// One thing to note is the fact that a method in itself is ambigous. It can't be called unless you know all of the required parameters.
    /// If we assume that it is not generic then we might easily run into exceptions. Therefore the structure is split into two sections: Generic methods and 
    /// Non-Generic methods. Non generic methods are easily called through the provision of object parameters whereas generic methods require more input.
    /// This structure forces the programmer to think about which type of method that is being called and therefore should eliminate one extra exception path.
    /// </remarks>
    public interface IMethod : IMember, IAccessModifiers, IEquatable<IMethod>
    {
        /// <summary>
        /// Gets whether this method is virtual.
        /// </summary>
        bool IsVirtual
        {
            get;
        }

        /// <summary>
        /// Gets whether this method is abstract.
        /// </summary>
        bool IsAbstract
        {
            get;
        }

        /// <summary>
        /// Gets whether this method is final.
        /// </summary>
        bool IsFinal
        {
            get;
        }

        /// <summary>
        /// Gets the list of parameters that this method takes as arguments.
        /// </summary>
        IEnumerable<IParameter> Parameters
        {
            get;
        }
    }
}
