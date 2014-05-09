﻿// Copyright 2014 Kallyn Gowdy
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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for a generic constraint that requires that a type inherits from some other type.
    /// </summary>
    public interface IInheritanceConstraint : IGenericConstraint, IEquatable<IInheritanceConstraint>
    {
        /// <summary>
        /// Gets whether the result of the constraint calculation is inverted.
        /// This acts like a NOT operator. "When the given type does NOT inherit from the required type"
        /// </summary>
        bool Invert
        {
            get;
        }

        /// <summary>
        /// Gets the type that is required to be in the inheritance chain of the generic argument.
        /// </summary>
        IType RequiredType
        {
            get;
        }
    }
}
