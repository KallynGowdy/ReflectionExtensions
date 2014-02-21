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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class that describes a generic parameter constraint.
    /// </summary>
    public interface IGenericConstraint : IEquatable<IGenericConstraint>
    {
        /// <summary>
        /// Determines if the given type matches this contstraint.
        /// </summary>
        /// <param name="type">The type to test against the constraint.</param>
        /// <returns>Returns true if the type matches the constraint, otherwise false.</returns>
        bool MatchesConstraint(IType type);
    }
}
