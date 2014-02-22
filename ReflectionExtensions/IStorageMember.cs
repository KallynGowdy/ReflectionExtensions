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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for a member of a type that stores a value. Defines operations and propterties for fields and properties of a type.
    /// </summary>
    [ContractClass(typeof(IStorageMemberContract))]
    public interface IStorageMember : IMember
    {
        /// <summary>
        /// Gets whether the value stored by the member is readable.
        /// </summary>
        bool CanRead
        {
            get;
        }

        /// <summary>
        /// Gets whether the value stored by the member is overwritable.
        /// </summary>
        bool CanWrite
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value of this property/field for the given object.
        /// </summary>
        /// <param name="reference">A reference to the object that contains this property/field.</param>
        /// <returns>Returns the value that is stored in this property/field for the given object.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read/write when trying to read/write.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        object this[object reference]
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>Returns the value stored by the given object in this field.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read the value.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        object GetValue(object reference);

        /// <summary>
        /// Gets the value stored in this member in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to retreive the value from.</param>
        /// <returns>Returns the value stored by the given object in this field.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot read the value.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        /// <exception cref="Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        T GetValue<T>(object reference);

        /// <summary>
        /// Sets the value stored in this field/property in the given object.
        /// </summary>
        /// <param name="reference">A reference to the object to set the value for.</param>
        /// <param name="value">The value to set in this property/field.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if this property/field cannot write when trying to write.</exception>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="reference"/>'s type does not equal this property/field's enclosing type.</exception>
        void SetValue(object reference, object value);
    }

    [ContractClassFor(typeof(IStorageMember))]
    internal abstract class IStorageMemberContract : IStorageMember
    {

        bool IStorageMember.CanRead
        {
            get { return default(bool); }
        }

        bool IStorageMember.CanWrite
        {
            get { return default(bool); }
        }

        object IStorageMember.this[object reference]
        {
            get
            {
                Contract.Requires(reference != null);
                Contract.Requires(((IStorageMember)this).CanRead);
                Contract.Requires(((IMember)this).EnclosingType.IsAssignableFrom(reference.GetType()));
                return default(object);
            }
            set
            {
                Contract.Requires(value != null);
                Contract.Requires(reference != null);
                Contract.Requires(((IStorageMember)this).CanWrite);
                Contract.Requires(((IMember)this).EnclosingType.IsAssignableFrom(reference.GetType()));
            }
        }

        object IStorageMember.GetValue(object reference)
        {
            Contract.Requires(reference != null);
            Contract.Requires(((IStorageMember)this).CanRead);
            Contract.Requires(((IMember)this).EnclosingType.IsAssignableFrom(reference.GetType()));
            return default(object);
        }

        T IStorageMember.GetValue<T>(object reference)
        {
            Contract.Requires(reference != null);
            Contract.Requires(((IStorageMember)this).CanRead);
            Contract.Requires(((IMember)this).EnclosingType.IsAssignableFrom(reference.GetType()));
            return default(T);
        }

        void IStorageMember.SetValue(object reference, object value)
        {
            Contract.Requires(value != null);
            Contract.Requires(reference != null);
            Contract.Requires(((IStorageMember)this).CanWrite);
            Contract.Requires(((IMember)this).EnclosingType.IsAssignableFrom(reference.GetType()));
        }

        string IMember.Name
        {
            get { return default(string); }
        }

        Type IMember.ReturnType
        {
            get { return default(Type); }
        }

        Type IMember.EnclosingType
        {
            get { return default(Type); }
        }

        bool IEquatable<IMember>.Equals(IMember other)
        {
            return default(bool);
        }
    }
}
