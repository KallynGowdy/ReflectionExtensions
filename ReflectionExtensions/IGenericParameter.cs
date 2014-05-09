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
    /// Defines an interface for a generic parameter.
    /// </summary>
    public interface IGenericParameter : IMember, IEquatable<IGenericParameter>
    {

        /// <summary>
        /// Gets the (zero-based) index that the parameter appears at.
        /// </summary>
        int Position
        {
            get;
        }

        /// <summary>
        /// Gets the list of constraints that are put on this parameter.
        /// </summary>
        IEnumerable<IGenericConstraint> Constraints
        {
            get;
        }

        /// <summary>
        /// Determines if the given type matches all of the constraints on this parameter.
        /// </summary>
        /// <param name="type">The type to test against the contstraints.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the given type is null.</exception>
        /// <returns></returns>
        bool MatchesConstraints(IType type);
    }

}
