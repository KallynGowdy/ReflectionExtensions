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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class for a generic constraint on a type that defines that it should have a certian constructor.
    /// </summary>
    public class ConstructorConstraint : IConstructorConstraint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorConstraint"/> class.
        /// </summary>
        /// <param name="parameters">The parameters that are required by the constructor.</param>
        public ConstructorConstraint(params IParameter[] parameters)
        {
            if(parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            RequiredParameters = parameters;
        }

        /// <summary>
        /// Gets the the parameters that are required to be passed to the constructor.
        /// </summary>
        public IEnumerable<IParameter> RequiredParameters
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public bool MatchesConstraint(IType type)
        {
            return type.IsStruct || type.Constructors.SequenceEqual(RequiredParameters, (c, p) => c.Parameters.All(cp => p.Equals(cp)));
        }

        /// <summary>
        /// Determines if this <see cref="ConstructorConstraint"/> object equals the given <see cref="IGenericConstraint"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IGenericConstraint"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IGenericConstraint other)
        {
            if (other is IConstructorConstraint)
            {
                return Equals((IConstructorConstraint)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        /// <summary>
        /// Determines if this <see cref="ConstructorConstraint"/> object equals the given <see cref="IConstructorConstraint"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IConstructorConstraint"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IConstructorConstraint other)
        {
            return other != null &&
                other.RequiredParameters.SequenceEqual(this.RequiredParameters);
        }
    }
}
