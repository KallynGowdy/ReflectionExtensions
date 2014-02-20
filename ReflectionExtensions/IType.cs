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
    /// Defines an interface for an object that describes a type.
    /// </summary>
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
        /// Gets a list of public non-static not auto-generated methods that belong to this type.
        /// </summary>
        IEnumerable<IMethod> Methods
        {
            get;
        }


        /// <summary>
        /// Gets the single method that has the given name.
        /// </summary>
        /// <param name="name">The name of the method to retrieve.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if the given name is null.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown if there is more than one method with the given name.</exception>
        /// <returns>Returns the method or null if it doesn't exist.</returns>
        IMethod GetMethod(string name);

        /// <summary>
        /// Gets a list of public non-static methods based on their names.
        /// </summary>
        /// <param name="name">The case sensitive name of the methods to retrieve.</param>
        /// <returns></returns>
        IEnumerable<IMethod> GetMethods(string name);

        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type that the returned value should be cast into.</typeparam>
        /// <param name="name">The case-sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="arguments">A list of objects to use as arguments for the invocation.</param>
        /// <exception cref="ReflectionExtensions.TypeArgumentException">Thrown if the value returned from the method cannot be cast into the given type.</exception>
        /// <exception cref="System.MissingMethodException">
        /// Thrown if the method to invoke does not exist. That is, if there is no method with the given name or is no method
        /// matches the given arguments.
        /// </exception>
        /// <exception cref="System.ArgumentException">Thrown if the given reference's type does not equal the type that is described by this value.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given name or reference is null.</exception>
        /// <returns>Returns the result of the method invocation cast into the given type. Returns default(<typeparamref name="TReturn"/>) if the method returns void.</returns>
        TReturn Invoke<TReturn>(string name, object reference, params object[] arguments);
        

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
    }
}
