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
using System.Text;
using System.Reflection;

namespace ReflectionExtensions
{
    public class FieldWrapper : IField
    {
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
        public FieldWrapper(FieldInfo field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            // TODO: Complete member initialization
            this.WrappedField = field;
        }

        public bool IsConst
        {
            get { return WrappedField.IsLiteral; }
        }

        public bool CanRead
        {
            get { return true; }
        }

        public bool CanWrite
        {
            get { return true; }
        }

        public object this[object reference]
        {
            get
            {
                return WrappedField.GetValue(reference);
            }
            set
            {
                WrappedField.SetValue(reference, value);
            }
        }

        public object GetValue(object reference)
        {
            return this[reference];
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public T GetValue<T>(object reference)
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

        public void SetValue(object reference, object value)
        {
            this[reference] = value;
        }

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

        public string Name
        {
            get { return WrappedField.Name; }
        }

        public Type ReturnType
        {
            get { return WrappedField.FieldType; }
        }

        public Type EnclosingType
        {
            get { return WrappedField.ReflectedType; }
        }

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

        public bool Equals(IMember other)
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

        public override int GetHashCode()
        {
            return Util.HashCode(Name, IsConst, ReturnType, CanRead, CanWrite, EnclosingType);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
