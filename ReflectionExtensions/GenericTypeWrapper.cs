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

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class for a generic type.
    /// </summary>
    public class GenericTypeWrapper : TypeWrapperBase, IGenericType
    {
        /// <summary>
        /// A dictionary that relates generated types to type argument list names.
        /// </summary>
        private Dictionary<string, NonGenericTypeWrapper> generatedTypes = new Dictionary<string, NonGenericTypeWrapper>();

        /// <summary>
        /// Creates a new GenericTypeWrapper from the given type.
        /// </summary>
        /// <param name="type">The type to wrap.</param>
        public GenericTypeWrapper(Type type) : base(type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsGenericType)
            {
                throw new ArgumentException("The given type must be a generic type.", "type");
            }
            if (!type.ContainsGenericParameters)
            {
                throw new ArgumentException("The given type must contain generic unfilled parameters.", "type");
            }
        }

        /// <summary>
        /// Gets the list of generic type parameters that can be passed to this type.
        /// </summary>
        public IEnumerable<IGenericParameter> TypeParameters
        {
            get { return WrappedType.GetGenericArguments().Where(a => a.IsGenericParameter).Select(a => new GenericParameter(a)); }
        }

        /// <summary>
        /// Gets the key that is stored in the dictionary from the given list of types.
        /// </summary>
        /// <param name="types">A list of types to generate a key from.</param>
        /// <returns></returns>
        private static string getKey(Type[] types)
        {
            if(types == null)
            {
                throw new ArgumentNullException("types");
            }
            return string.Join<Type>(", ", types);
        }

        /// <summary>
        /// Creates a new ReflectionExtensions.INonGenericType from this type using the given types as arguments.
        /// </summary>
        /// <param name="typeArguments">The list of type arguments to use for the new type.</param>
        /// <returns>
        /// Returns a new ReflectionINonGenericType.
        /// </returns>
        /// <exception cref="System.ArgumentException">One or more of the given type arguments does not match a constraint.</exception>
        public INonGenericType MakeGenericType(params Type[] typeArguments)
        {
            string key = getKey(typeArguments);
            NonGenericTypeWrapper generatedType;
            if (generatedTypes.TryGetValue(key, out generatedType))
            {
                return generatedType;
            }
            else
            {
                var argsAndParams = typeArguments.Zip(GenericArguments, (a, p) => new { Argument = a, Parameter = p });
                foreach (var argParam in argsAndParams)
                {
                    if (!argParam.Parameter.MatchesConstraints(argParam.Argument.Wrap()))
                    {
                        throw new ArgumentException("One or more of the given type arguments does not match a constraint.");
                    }
                }

                generatedType = new NonGenericTypeWrapper(WrappedType.MakeGenericType(typeArguments));

                generatedTypes.Add(key, generatedType);

                return generatedType;
            }
        }

        int? hashCode = null;

        /// <summary>
        /// Gets a value indicating whether [is generic type].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is generic type]; otherwise, <c>false</c>.
        /// </value>
        public bool IsGenericType
        {
            get { return WrappedType.IsGenericType; }
        }

        /// <summary>
        /// Gets the generic arguments.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IEnumerable<IGenericParameter> GenericArguments
        {
            get { return WrappedType.GetGenericArguments().Select(a => new GenericParameter(a)); }
        }
        /// <summary>
        /// Determines if this <see cref="GenericTypeWrapper"/> object equals the given <see cref="IType"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IType"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public override bool Equals(IType other)
        {
            if (other is IGenericType)
            {
                return Equals((IGenericType)other);
            }
            else
            {
                return base.Equals(other);
            }
        }

        /// <summary>
        /// Determines if this <see cref="GenericTypeWrapper"/> object equals the given <see cref="IGenericType"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IGenericType"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IGenericType other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.Access == other.Access &&
                this.IsAbstract == other.IsAbstract &&
                this.IsClass == other.IsClass &&
                this.IsStruct == other.IsStruct &&
                this.IsInterface == other.IsInterface &&
                this.TypeParameters.SequenceEqual(other.TypeParameters);
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
            if (obj is IGenericType)
            {
                return Equals((IGenericType)obj);
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
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (!hashCode.HasValue)
            {
                hashCode = Util.HashCode(15761, base.GetHashCode(), TypeParameters);
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
            return string.Format("{0} {1} {2}<{3}> {4}", this.Access, (IsClass ? "class" : (IsInterface ? "interface" : "struct")), this.Name, string.Join(", ", GenericArguments), this.BaseType != null ? " : " + this.BaseType.ToString() : string.Empty);
        }
    }
}
