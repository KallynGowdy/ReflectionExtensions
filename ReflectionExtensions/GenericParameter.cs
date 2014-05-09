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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class for a generic parameter.
    /// </summary>
    public class GenericParameter : MemberBase, IGenericParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericParameter"/> class.
        /// </summary>
        /// <param name="type">The type that the parameter represents..</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public GenericParameter(Type type) : base(type, type.Wrap())
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsGenericParameter)
            {
                throw new ArgumentException("The given type must be a generic parameter.", "type");
            }

            Position = type.GenericParameterPosition;
            BuildConstraints(type);
        }

        private void BuildConstraints(Type type)
        {
            if(type == null)
            {
                throw new ArgumentNullException("type");
            }
            List<IGenericConstraint> constraints = new List<IGenericConstraint>();


            if (type.BaseType != null && !type.BaseType.Equals(typeof(object)))
            {
                constraints.Add(new InheritanceConstraint(type.BaseType));
            }
            else if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
            {
                //Allow any type that does not inherit from System.ValueType.
                constraints.Add(new InheritanceConstraint(typeof(ValueType), true));
            }
            else if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                //Allow any type that inherits from System.ValueType.
                constraints.Add(new InheritanceConstraint(typeof(ValueType)));
            }

            if (type.GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
            {
                constraints.Add(new ConstructorConstraint());
            }

            foreach (Type @interface in type.GetInterfaces())
            {
                constraints.Add(new InheritanceConstraint(@interface));
            }

            this.Constraints = constraints.ToArray();
        }

        /// <summary>
        /// Gets the (zero-based) index that the parameter appears at.
        /// </summary>
        public int Position
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of constraints that are put on this parameter.
        /// </summary>
        public IEnumerable<IGenericConstraint> Constraints
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines if the given type matches all of the constraints on this parameter.
        /// </summary>
        /// <param name="type">The type to test against the contstraints.</param>
        /// <returns></returns>
        public bool MatchesConstraints(IType type)
        {
            return  Constraints.All(c => c.MatchesConstraint(type));
        }

        /// <summary>
        /// Determines if this <see cref="GenericParameter"/> object equals the given <see cref="IMember"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMember"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public override bool Equals(IMember other)
        {
            if (other is IGenericParameter)
            {
                return Equals((IGenericParameter)other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if this <see cref="GenericParameter"/> object equals the given <see cref="IGenericParameter"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IGenericParameter"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IGenericParameter other)
        {
            return other != null &&
                other.Name.Equals(this.Name) &&
                other.Position == this.Position &&
                other.EnclosingType.Equals(this.EnclosingType) &&
                other.Constraints.SequenceEqual(this.Constraints);
        }

        /// <summary>
        /// Determines if this <see cref="GenericParameter"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IGenericParameter)
            {
                return Equals((IGenericParameter)obj);
            }
            else if (obj is IParameter)
            {
                return Equals((IParameter)obj);
            }
            else if (obj is IMember)
            {
                return Equals((IMember)obj);
            }
            else
            {
                return base.Equals(obj);
            }            
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Util.HashCode(12791, Name, ReturnType, EnclosingType, Position, Constraints);
        }
    }
}
