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
    class ParameterWrapper : MemberBase, IParameter
    {
        public ParameterWrapper(System.Reflection.ParameterInfo parameter) : base(parameter.Name, parameter.Member.DeclaringType.Wrap(), parameter.ParameterType.Wrap())
        {
            this.WrappedParameter = parameter;
        }

        /// <summary>
        /// Gets the wrapped parameter.
        /// </summary>
        /// <value>
        /// The wrapped parameter.
        /// </value>
        public ParameterInfo WrappedParameter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the method that this parameter belongs to.
        /// </summary>
        public IMethod Method
        {
            get { return WrappedParameter.Member.Wrap() as IMethod; }
        }

        /// <summary>
        /// Gets the position in the list of arguments that is given to the method.
        /// </summary>
        public int Position
        {
            get { return WrappedParameter.Position; }
        }

        /// <summary>
        /// Gets whether this parameter has a default value.
        /// </summary>
        public bool HasDefaultValue
        {
            get { return WrappedParameter.HasDefaultValue; }
        }

        /// <summary>
        /// Gets the default value of the parameter.
        /// </summary>
        public object DefaultValue
        {
            get { return WrappedParameter.DefaultValue; }
        }

       
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name, this.ReturnType);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Util.HashCode(29137, Name, Position, ReturnType, EnclosingType);
        }

        /// <summary>
        /// Determines if this <see cref="ParameterWrapper"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IParameter)
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
        /// Determines if this <see cref="ParameterWrapper"/> object equals the given <see cref="IMember"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMember"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public override bool Equals(IMember other)
        {
            if (other is IParameter)
            {
                return Equals((IParameter)other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if this <see cref="ParameterWrapper"/> object equals the given <see cref="IParameter"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IParameter"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IParameter other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.Position.Equals(other.Position) &&
                this.ReturnType.Equals(other.ReturnType) &&
                this.EnclosingType.Equals(other.EnclosingType);
        }
    }
}
