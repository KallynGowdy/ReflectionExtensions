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
using System.Linq;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class for a type.
    /// </summary>
    public class NonGenericTypeWrapper : TypeWrapperBase, INonGenericType
    {
        /// <summary>
        /// Creates a new ReflectionExtensions.TypeWrapper object around the given type.
        /// </summary>
        /// <param name="representedType">The type to augment.</param>
        public NonGenericTypeWrapper(Type representedType) : base(representedType)
        {
            if (representedType == null)
            {
                throw new ArgumentNullException("representedType");
            }
        }

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
        /// <exception cref="System.MissingMethodException"></exception>
        public TReturn Invoke<TReturn>(string name, object reference, params object[] arguments)
        {

            INonGenericMethod method = Methods.WithName(name).OfType<INonGenericMethod>().SingleOrDefault(m => m.Parameters.SequenceEqual(arguments, (p, a) => (a == null && !p.ReturnType.IsStruct) || a.GetType().Wrap().InheritsFrom(p.ReturnType)));
            if (method == null)
            {
                throw new MissingMethodException(string.Format("The method, {0}, could not be found with the signature {0}({1})", name, string.Join(", ", arguments.Select(a => a.GetType().Name).ToArray())));
            }
            else
            {
                return method.Invoke<TReturn>(reference, arguments);
            }
        }

        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <param name="name">The case-sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="arguments">A list of objects that should be used as arguments for the invocation.</param>
        /// <returns>Returns the result of the method call.</returns>
        public object Invoke(string name, object reference, params object[] arguments)
        {
            return Invoke<object>(name, reference, arguments);
        }

        /// <summary>
        /// Invokes the method with the given case-sensitive name in the context of the given reference using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type that the returned value should be cast into.</typeparam>
        /// <param name="name">The case-sensitive name of the method to invoke.</param>
        /// <param name="reference">A reference to the object whose type contains the method to invoke.</param>
        /// <param name="genericArguments">A list of System.Type objects to use as generic arguments for the invocation.</param>
        /// <param name="arguments">A list of objects to use as arguments for the invocation.</param>
        /// <returns>
        /// Returns the result of the method invocation cast into the given type. Returns default(<typeparamref name="TReturn" />) if the method returns null or void.
        /// </returns>
        /// <exception cref="System.MissingMethodException">Thrown if the method to invoke does not exist. That is, if there is no method with the given name or no method
        /// matches the given arguments.</exception>
        /// <exception cref="T:Extensions.TypeArgumentException">Thrown if the value returned from the method cannot be cast into the given type.</exception>
        /// <exception cref="System.ArgumentException">Thrown if the given reference's type does not equal the type that is described by this value.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the given name or reference is null.</exception>
        public TReturn Invoke<TReturn>(string name, object reference, Type[] genericArguments, object[] arguments)
        {
            INonGenericMethod method = Methods.WithName(name).OfType<INonGenericMethod>().SingleOrDefault(m => m.Parameters.SequenceEqual(arguments, (p, a) => a.GetType().Wrap().InheritsFrom(p.ReturnType)));
            if (method == null)
            {
                throw new MissingMethodException(string.Format("The method, {0}, could not be found with the signature {0}({1})", name, string.Join(", ", arguments.Select(a => a.GetType().Name).ToArray())));
            }
            else
            {
                return method.Invoke<TReturn>(reference, genericArguments, arguments);
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        /// <summary>
        /// Determines if this <see cref="NonGenericTypeWrapper"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is INonGenericType)
            {
                return Equals((INonGenericType)obj);
            }
            else if (obj is IType)
            {
                return Equals((IType)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Determines if this <see cref="NonGenericTypeWrapper"/> object equals the given <see cref="INonGenericType"/> object.
        /// </summary>
        /// <param name="other">The <see cref="INonGenericType"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(INonGenericType other)
        {
            return other != null &&
                this.FullName.Equals(other.FullName) &&
                this.IsClass == other.IsClass &&
                this.IsStruct == other.IsStruct &&
                this.IsAbstract == other.IsAbstract &&
                this.IsInterface == other.IsInterface;
        }

        /// <summary>
        /// Determines if this <see cref="NonGenericTypeWrapper"/> object equals the given <see cref="IType"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IType"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public override bool Equals(IType other)
        {
            if (other is INonGenericType)
            {
                return Equals((INonGenericType)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        /// <summary>
        /// Gets a method with the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IMethod GetMethod(string name)
        {
            return Methods.SingleOrDefault(m => m.Name.Equals(name));
        }
    }
}
