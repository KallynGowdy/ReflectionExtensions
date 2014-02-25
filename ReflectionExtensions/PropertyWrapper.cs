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
    /// Defines a wrapper class around System.Reflection.PropertyInfo.
    /// </summary>
    public class PropertyWrapper : IProperty
    {
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
        }

        public bool CanRead
        {
            get { return PropertyInfo.CanRead; }
        }

        public bool CanWrite
        {
            get { return PropertyInfo.CanWrite; }
        }

        public string Name
        {
            get { return PropertyInfo.Name; }
        }

        public Type ReturnType
        {
            get { return PropertyInfo.PropertyType; }
        }

        public Type EnclosingType
        {
            get { return PropertyInfo.ReflectedType; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public object GetValue(object reference)
        {
            if (CanRead)
            {
                //if (this.ReturnType.IsAssignableFrom(reference.GetType()))
                //{
                    return PropertyInfo.GetValue(reference);
                //}
                //else
                //{
                //    throw new ArgumentException("The given object must be assignable to this properties type.", "reference");
                //}
            }
            else
            {
                throw new InvalidOperationException("The value cannot be read from this property.");
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void SetValue(object reference, object value)
        {
            PropertyInfo.SetValue(reference, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public object this[object reference]
        {
            get
            {
                return PropertyInfo.GetValue(reference);
            }
            set
            {
                PropertyInfo.SetValue(reference, value);
            }
        }

        public IMethod GetMethod
        {
            get { return PropertyInfo.GetMethod.Wrap(); }
        }

        public IMethod SetMethod
        {
            get { return PropertyInfo.SetMethod.Wrap(); }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public T GetValue<T>(object reference)
        {
            try
            {
                return (T)GetValue(reference);
            }
            catch (InvalidCastException e)
            {
                throw new TypeArgumentException("The returned value could not be cast into the given type.", "T", e);
            }
        }

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

        public bool Equals(IProperty other)
        {
            return other != null &&
                other.Name.Equals(this.Name) &&
                other.ReturnType.Equals(this.ReturnType) &&
                other.CanRead == this.CanRead &&
                other.CanWrite == this.CanWrite &&
                other.EnclosingType.Equals(this.EnclosingType);
        }

        public override int GetHashCode()
        {
            return Util.HashCode(Name, ReturnType, CanRead, CanWrite, EnclosingType);
        }


        public bool IsArray
        {
            get { return PropertyInfo.PropertyType.IsArray; }
        }

        public int ArrayRank
        {
            get { return PropertyInfo.PropertyType.GetArrayRank(); }
        }

        public object this[object reference, params int[] indexes]
        {
            get
            {
                if (CanRead)
                {
                    if (IsArray)
                    {
                        Array array = (Array)PropertyInfo.GetValue(reference);
                        if (array != null)
                        {
                            return array.GetValue(indexes);
                        }
                        else
                        {
                            throw new NullReferenceException("The Array is null. Therfore an index cannot be accessed.");
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
                        Array array = (Array)PropertyInfo.GetValue(reference);
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
