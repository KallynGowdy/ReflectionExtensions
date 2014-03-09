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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for a type for which all of the type parameters have been resolved.
    /// </summary>
    [ContractClass(typeof(INonGenericTypeContract))]
    public interface INonGenericType : IType, IEquatable<INonGenericType>
    {
        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type that the returned value should be cast into.</typeparam>
        /// <param name="name">The case-sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="arguments">A list of objects to use as arguments for the invocation.</param>
        /// <returns>
        /// Returns the result of the method invocation cast into the given type. Returns default(<typeparamref name="TReturn" />) if the method returns null or void.
        /// </returns>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the value returned from the method cannot be cast into the given type.</exception>
        /// <exception cref="System.MissingMethodException">Thrown if the method to invoke does not exist. That is, if there is no method with the given name or no method
        /// matches the given arguments.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the given reference's type does not equal the type that is described by this value.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given name or reference is null.</exception>
        TReturn Invoke<TReturn>(string name, object reference, params object[] arguments);

        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type that the returned value should be cast into.</typeparam>
        /// <param name="name">The cast sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="genericArguments">A list of System.Type objects to use as generic arguments for the invocation.</param>
        /// <param name="arguments">A list of objects that should be used as arguments for the invocation.</param>
        /// <returns>
        /// Returns the result of the method call cast into the given type. Returns default(<typeparamref name="TReturn" />) if the method returns null or void.
        /// </returns>
        /// <exception cref="System.MissingMethodException">Thrown if the method to call does not exist. That is, if there is no method with the given name or there no method matches
        /// the given arguments.</exception>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the value returned from the method cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given name or reference is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the given reference's type does not equal the type that is described by this value.</exception>
        TReturn Invoke<TReturn>(string name, object reference, Type[] genericArguments, object[] arguments);

        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <param name="name">The case-sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="arguments">A list of objects that should be used as arguments for the invocation.</param>
        /// <returns>Returns the result of the method call.</returns>
        object Invoke(string name, object reference, params object[] arguments);
    }

    [ContractClassFor(typeof(INonGenericType))]
    internal abstract class INonGenericTypeContract : INonGenericType
    {

        TReturn INonGenericType.Invoke<TReturn>(string name, object reference, params object[] arguments)
        {
            Contract.Requires(name != null);
            Contract.Requires(reference != null);
            return default(TReturn);
        }

        TReturn INonGenericType.Invoke<TReturn>(string name, object reference, Type[] genericArguments, object[] arguments)
        {
            Contract.Requires(name != null);
            Contract.Requires(reference != null );
            return default(TReturn);
        }

        object INonGenericType.Invoke(string name, object reference, params object[] arguments)
        {
            Contract.Requires(name != null);
            Contract.Requires(reference != null);
            return default(object);
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

        bool IEquatable<INonGenericType>.Equals(INonGenericType other)
        {
            return default(bool);
        }

        public AccessModifier Access
        {
            get { return default(AccessModifier); }
        }
    }
}
