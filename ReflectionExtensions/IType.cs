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
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for an object that describes a type.
    /// </summary>
    [ContractClass(typeof(ITypeContract))]
    public interface IType : IEquatable<IType>
    {
        /// <summary>
        /// Gets the simple name of the type.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the fully qualified name of the type.
        /// </summary>
        string FullName
        {
            get;
        }

        /// <summary>
        /// Gets the assembly that this type belongs to.
        /// </summary>
        Assembly Assembly
        {
            get;
        }

        /// <summary>
        /// Gets whether this type accepts generic arguments.
        /// </summary>
        bool IsGenericType
        {
            get;
        }

        /// <summary>
        /// Gets whether this type is a class.
        /// </summary>
        bool IsClass
        {
            get;
        }

        /// <summary>
        /// Gets whether this type is a structure.
        /// </summary>
        bool IsStruct
        {
            get;
        }

        /// <summary>
        /// Gets whether this type is abstract.
        /// </summary>
        bool IsAbstract
        {
            get;
        }

        /// <summary>
        /// Gets the type that this type inherits from.
        /// </summary>
        IType BaseType
        {
            get;
        }

        /// <summary>
        /// Gets the generic arguments that this type accepts.
        /// </summary>
        IEnumerable<IGenericParameter> GenericArguments
        {
            get;
        }

        /// <summary>
        /// Gets a list of public non-static members from this type.
        /// </summary>
        IEnumerable<IMember> Members
        {
            get;
        }

        /// <summary>
        /// Gets a list of public non-static fields from this type.
        /// </summary>
        IEnumerable<IField> Fields
        {
            get;
        }

        /// <summary>
        /// Gets a list of public non-static properties that belong to this type.
        /// </summary>
        IEnumerable<IProperty> Properties
        {
            get;
        }

        /// <summary>
        /// Gets a list of public non-static methods that belong to this type.
        /// </summary>
        IEnumerable<IMethod> Methods
        {
            get;
        }

        /// <summary>
        /// Determines if this type inherits from the given base type.
        /// </summary>
        /// <param name="baseType">The type to check inheritance from.</param>
        /// <returns>Returns true if this type inherits from the given base type, otherwise false.</returns>
        bool InheritsFrom(IType baseType);

        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type that the returned value should be cast into.</typeparam>
        /// <param name="name">The case-sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="arguments">A list of objects to use as arguments for the invocation.</param>
        /// <exception cref="Extensions.TypeArgumentException">Thrown if the value returned from the method cannot be cast into the given type.</exception>
        /// <exception cref="System.MissingMethodException">
        /// Thrown if the method to invoke does not exist. That is, if there is no method with the given name or no method
        /// matches the given arguments.
        /// </exception>
        /// <exception cref="System.ArgumentException">Thrown if the given reference's type does not equal the type that is described by this value.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given name or reference is null.</exception>
        /// <returns>Returns the result of the method invocation cast into the given type. Returns default(<typeparamref name="TReturn"/>) if the method returns null or void.</returns>
        TReturn Invoke<TReturn>(string name, object reference, params object[] arguments);

        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type that the returned value should be cast into.</typeparam>
        /// <param name="name">The cast sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="genericArguments">A list of System.Type objects to use as generic arguments for the invocation.</param>
        /// <param name="arguments">A list of objects that should be used as arguments for the invocation.</param>
        /// <exception cref="System.MissingMethodException">
        /// Thrown if the method to call does not exist. That is, if there is no method with the given name or there no method matches
        /// the given arguments.
        /// </exception>
        /// <exception cref="Extensions.TypeArgumentException">Thrown if the value returned from the method cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if the given name or reference is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if the given reference's type does not equal the type that is described by this value.
        /// </exception>
        /// <returns>Returns the result of the method call cast into the given type. Returns default(<typeparamref name="TReturn"/>) if the method returns null or void.</returns>
        TReturn Invoke<TReturn>(string name, object reference, Type[] genericArguments, object[] arguments);

        /// <summary>
        /// Gets a list of public non-static members that retrieve/set some value.
        /// </summary>
        IEnumerable<IStorageMember> StorageMembers
        {
            get;
        }

        /// <summary>
        /// Gets a list of public non-static constructors that can create this type.
        /// </summary>
        IEnumerable<IMethod> Constructors
        {
            get;
        }

        /// <summary>
        /// Gets whether the type defines an interface.
        /// </summary>
        bool IsInterface
        {
            get;
        }
    }

    [ContractClassFor(typeof(IType))]
    internal abstract class ITypeContract : IType
    {
        string IType.Name
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return default(string);
            }
        }

        string IType.FullName
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return default(string);
            }
        }

        Assembly IType.Assembly
        {
            get
            {
                Contract.Ensures(Contract.Result<Assembly>() != null);
                return default(Assembly);
            }
        }

        bool IType.IsGenericType
        {
            get { return default(bool); }
        }

        bool IType.IsClass
        {
            get { return default(bool); }
        }

        bool IType.IsStruct
        {
            get { return default(bool); }
        }

        bool IType.IsAbstract
        {
            get { return default(bool); }
        }

        IType IType.BaseType
        {
            get
            {
                return default(IType);
            }
        }

        IEnumerable<IGenericParameter> IType.GenericArguments
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IGenericParameter>>() != null);
                return default(IEnumerable<IGenericParameter>);
            }
        }

        IEnumerable<IMember> IType.Members
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IMember>>() != null);
                return default(IEnumerable<IMember>);
            }
        }

        IEnumerable<IField> IType.Fields
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IField>>() != null);
                return default(IEnumerable<IField>);
            }
        }

        IEnumerable<IProperty> IType.Properties
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IProperty>>() != null);
                return default(IEnumerable<IProperty>);
            }
        }

        IEnumerable<IMethod> IType.Methods
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IMethod>>() != null);
                return default(IEnumerable<IMethod>);
            }
        }

        bool IType.InheritsFrom(IType baseType)
        {
            Contract.Requires<ArgumentNullException>(baseType != null, "baseType");
            return default(bool);
        }

        TReturn IType.Invoke<TReturn>(string name, object reference, params object[] arguments)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(reference != null, "reference");
            return default(TReturn);
        }

        TReturn IType.Invoke<TReturn>(string name, object reference, Type[] genericArguments, object[] arguments)
        {
            Contract.Requires<ArgumentNullException>(name != null, "name");
            Contract.Requires<ArgumentNullException>(reference != null, "reference");
            return default(TReturn);
        }

        IEnumerable<IStorageMember> IType.StorageMembers
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IStorageMember>>() != null);
                return default(IEnumerable<IStorageMember>);
            }
        }

        IEnumerable<IMethod> IType.Constructors
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IMethod>>() != null);
                return default(IEnumerable<IMethod>);
            }
        }

        bool IEquatable<IType>.Equals(IType other)
        {
            return default(bool);
        }


        bool IType.IsInterface
        {
            get
            {
                return default(bool);
            }
        }
    }
}
