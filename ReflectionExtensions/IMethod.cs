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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines an interface for a method that belongs to a type.
    /// </summary>
    public interface IMethod : IMember, IAccessModifiers
    {
        /// <summary>
        /// Gets whether this method is virtual.
        /// </summary>
        bool IsVirtual
        {
            get;
        }

        /// <summary>
        /// Gets whether this method is abstract.
        /// </summary>
        bool IsAbstract
        {
            get;
        }

        /// <summary>
        /// Gets whether this method is final.
        /// </summary>
        bool IsFinal
        {
            get;
        }

        /// <summary>
        /// Gets the list of parameters that this method takes as arguments.
        /// </summary>
        IEnumerable<IParameter> Parameters
        {
            get;
        }

        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <typeparam name="T">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <param name="defaultValue">The value to return if the return type of this method is void.</param>
        /// <exception cref="ReflectionExtensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <returns>Returns the value returned from the method cast into the given type. Returns the default value if the return type is void.</returns>
        T Invoke<T>(object reference, object arguments, T defaultValue = default(T));

        /// <summary>
        /// Invokes this method using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods signature.</param>
        /// <exception cref="ReflectionExtensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <returns>Returns the value returned from the method cast into the given type. Returns default(<typeparamref name="TReturn"/>) value if the return type is void.</returns>
        TReturn Invoke<TReturn>(object reference, params object[] arguments);

        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <param name="reference">A reference to the object that contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <param name="defaultValue">The value to return if the return type of this method is void.</param>
        /// <returns>Returns the value returned from the method. Returns null if the return type is void.</returns>
        object Invoke(object reference, object arguments);
    }
}
