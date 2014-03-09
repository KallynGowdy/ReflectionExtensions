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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an abstract class for <see cref="IType"/> objects that wrap around a <see cref="System.Type"/> object.
    /// </summary>
    public abstract class TypeWrapperBase : IType
    {
        /// <summary>
        /// Gets the type that the wrapper augments.
        /// </summary>
        public Type WrappedType
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="ReflectionExtensions.TypeWrapperBase"/> that augments the given type.
        /// </summary>
        /// <param name="type">The type to augment.</param>
        protected TypeWrapperBase(Type type)
        {
            Contract.Requires(type != null);

            WrappedType = type;
        }

        private IEnumerable<IMember> members;

        /// <summary>
        /// Gets a list of public non-static members from this type.
        /// </summary>
        public IEnumerable<IMember> Members
        {
            get
            {
                if (members == null)
                {
                    members = WrappedType.GetMembers(BindingFlags.Public | BindingFlags.Instance).Select(m => m.Wrap());
                }
                return members;
            }
        }

        private IEnumerable<IField> fields;

        /// <summary>
        /// Gets a list of public non-static fields from this type.
        /// </summary>
        public IEnumerable<IField> Fields
        {
            get
            {
                if (fields == null)
                {
                    fields = WrappedType.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(f => f.Wrap());
                }
                return fields;
            }
        }

        private IEnumerable<IProperty> properties;

        /// <summary>
        /// Gets a list of public non-static properties that belong to this type.
        /// </summary>
        public IEnumerable<IProperty> Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = WrappedType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Wrap());
                }
                return properties;
            }
        }

        IEnumerable<IMethod> methods;

        /// <summary>
        /// Gets a list of public non-static methods that belong to this type.
        /// </summary>
        public IEnumerable<IMethod> Methods
        {
            get
            {
                if (methods == null)
                {
                    methods = WrappedType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(m => m.Wrap());
                }
                return methods;
            }
        }

        private IEnumerable<IStorageMember> storageMembers;

        /// <summary>
        /// Gets a list of public non-static members that retrieve/set some value.
        /// </summary>
        public IEnumerable<IStorageMember> StorageMembers
        {
            get
            {
                if (storageMembers == null)
                {
                    storageMembers = Members.OfType<IStorageMember>(); ;
                }
                return storageMembers;
            }
        }

        IEnumerable<IMethod> constructors;

        /// <summary>
        /// Gets a list of public non-static constructors that can create this type.
        /// </summary>
        public IEnumerable<IMethod> Constructors
        {
            get
            {
                if (constructors == null)
                {
                    constructors = WrappedType.GetConstructors().Select(c => c.Wrap());
                }
                return constructors;
            }
        }

        /// <summary>
        /// Gets the simple name of the type.
        /// </summary>
        public string Name
        {
            get { return WrappedType.Name; }
        }

        /// <summary>
        /// Gets the fully qualified name of the type.
        /// </summary>
        public string FullName
        {
            get { return WrappedType.FullName; }
        }

        /// <summary>
        /// Gets the assembly that this type belongs to.
        /// </summary>
        public System.Reflection.Assembly Assembly
        {
            get { return WrappedType.Assembly; }
        }

        /// <summary>
        /// Gets whether this type is a class.
        /// </summary>
        public bool IsClass
        {
            get { return WrappedType.IsClass; }
        }

        /// <summary>
        /// Gets whether this type is a structure.
        /// </summary>
        public bool IsStruct
        {
            get { return WrappedType.IsValueType; }
        }

        /// <summary>
        /// Gets whether this type is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return WrappedType.IsAbstract; }
        }

        /// <summary>
        /// Gets the type that this type inherits from.
        /// </summary>
        public IType BaseType
        {
            get { return WrappedType.BaseType.Wrap(); }
        }

        /// <summary>
        /// Gets whether the type defines an interface.
        /// </summary>
        public bool IsInterface
        {
            get { return WrappedType.IsInterface; }
        }

        /// <summary>
        /// Gets the access modifiers that are applied to this type.
        /// </summary>
        public AccessModifier Access
        {
            get
            {
                return Util.GetAccessModifiers(this.WrappedType);
            }
        }

        /// <summary>
        /// Determines if this type inherits from the given base type.
        /// </summary>
        /// <param name="baseType">The type to check inheritance from.</param>
        /// <returns>
        /// Returns true if this type inherits from the given base type, otherwise false.
        /// </returns>
        public bool InheritsFrom(IType baseType)
        {
            if (this.Equals(baseType))
            {
                return true;
            }
            else
            {
                IType objectType = typeof(object).Wrap();
                if (baseType.Equals(objectType))
                {
                    return true;
                }

                if (baseType.IsInterface)
                {
                    return this.WrappedType.GetInterfaces().Any(a => a.Wrap().Equals(baseType));
                }
                else
                {
                    IType inheritedType = this.BaseType;

                    //Otherwise, go through the inheritance chain and check for the type.
                    while (inheritedType != null && !inheritedType.Equals(objectType))
                    {
                        if (inheritedType.Equals(inheritedType))
                        {
                            return true;
                        }
                        inheritedType = inheritedType.BaseType;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Determines if this <see cref="GenericTypeWrapper"/> object equals the given <see cref="IType"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IType"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public virtual bool Equals(IType other)
        {
            return other != null &&
                this.FullName.Equals(other.FullName) &&
                this.IsAbstract == other.IsAbstract &&
                this.IsClass == other.IsClass &&
                this.IsInterface == other.IsInterface &&
                this.IsStruct == other.IsStruct &&
                this.Access == other.Access &&
                this.BaseType.Equals(other.BaseType);
        }

        /// <summary>
        /// Determines if this <see cref="GenericTypeWrapper"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IType)
            {
                return Equals((IType)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        int? hashCode = null;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (!hashCode.HasValue)
            {
                hashCode = Util.HashCode(27457, FullName, IsClass, IsStruct, IsAbstract, Assembly, BaseType);
            }
            return hashCode.Value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Access, (IsClass ? "class" : (IsInterface ? "interface" : "struct")), Name, BaseType != null ? ": " + BaseType : string.Empty);
        }
    }
}
