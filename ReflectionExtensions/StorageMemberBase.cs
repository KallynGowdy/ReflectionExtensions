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
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a base class for storage members.
    /// </summary>
    public abstract class StorageMemberBase : MemberBase, IStorageMember
    {
        /// <summary>
        /// Gets whether the value stored by the member is readable.
        /// </summary>
        public bool CanRead
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets whether the value stored by the member is overwritable.
        /// </summary>
        public bool CanWrite
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets whether the Type stored by this member is an array.
        /// </summary>
        public bool IsArray
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the number of dimentions of the array that is stored in this object.
        /// Always returns 1 or higher.
        /// </summary>
        /// <remarks>
        /// Even though not every member is an array, you can treat every member like it stores an array. If it is not an array, the first index returns the
        /// value stored in this array.
        /// </remarks>
        public int ArrayRank
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified reference.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="reference">The reference.</param>
        /// <param name="indexes">The indexes.</param>
        /// <returns></returns>
        public abstract object this[object reference, params int[] indexes]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified reference.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public abstract object this[object reference]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>
        /// Returns the value stored by the given object in this field.
        /// </returns>
        public abstract object GetValue(object reference);

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>
        /// Returns the value stored by the given object in this field.
        /// </returns>
        /// <exception cref="TypeArgumentException">The returned value cannot be cast into the given type.;T</exception>
        public virtual T GetValue<T>(object reference)
        {
            try
            {
                return (T)GetValue(reference);
            }
            catch (InvalidCastException e)
            {
                throw new TypeArgumentException("The returned value cannot be cast into the given type.", "T", e);
            }
        }

        /// <summary>
        /// Sets the value stored in this field/property in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to set the value for.</param>
        /// <param name="value">The value to set in this property/field.</param>
        public abstract void SetValue(object reference, object value);

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageMemberBase"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        protected StorageMemberBase(FieldInfo field)
            : base(field, field.FieldType.Wrap())
        {
            this.CanRead = true;
            this.CanWrite = true;
            this.IsArray = field.FieldType.IsArray;
            this.ArrayRank = IsArray ? field.FieldType.GetArrayRank() : 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageMemberBase"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        protected StorageMemberBase(PropertyInfo property)
            : base(property, property.PropertyType.Wrap())
        {
            this.CanWrite = property.CanWrite;
            this.CanRead = property.CanRead;
            this.IsArray = property.PropertyType.IsArray;
            this.ArrayRank = IsArray ? property.PropertyType.GetArrayRank() : 1;
        }
    }
}
