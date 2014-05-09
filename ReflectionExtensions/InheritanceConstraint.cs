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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a basic class that specifies that a given type parameter inherit from a certian type.
    /// </summary>
    public class InheritanceConstraint : IInheritanceConstraint
    {
        /// <summary>
        /// Inverts the result of the constraint calculation.
        /// This acts like a NOT operator (!).
        /// </summary>
        public bool Invert
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new generic constraint that requires that a type argument inherit from the given type.
        /// </summary>
        /// <param name="type">The type to require inheritance from.</param>
        public InheritanceConstraint(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.RequiredType = type.Wrap();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritanceConstraint"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        public InheritanceConstraint(Type type, bool invert)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.RequiredType = type.Wrap();
            this.Invert = invert;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritanceConstraint"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        public InheritanceConstraint(IType type, bool invert)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.RequiredType = type;
            this.Invert = invert;
        }

        /// <summary>
        /// Creates a new generic constraint that requires that a type argument inherit from the given type.
        /// </summary>
        /// <param name="type">The type to require inheritance from.</param>
        public InheritanceConstraint(IType type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            this.RequiredType = type;
        }

        /// <summary>
        /// Gets the type that is required to be in the inheritance chain of the generic argument.
        /// </summary>
        public IType RequiredType
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines if the given type matches this contstraint.
        /// </summary>
        /// <param name="type">The type to test against the constraint.</param>
        /// <returns>
        /// Returns true if the type matches the constraint, otherwise false.
        /// </returns>
        /// Validation is taken care of by Code Contracts
        public bool MatchesConstraint(IType type)
        {
            if(type == null)
            {
                throw new ArgumentNullException("type");
            }
            bool result = type.InheritsFrom(RequiredType);
            return Invert ? !result : result;
        }

        /// <summary>
        /// Determines if this <see cref="InheritanceConstraint"/> object equals the given <see cref="IGenericConstraint"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IGenericConstraint"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IGenericConstraint other)
        {
            if (other is InheritanceConstraint)
            {
                return Equals((IInheritanceConstraint)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        /// <summary>
        /// Determines if this <see cref="InheritanceConstraint" /> object equals the given <see cref="IInheritanceConstraint" /> object.
        /// </summary>
        /// <param name="other">The <see cref="IInheritanceConstraint" /> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IInheritanceConstraint other)
        {
            return other != null &&
                other.RequiredType.Equals(this.RequiredType);
        }
    }
}
