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
    /// Defines an interface for a method that is not generic.
    /// </summary>
    [ContractClass(typeof(INonGenericMethodContract))]
    public interface INonGenericMethod : IMethod, IEquatable<INonGenericMethod>
    {
        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <typeparam name="T">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference object is null.</exception>
        /// <returns>Returns the value returned from the method cast into the given type. Returns the default value if the return type is void or null.</returns>
        T Invoke<T>(object reference, object arguments);

        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <typeparam name="T">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <param name="defaultValue">The value to return if the return type of this method is void.</param>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference object is null.</exception>
        /// <returns>Returns the value returned from the method cast into the given type. Returns the default value if the return type is void or null.</returns>
        T Invoke<T>(object reference, object arguments, T defaultValue);

        /// <summary>
        /// Invokes this method using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods signature.</param>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the returned value cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference object is null.</exception>
        /// <returns>Returns the value returned from the method cast into the given type. Returns default(<typeparamref name="TReturn"/>) value if the return type is void or null.</returns>
        TReturn Invoke<TReturn>(object reference, params object[] arguments);

        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <param name="reference">A reference to the object that contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <returns>
        /// Returns the value returned from the method. Returns null if the return type is void or null.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if the given reference object is null.</exception>
        object Invoke(object reference, object arguments);

    }

    [ContractClassFor(typeof(INonGenericMethod))]
    internal abstract class INonGenericMethodContract : INonGenericMethod
    {

        T INonGenericMethod.Invoke<T>(object reference, object arguments)
        {
            return default(T);
        }

        T INonGenericMethod.Invoke<T>(object reference, object arguments, T defaultValue)
        {
            return default(T);
        }

        TReturn INonGenericMethod.Invoke<TReturn>(object reference, params object[] arguments)
        {
            return default(TReturn);
        }

        object INonGenericMethod.Invoke(object reference, object arguments)
        {
            return default(object);
        }

        bool IMethod.IsVirtual
        {
            get { return default(bool); }
        }

        bool IMethod.IsAbstract
        {
            get { return default(bool); }
        }

        bool IMethod.IsFinal
        {
            get { return default(bool); }
        }

        IEnumerable<IParameter> IMethod.Parameters
        {
            get
            {
                return default(IEnumerable<IParameter>);
            }
        }

        string IMember.Name
        {
            get
            {
                return default(string);
            }
        }

        Type IMember.ReturnType
        {
            get
            {
                return default(Type);
            }
        }

        Type IMember.EnclosingType
        {
            get
            {
                return default(Type);
            }
        }

        bool IEquatable<IMember>.Equals(IMember other)
        {
            return default(bool);
        }

        AccessModifier IAccessModifiers.Access
        {
            get { return default(AccessModifier); }
        }

        bool IEquatable<IMethod>.Equals(IMethod other)
        {
            return default(bool);
        }

        bool IEquatable<INonGenericMethod>.Equals(INonGenericMethod other)
        {
            return default(bool);
        }
    }
}
