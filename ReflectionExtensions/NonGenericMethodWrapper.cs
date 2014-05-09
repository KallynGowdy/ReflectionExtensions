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


using Fasterflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReflectionExtensions
{
    /// <summary>
    /// Defines a wrapper class around a MethodBase object.
    /// </summary>
    /// <remarks>
    /// Note that while this method wrapper is normally used for non-generic methods, not all MethodBase objects need to be non-generic.
    /// This class just doesn't provide support for generic methods. Besides, some MethodBase objects can be generic but not accept generic arguments,
    /// such as a constructor.
    /// </remarks>
    public class NonGenericMethodWrapper : MemberBase, INonGenericMethod
    {
        MethodInvoker invoker;
        ConstructorInvoker ctor;

        /// <summary>
        /// Creates a new wrapper around the given method.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown if the given method is null.</exception>
        /// <param name="method">The method to augment. Must be non generic.</param>
        public NonGenericMethodWrapper(MethodBase method) : base(method, (method is MethodInfo ? ((MethodInfo)method).ReturnType : method.DeclaringType).Wrap())
        {
            if(method == null)
            {
                throw new ArgumentNullException("method");
            }
            
            this.WrappedMethod = method;
            if (method is ConstructorInfo)
            {
                ctor = ((ConstructorInfo)method).DelegateForCreateInstance();
            }
            else
            {
                MethodInfo m = (MethodInfo)method;
                invoker = m.DelegateForCallMethod();
            }
        }

        /// <summary>
        /// Gets the method that this object is wrapped around.
        /// </summary>
        public MethodBase WrappedMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether this method is virtual.
        /// </summary>
        public bool IsVirtual
        {
            get { return WrappedMethod.IsVirtual; }
        }

        /// <summary>
        /// Gets whether this method is abstract.
        /// </summary>
        public bool IsAbstract
        {
            get { return WrappedMethod.IsAbstract; }
        }

        /// <summary>
        /// Gets whether this method is final.
        /// </summary>
        public bool IsFinal
        {
            get { return WrappedMethod.IsFinal; }
        }

        /// <summary>
        /// Gets the access modifier that defines what can access this object.
        /// </summary>
        public AccessModifier Access
        {
            get
            {
                return WrappedMethod.GetAccessModifiers();
            }
        }

        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <typeparam name="T">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <param name="defaultValue">The value to return if the return type of this method is void.</param>
        /// <returns>
        /// Returns the value returned from the method cast into the given type. Returns the default value if the return type is void or null.
        /// </returns>
        /// <exception cref="TypeArgumentException">T</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public T Invoke<T>(object reference, object arguments, T defaultValue)
        {
            try
            {
                T value = (T)Invoke(reference, arguments);
                return value != null ? value : defaultValue;
            }
            catch (InvalidCastException e)
            {
                throw new TypeArgumentException(string.Format("The returned value from the method cannot be cast into the given type. ({0})", typeof(T)), "T", e);
            }
        }

        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <typeparam name="T">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <returns>
        /// Returns the value returned from the method cast into the given type. Returns the default value if the return type is void or null.
        /// </returns>
        public T Invoke<T>(object reference, object arguments)
        {
            return Invoke<T>(reference, arguments, default(T));
        }

        /// <summary>
        /// Invokes this method using the given objects as arguments.
        /// </summary>
        /// <typeparam name="TReturn">The type to cast the returned value into.</typeparam>
        /// <param name="reference">A reference to the object whose type contains this method.</param>
        /// <param name="arguments">A list of arguments whose order and type matches the methods signature.</param>
        /// <returns>
        /// Returns the value returned from the method cast into the given type. Returns default(<typeparamref name="TReturn" />) value if the return type is void or null.
        /// </returns>
        /// <exception cref="TypeArgumentException">
        /// Thrown if the value returned from the method cannot be cast into the given type.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        public TReturn Invoke<TReturn>(object reference, params object[] arguments)
        {
            if (ctor != null)
            {
                TReturn value = CastOrThrow<TReturn>(ctor(arguments));
                return value != null ? value : default(TReturn);
            }
            else if (invoker != null)
            {
                TReturn value = CastOrThrow<TReturn>(invoker(reference, arguments));
                return value != null ? value : default(TReturn);
            }
            else
            {
                TReturn value = CastOrThrow<TReturn>(WrappedMethod.Invoke(reference, arguments));
                return value != null ? value : default(TReturn);
            }
        }

        private static T CastOrThrow<T>(object value)
        {
            try
            {
                return (T)value;
            }
            catch (InvalidCastException e)
            {
                throw new TypeArgumentException(string.Format("The returned value from the method cannot be cast into the given type. ({0})", typeof(T)), "T", e);
            }
        }

        /// <summary>
        /// Invokes this method using the given object's members as arguments.
        /// </summary>
        /// <param name="reference">A reference to the object that contains this method.</param>
        /// <param name="arguments">An object whose members define the values to pass to the method.</param>
        /// <returns>
        /// Returns the value returned from the method. Returns null if the return type is void or null.
        /// </returns>
        public object Invoke(object reference, object arguments)
        {
            var parameters = arguments != null ? arguments.GetType().Wrap().StorageMembers.Where(a => a.CanRead).Join(WrappedMethod.GetParameters(), a => a.Name, p => p.Name, (a, p) => new { Argument = a, Parameter = p }) : null;
            if (parameters != null)
            {
                parameters = parameters.OrderBy(v => v.Parameter.Position);
                return Invoke<object>(reference, parameters.Select(v => v.Argument.GetValue(arguments)).ToArray());
            }
            else
            {
                return Invoke<object>(reference, null);
            }
        }

        /// <summary>
        /// Gets the list of parameters that this method takes as arguments.
        /// </summary>
        public IEnumerable<IParameter> Parameters
        {
            get { return WrappedMethod.GetParameters().Select(p => p.Wrap()); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}({1})", this.Name, string.Join(", ", this.Parameters));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Util.HashCode(75353, Name, ReturnType, EnclosingType, Access, Parameters);
        }

        /// <summary>
        /// Determines if this <see cref="NonGenericMethodWrapper"/> object equals the given <see cref="Object"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the obj object, otherwise false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IMethod)
            {
                return Equals((IMethod)obj);
            }
            else if (obj is IMember)
            {
                return Equals((IMember)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Determines if this <see cref="NonGenericMethodWrapper"/> object equals the given <see cref="IMember"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMember"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public override bool Equals(IMember other)
        {
            if (other is IMethod)
            {
                return Equals((IMethod)other);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if this <see cref="NonGenericMethodWrapper"/> object equals the given <see cref="IMethod"/> object.
        /// </summary>
        /// <param name="other">The <see cref="IMethod"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(IMethod other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.Access.Equals(other.Access) &&
                this.ReturnType.Equals(other.ReturnType) &&
                this.EnclosingType.Equals(other.EnclosingType) &&
                this.Parameters.SequenceEqual(other.Parameters);
        }

        /// <summary>
        /// Determines if this <see cref="NonGenericMethodWrapper"/> object equals the given <see cref="INonGenericMethod"/> object.
        /// </summary>
        /// <param name="other">The <see cref="INonGenericMethod"/> object to compare with this object.</param>
        /// <returns>
        /// Returns true if this object object is equal to the other object, otherwise false.
        /// </returns>
        public bool Equals(INonGenericMethod other)
        {
            return other != null &&
                this.Name.Equals(other.Name) &&
                this.IsAbstract == other.IsAbstract &&
                this.IsFinal == other.IsFinal &&
                this.IsVirtual == other.IsVirtual &&
                this.Access == other.Access &&
                this.EnclosingType.Equals(other.EnclosingType) &&
                this.Parameters.SequenceEqual(other.Parameters);
        }
    }
}
