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
    /// Defines an interface for a generic type.
    /// </summary>
    [ContractClass(typeof(IGenericTypeContract))]
    public interface IGenericType : IType, IEquatable<IGenericType>
    {
        

        /// <summary>
        /// Gets the list of generic type parameters that can be passed to this type.
        /// </summary>
        IEnumerable<IGenericParameter> TypeParameters
        {
            get;
        }

        /// <summary>
        /// Creates a new ReflectionExtensions.INonGenericType from this type using the given types as arguments.
        /// </summary>
        /// <param name="typeArguments">The list of type arguments to use for the new type.</param>
        /// <returns>Returns a new ReflectionINonGenericType.</returns>
        INonGenericType MakeGenericType(params Type[] typeArguments);
    }

    [ContractClassFor(typeof(IGenericType))]
    internal abstract class IGenericTypeContract : IGenericType
    {
        IEnumerable<IGenericParameter> IGenericType.TypeParameters
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IGenericParameter>>() != null);
                return default(IEnumerable<IGenericParameter>);
            }
        }

        INonGenericType IGenericType.MakeGenericType(params Type[] typeArguments)
        {
            Contract.Requires(typeArguments != null);
            Contract.Ensures(Contract.Result<INonGenericType>() != null);
            return default(INonGenericType);
        }

        string IType.Name
        {
            get { return default(string); }
        }

        string IType.FullName
        {
            get { return default(string); }
        }

        System.Reflection.Assembly IType.Assembly
        {
            get { return default(Assembly); }
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
            get { return default(IType); }
        }

        IEnumerable<IMember> IType.Members
        {
            get { return default(IEnumerable<IMember>); }
        }

        IEnumerable<IField> IType.Fields
        {
            get { return default(IEnumerable<IField>); }
        }

        IEnumerable<IProperty> IType.Properties
        {
            get { return default(IEnumerable<IProperty>); }
        }

        IEnumerable<IMethod> IType.Methods
        {
            get { return default(IEnumerable<IMethod>); }
        }

        bool IType.InheritsFrom(IType baseType)
        {
            return default(bool);
        }

        IEnumerable<IStorageMember> IType.StorageMembers
        {
            get { return default(IEnumerable<IStorageMember>); }
        }

        IEnumerable<IMethod> IType.Constructors
        {
            get { return default(IEnumerable<IMethod>); }
        }

        bool IType.IsInterface
        {
            get { return default(bool); }
        }

        bool IEquatable<IType>.Equals(IType other)
        {
            return default(bool);
        }

        bool IEquatable<IGenericType>.Equals(IGenericType other)
        {
            return default(bool);
        }

        public AccessModifier Access
        {
            get { return default(AccessModifier); }
        }
    }
}
