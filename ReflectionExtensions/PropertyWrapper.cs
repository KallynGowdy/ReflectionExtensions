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


using Fasterflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class around System.Reflection.PropertyInfo.
    /// </summary>
    public class PropertyWrapper : IProperty
    {
        private MemberGetter getter;

        private MemberSetter setter;

        /// <summary>
        /// Gets the System.Reflection.PropertyInfo object that this wrapper uses.
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new wrapper around the given System.Reflection.PropertyInfo object.
        /// </summary>
        /// <param name="propertyInfo">The System.Reflection.PropertyInfo object to create a wrapper around.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyInfo"/> is null.</exception>
        public PropertyWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }
            this.PropertyInfo = propertyInfo;
            if (CanRead)
            {
                this.getter = propertyInfo.DelegateForGetPropertyValue();
            }
            if (CanWrite)
            {
                this.setter = propertyInfo.DelegateForSetPropertyValue();
            }
        }

        /// <summary>
        /// Gets whether the value stored by the member is readable.
        /// </summary>
        public bool CanRead
        {
            get { return PropertyInfo.CanRead; }
        }

        /// <summary>
        /// Gets whether the value stored by the member is overwritable.
        /// </summary>
        public bool CanWrite
        {
            get { return PropertyInfo.CanWrite; }
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        public string Name
        {
            get { return PropertyInfo.Name; }
        }

        /// <summary>
        /// Gets the type that this member uses.
        /// Returns the return type for methods, null if the return type is void.
        /// Returns the field/property type for fields/properties.
        /// Returns the enclosing type for constructors.
        /// Returns the accepted type for parameters.
        /// Returns null for generic parameters.
        /// </summary>
        public Type ReturnType
        {
            get { return PropertyInfo.PropertyType; }
        }

        /// <summary>
        /// Gets the type that this member belongs to.
        /// </summary>
        public Type EnclosingType
        {
            get { return PropertyInfo.ReflectedType; }
        }

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>
        /// Returns the value stored by the given object in this field.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">The value cannot be read from this property.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public object GetValue(object reference)
        {
            if (CanRead)
            {
                return getter(reference);
            }
            else
            {
                throw new InvalidOperationException("The value cannot be read from this property.");
            }

        }

        /// <summary>
        /// Sets the value stored in this field/property in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to set the value for.</param>
        /// <param name="value">The value to set in this property/field.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void SetValue(object reference, object value)
        {
            if (CanWrite)
            {
                setter(reference, value);
            }
            else
            {
                throw new InvalidOperationException("The value cannot be written to this property");
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified reference.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public object this[object reference]
        {
            get
            {
                return this.GetValue(reference);
            }
            set
            {
                this.SetValue(reference, value);
            }
        }

        /// <summary>
        /// Gets the get method of this property.
        /// </summary>
        public IMethod GetMethod
        {
            get { return PropertyInfo.GetMethod.Wrap(); }
        }

        /// <summary>
        /// Gets the set method of this property.
        /// </summary>
        public IMethod SetMethod
        {
            get { return PropertyInfo.SetMethod.Wrap(); }
        }


        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>
        /// Returns the value stored by the given object in this field.
        /// </returns>
        /// <exception cref="TypeArgumentException">The returned value could not be cast into the given type.;T</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public T GetValue<T>(object reference)
        {
            try
            {
                return (T)this.GetValue(reference);
            }
            catch (InvalidCastException e)
            {
                throw new TypeArgumentException("The returned value could not be cast into the given type.", "T", e);
            }
        }

        /// <summary>
        /// Determines if this <see cref="PropertyWrapper"/> object equals the given <see cref="Object"/> object.
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
            else if (obj is IProperty)
            {
                return Equals((IProperty)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Determines if this <see cref="PropertyWrapper"/> object equals the given <see cref="IMember"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMember"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IMember other)
        {
            if (other is IProperty)
            {
                return Equals((IProperty)other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if this <see cref="PropertyWrapper"/> object equals the given <see cref="IProperty"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IProperty"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IProperty other)
        {
            return other != null &&
                other.Name.Equals(this.Name) &&
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
            return Util.HashCode(98981, Name, ReturnType, CanRead, CanWrite, EnclosingType);
        }

        /// <summary>
        /// Gets whether the Type stored by this member is an array.
        /// </summary>
        public bool IsArray
        {
            get { return PropertyInfo.PropertyType.IsArray; }
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
            get { return PropertyInfo.PropertyType.GetArrayRank(); }
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
        /// The Array is null. Therefore an index cannot be accessed.
        /// </exception>
        /// <exception cref="System.IndexOutOfRangeException">
        /// The value store in this object is not an array. Therefore the given index(es) need to refer to the first element in the first dimention to retrieve a valid value.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The Property Cannot Read it's array and therefore cannot retrieve the value at the given index
        /// or
        /// The Property Cannot Read it's array value and therefore cannot set the value at the given index.
        /// </exception>
        public object this[object reference, params int[] indexes]
        {
            get
            {
                if (CanRead)
                {
                    if (IsArray)
                    {
                        Array array = this.GetValue<Array>(reference);
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
                        if (indexes.Length == 1 && indexes[0] == 0)
                        {
                            return this[reference];
                        }
                        else
                        {
                            throw new IndexOutOfRangeException("The value store in this object is not an array. Therefore the given index(es) need to refer to the first element in the first dimention to retrieve a valid value.");
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("The Property Cannot Read it's array and therefore cannot retrieve the value at the given index");
                }
            }
            set
            {
                if (CanRead)
                {
                    if (IsArray)
                    {
                        Array array = this.GetValue<Array>(reference);
                        if (array != null)
                        {
                            array.SetValue(value, indexes);
                        }
                        else
                        {
                            throw new NullReferenceException("The array is null. Therfore an index cannot be accessed");
                        }
                    }
                    else
                    {
                        if (indexes.Length == 1 && indexes[0] == 0)
                        {
                            this[reference] = value;
                        }
                        else
                        {
                            throw new IndexOutOfRangeException("The value store in this object is not an array. Therefore the given index(es) need to refer to the first element in the first dimention to retrieve a valid value.");
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("The Property Cannot Read it's array value and therefore cannot set the value at the given index.");
                }
            }
        }
    }
}
