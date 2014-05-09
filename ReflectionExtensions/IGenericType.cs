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
    /// Defines an interface for a generic type.
    /// </summary>
    public interface IGenericType : IType, IEquatable<IGenericType>
    {
        

        /// <summary>
        /// Gets the list of generic type parameters that can be passed to this type.
        /// </summary>
        IEnumerable<IGenericParameter> TypeParameters
        {
            get;
        }

        /// <summary>
        /// Creates a new ReflectionExtensions.INonGenericType from this type using the given types as arguments.
        /// </summary>
        /// <param name="typeArguments">The list of type arguments to use for the new type.</param>
        /// <returns>Returns a new ReflectionINonGenericType.</returns>
        INonGenericType MakeGenericType(params Type[] typeArguments);
    }

    
}
