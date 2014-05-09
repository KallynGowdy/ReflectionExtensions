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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for a method that is generic and requires type parameters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Generics in C# is implemented using type expansion. A new concrete method/type is created for every used combination.
    /// This provides reflection to generic types and prevents boxing/unboxing on value types.
    /// Normaly the compiler takes care of this for us. However, when we want to call a generic method at runtime the .Net framework needs to create
    /// the concrete version of that method so we can use it. This is a little more difficult to use than just a simple method call through reflection.
    /// Most of the complexity of creating a new method using the type arguments to then call it is taken out so that it looks like a normal generic method call.
    /// </para>
    /// Implementations of this interface are recommended to cache the generated methods so that they can be reused.
    /// </remarks>
    public interface IGenericMethod : IMethod, IEquatable<IGenericMethod>
    {
        /// <summary>
        /// Gets the list of generic parameters that this method takes as arguments.
        /// </summary>
        IEnumerable<IGenericParameter> GenericParameters
        {
            get;
        }

        /// <summary>
        /// Invokes this method using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="genericArguments">A list of types that should be provided to the method as generic arguments.</param>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods signature.</param>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException"> Thrown if the given reference is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if one of the given generic arguments does not match a contstraint.</exception>
        /// <returns>Returns the value returned from the method cast into the given type. Returns default(<typeparamref name="TReturn"/>) value if the return type is void or null.</returns>
        TReturn Invoke<TReturn>(object reference, Type[] genericArguments, object[] arguments);

        /// <summary>
        /// Invokes this method using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="genericArguments">A list of types that should be provided to the method as generic arguments.</param>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods signature.</param>
        /// <param name="defaultValue">The value to return if the returned value from the invocation was void or null.</param>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException"> Thrown if the given reference is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown if one of the given generic arguments does not match a contstraint.</exception>
        /// <returns>Returns the value returned from the method cast into the given type. Returns default(<typeparamref name="TReturn"/>) value if the return type is void or null.</returns>
        TReturn Invoke<TReturn>(object reference, Type[] genericArguments, object[] arguments, TReturn defaultValue);
    }
}
