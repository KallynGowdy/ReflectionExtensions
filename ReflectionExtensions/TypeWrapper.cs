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
    /// Defines a wrapper class for a type.
    /// </summary>
    public class TypeWrapper : IType
    {
        /// <summary>
        /// Gets the type that this wrapper represents.
        /// </summary>
        public Type RepresentedType
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new ReflectionExtensions.TypeWrapper object around the given type.
        /// </summary>
        /// <param name="representedType">The type to augment.</param>
        public TypeWrapper(Type representedType)
        {
            Contract.Requires(representedType != null);
            this.RepresentedType = representedType;
        }

        public System.Reflection.Assembly Assembly
        {
            get { return RepresentedType.Assembly; }
        }

        public bool IsClass
        {
            get { return RepresentedType.IsClass; }
        }

        public bool IsStruct
        {
            get { return RepresentedType.IsValueType; }
        }

        public bool IsAbstract
        {
            get { return RepresentedType.IsAbstract; }
        }

        public string Name
        {
            get { return RepresentedType.Name; }
        }

        public string FullName
        {
            get { return RepresentedType.FullName; }
        }

        public IEnumerable<IMember> Members
        {
            get { return Fields.OfType<IMember>().Concat(Properties).Concat(Methods); }
        }

        public IEnumerable<IField> Fields
        {
            get { return RepresentedType.GetFields(BindingFlags.Public | BindingFlags.Instance).Select(f => f.Wrap()); }
        }

        public IEnumerable<IProperty> Properties
        {
            get { return RepresentedType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Wrap()); }
        }

        /// <summary>
        /// Gets a list of public non-static not auto-generated methods that belong to this type.
        /// </summary>
        public IEnumerable<IMethod> Methods
        {
            get
            {
                return RepresentedType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => !m.IsSpecialName).Select(m => m.Wrap());
            }
        }

        public IEnumerable<IStorageMember> StorageMembers
        {
            get { return Fields.OfType<IStorageMember>().Concat(Properties); }
        }


        public IEnumerable<IMethod> Constructors
        {
            get { return RepresentedType.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Select(c => c.Wrap()); }
        }


        public IEnumerable<IMethod> GetMethods(string name)
        {
            return Methods.Where(m => m.Name.Equals(name));
        }

        public TReturn Invoke<TReturn>(string name, object reference, params object[] arguments)
        {

            INonGenericMethod method = GetMethods(name).OfType<INonGenericMethod>().SingleOrDefault(m => m.Parameters.SequenceEqual(arguments, (p, a) => p.ReturnType.IsAssignableFrom(a.GetType())));
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
        public TReturn Invoke<TReturn>(string name, object reference, Type[] genericArguments, object[] arguments)
        {


            INonGenericMethod method = GetMethods(name).OfType<INonGenericMethod>().SingleOrDefault(m => m.Parameters.SequenceEqual(arguments, (p, a) => p.ReturnType.IsAssignableFrom(a.GetType())));
            if (method == null)
            {
                throw new MissingMethodException(string.Format("The method, {0}, could not be found with the signature {0}({1})", name, string.Join(", ", arguments.Select(a => a.GetType().Name).ToArray())));
            }
            else
            {
                return method.Invoke<TReturn>(reference, genericArguments, arguments);
            }
        }

        public override int GetHashCode()
        {
            return Util.HashCode(FullName, IsClass, IsStruct, IsAbstract, Assembly);
        }

        public override string ToString()
        {
            return Name;
        }

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

        public bool Equals(IType other)
        {
            return other != null &&
                this.FullName.Equals(other.FullName) &&
                this.IsClass == other.IsClass &&
                this.IsStruct == other.IsStruct &&
                this.IsAbstract == other.IsAbstract &&
                this.Assembly.Equals(other.Assembly);
        }

        public IMethod GetMethod(string name)
        {
            return Methods.SingleOrDefault(m => m.Name.Equals(name));
        }


        public bool IsGenericType
        {
            get { return RepresentedType.IsGenericType; }
        }

        public IEnumerable<IGenericParameter> GenericArguments
        {
            get { return RepresentedType.GetGenericArguments().Select(a => new GenericParameter(a)); }
        }

        public IType BaseType
        {
            get
            {
                return RepresentedType.BaseType.Wrap();
            }
        }

        /// <summary>
        /// Determines if this type inherits from the given base type.
        /// </summary>
        /// <param name="baseType">The type to check inheritance from.</param>
        /// <returns>Returns true if this type inherits from the given base type, otherwise false.</returns>
        public bool InheritsFrom(IType baseType)
        {
            IType objectType = typeof(object).Wrap();

            //Every object inherits from System.Object
            if (baseType.Equals(objectType))
            {
                return true;
            }

            IType inheritedType = this.BaseType;

            //Otherwise, go through the inheritance chain and check for the type.
            while (inheritedType != null && !inheritedType.Equals(objectType))
            {
                if (inheritedType.Equals(inheritedType))
                {
                    return true;
                }
            }
            return false;
        }


        public bool IsInterface
        {
            get { return RepresentedType.IsInterface; }
        }
    }
}
