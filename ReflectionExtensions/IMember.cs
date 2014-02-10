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
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an abstraction for members of a type.
    /// </summary>
    public interface IMember
    {
        /// <summary>
        /// Gets the access modifiers that are applied to this member.
        /// </summary>
        AccessModifier Access
        {
            get;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the type that this member uses.
        /// Returns the return type for methods, null if the return type is void.
        /// Returns the field/property type for fields/properties.
        /// Returns the enclosing type for constructors.
        /// </summary>
        Type ReturnType
        {
            get;
        }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        Type EnclosingType
        {
            get;
        }
    }
}
