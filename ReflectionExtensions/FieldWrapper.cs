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
using System.Reflection;
using Fasterflect;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a class that provides a wrapper for a FieldInfo object that implements <see cref="ReflectionExtensions.IField"/>.
    /// </summary>
    public class FieldWrapper : StorageMemberBase, IField
    {
        private readonly MemberGetter getter;
        private readonly MemberSetter setter;

        /// <summary>
        /// Gets the field that is being wrapped.
        /// </summary>
        public FieldInfo WrappedField
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new FieldWrapper around the given FieldInfo object.
        /// </summary>
        /// <param name="field">The field to wrap.</param>
        /// <exception cref="System.ArgumentNullException">Throw if the given field is null.</exception>
        public FieldWrapper(FieldInfo field) : base(field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            this.WrappedField = field;
            this.getter = WrappedField.DelegateForGetFieldValue();
            this.setter = WrappedField.DelegateForSetFieldValue();
        }

        /// <summary>
        /// Gets whether this field is constant.
        /// </summary>
        public bool IsConst
        {
            get { return WrappedField.IsLiteral; }
        }

        /// <summary>
        /// Gets or sets the value stored in this field in the given object.
        /// </summary>
        /// <param name="reference">The object reference from which the value should be retrieved or set.</param>
        /// <returns>Returns the value referenced by this field that was stored in the given reference.</returns>
        public override object this[object reference]
        {
            get
            {
                return getter(reference);
            }
            set
            {
                setter(reference, value);
            }
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
        /// <exception cref="System.NullReferenceException">
        /// The Array is null. Therfore an index cannot be accessed.
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">
        /// The value store in this object is not an array. Therefore the given index(es) need to refer to the first element in the first dimention to retrieve a valid value.
        /// </exception>
        public override object this[object reference, params int[] indexes]
        {
            get
            {
                if (IsArray)
                {
                    Array array = (Array)getter(reference);
                    if (array != null)
                    {
                        return array.GetValue(indexes);
                    }
                    else
                    {
                        throw new NullReferenceException("The Array is null. Therefore an index cannot be accessed.");
                    }
                }
                else
                {
                    if(indexes == null)
                    {
                        throw new ArgumentNullException("indexes");
                    }
                    if (indexes.Length == 1 && indexes[0] == 0)
                    {
                        return this[reference];
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("The value store in this object is not an array. Therefore the given index(es) need to refer to the first element in the first dimension to retrieve a valid value.");
                    }
                }
            }
            set
            {
                if (IsArray)
                {
                    Array array = (Array)getter(reference);
                    if (array != null)
                    {
                        array.SetValue(value, indexes);
                    }
                    else
                    {
                        throw new NullReferenceException("The Array is null. Therfore an index cannot be accessed");
                    }
                }
                else
                {
                    if(indexes == null)
                    {
                        throw new ArgumentNullException("indexes");
                    }
                    if (indexes.Length == 1 && indexes[0] == 0)
                    {
                        this[reference] = value;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("The value store in this object is not an array. Therefore the given index(es) need to refer to the first element in the first dimension to retrieve a valid value.");
                    }
                }
            }
        }

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>
        /// Returns the value stored by the given object in this field.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override object GetValue(object reference)
        {
            return this[reference];
        }

        /// <summary>
        /// Sets the value stored in this field/property in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to set the value for.</param>
        /// <param name="value">The value to set in this property/field.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override void SetValue(object reference, object value)
        {
            this[reference] = value;
        }

        /// <summary>
        /// Gets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        public AccessModifier Access
        {
            get
            {
                if (WrappedField.IsPublic)
                {
                    return AccessModifier.Public;
                }
                else if (WrappedField.IsPrivate)
                {
                    return AccessModifier.Private;
                }
                else if (WrappedField.IsFamily)
                {
                    return AccessModifier.Protected;
                }
                else if (WrappedField.IsFamilyAndAssembly)
                {
                    return AccessModifier.ProtectedAndInternal;
                }
                else
                {
                    return AccessModifier.ProtectedOrInternal;
                }
            }
        }

        /// <summary>
        /// Determines if this <see cref="FieldWrapper"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IField)
            {
                return Equals((IField)obj);
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
        /// Determines if this <see cref="FieldWrapper"/> object equals the given <see cref="IMember"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMember"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public override bool Equals(IMember other)
        {
            if (other is IField)
            {
                return Equals((IField)other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if this <see cref="FieldWrapper"/> object equals the given <see cref="IField"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IField"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IField other)
        {
            return other != null &&
                other.Name.Equals(this.Name) &&
                other.IsConst == this.IsConst &&
                other.ReturnType.Equals(this.ReturnType) &&
                other.CanRead == this.CanRead &&
                other.CanWrite == this.CanWrite &&
                other.EnclosingType.Equals(this.EnclosingType);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Util.HashCode(39511, Name, IsConst, ReturnType, CanRead, CanWrite, EnclosingType);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
