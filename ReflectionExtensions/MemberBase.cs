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
using System.Reflection;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a base class for IMember objects.
    /// </summary>
    public abstract class MemberBase : IMember
    {
        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type that this member uses.
        /// Returns the return type for methods, null if the return type is void.
        /// Returns the field/property type for fields/properties.
        /// Returns the enclosing type for constructors.
        /// Returns the accepted type for parameters.
        /// Returns null for generic parameters.
        /// </summary>
        public IType ReturnType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        public IType EnclosingType
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines if this <see cref="MemberBase"/> object equals the given <see cref="IMember"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMember"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public abstract bool Equals(IMember other);

        /// <summary>
        /// Determines if this <see cref="MemberBase"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IMember)
            {
                return Equals((IMember)obj);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public abstract override int GetHashCode();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberBase" /> class.
        /// </summary>
        /// <param name="member">The member that this object defines.</param>
        /// <param name="returnType">The type of objects that this member returns.</param>
        protected MemberBase(MemberInfo member, IType returnType)
        {
            if(member == null)
            {
                throw new ArgumentNullException("member");
            }
            this.Name = member.Name;
            this.EnclosingType = member.DeclaringType.Wrap();
            this.ReturnType = returnType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="enclosingType">Type of the enclosing.</param>
        /// <param name="returnType">Type of the return.</param>
        protected MemberBase(string name, IType enclosingType, IType returnType)
        {
            this.Name = name;
            this.EnclosingType = enclosingType;
            this.ReturnType = returnType;
        }
    }
}
